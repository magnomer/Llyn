using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists an entry's inflected forms and the ordered grammatical features each carries. Inflections
/// are written as ordered child rows under an entry, so reordering rewrites <c>position</c> only and
/// never touches the entry id; a feature's identity is <c>(entry_id, inflection_position, position)</c>.
/// Deleting an inflection removes its features through the foreign-key cascade, and deleting the entry
/// removes both. Only stable ids are stored here — feature and value display names are resolved from
/// the morphology vocabulary (<see cref="LMorphologyArchive"/>).
/// </summary>
public sealed class LInflectionArchive
{
    private readonly LDatabase _lInflectionArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LInflectionArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lInflectionArchiveDatabase = database;
    }

    /// <summary>
    /// Writes <paramref name="inflections"/> as ordered child rows under the entry identified by
    /// <paramref name="entryId"/>, each with its features in list order. The whole write is one
    /// transaction; the inflection and feature positions are assigned from list order.
    /// </summary>
    public void LInflectionCreate(string entryId, IReadOnlyList<LInflection> inflections)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentNullException.ThrowIfNull(inflections);

        using SqliteConnection connection = _lInflectionArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();
        LInflectionInsert(connection, entryId, inflections);
        transaction.Commit();
    }

    /// <summary>Reads the entry's inflections, ordered by position, each carrying its ordered features.</summary>
    public IReadOnlyList<LInflection> LInflectionRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using SqliteConnection connection = _lInflectionArchiveDatabase.LDatabaseRead();

        List<(int Position, string Text, string? Local, string? SpeechId)> rows = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                SELECT position, text, local, part_of_speech_id
                FROM inflection WHERE entry_id = $entry ORDER BY position;
                """;
            command.Parameters.AddWithValue("$entry", entryId);

            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add((
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.IsDBNull(2) ? null : reader.GetString(2),
                    reader.IsDBNull(3) ? null : reader.GetString(3)));
            }
        }

        List<LInflection> inflections = [];
        foreach ((int position, string text, string? local, string? speechId) in rows)
        {
            inflections.Add(new LInflection(
                entryId,
                position,
                text,
                local,
                speechId,
                LInflectionFeatureRead(connection, entryId, position)));
        }

        return inflections;
    }

    /// <summary>
    /// Replaces the entry's inflections with <paramref name="inflections"/> in list order: existing
    /// inflection rows are cleared (their features cascade) and the new set written, so reordering
    /// rewrites positions while the entry id stays fixed.
    /// </summary>
    public void LInflectionSet(string entryId, IReadOnlyList<LInflection> inflections)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentNullException.ThrowIfNull(inflections);

        using SqliteConnection connection = _lInflectionArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();
        LInflectionClear(connection, entryId);
        LInflectionInsert(connection, entryId, inflections);
        transaction.Commit();
    }

    /// <summary>
    /// Deletes the single inflection at <paramref name="position"/> under <paramref name="entryId"/>.
    /// Its features are removed by the foreign-key cascade; other inflections are untouched.
    /// </summary>
    public void LInflectionDelete(string entryId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using SqliteConnection connection = _lInflectionArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM inflection WHERE entry_id = $entry AND position = $position;";
        command.Parameters.AddWithValue("$entry", entryId);
        command.Parameters.AddWithValue("$position", position);
        command.ExecuteNonQuery();
    }

    private static IReadOnlyList<LFeature> LInflectionFeatureRead(
        SqliteConnection connection, string entryId, int inflectionPosition)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT feature_id, value_id
            FROM inflection_feature
            WHERE entry_id = $entry AND inflection_position = $inflection
            ORDER BY position;
            """;
        command.Parameters.AddWithValue("$entry", entryId);
        command.Parameters.AddWithValue("$inflection", inflectionPosition);

        List<LFeature> features = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            features.Add(new LFeature(reader.GetString(0), reader.GetString(1)));
        }

        return features;
    }

    private static void LInflectionInsert(
        SqliteConnection connection, string entryId, IReadOnlyList<LInflection> inflections)
    {
        for (int position = 0; position < inflections.Count; position++)
        {
            LInflection inflection = inflections[position];
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    """
                    INSERT INTO inflection (entry_id, position, text, local, part_of_speech_id)
                    VALUES ($entry, $position, $text, $local, $speech);
                    """;
                command.Parameters.AddWithValue("$entry", entryId);
                command.Parameters.AddWithValue("$position", position);
                command.Parameters.AddWithValue("$text", inflection.LInflectionText);
                command.Parameters.AddWithValue("$local", (object?)inflection.LInflectionLocal ?? DBNull.Value);
                command.Parameters.AddWithValue("$speech", (object?)inflection.LInflectionSpeechId ?? DBNull.Value);
                command.ExecuteNonQuery();
            }

            LInflectionFeatureInsert(connection, entryId, position, inflection.LInflectionFeatures);
        }
    }

    private static void LInflectionFeatureInsert(
        SqliteConnection connection, string entryId, int inflectionPosition, IReadOnlyList<LFeature> features)
    {
        for (int position = 0; position < features.Count; position++)
        {
            LFeature feature = features[position];
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO inflection_feature (entry_id, inflection_position, position, feature_id, value_id)
                VALUES ($entry, $inflection, $position, $feature, $value);
                """;
            command.Parameters.AddWithValue("$entry", entryId);
            command.Parameters.AddWithValue("$inflection", inflectionPosition);
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$feature", feature.LFeatureId);
            command.Parameters.AddWithValue("$value", feature.LFeatureValueId);
            command.ExecuteNonQuery();
        }
    }

    private static void LInflectionClear(SqliteConnection connection, string entryId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM inflection WHERE entry_id = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);
        command.ExecuteNonQuery();
    }
}
