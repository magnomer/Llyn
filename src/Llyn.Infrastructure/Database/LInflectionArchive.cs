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
/// <para>
/// Position is both the order and the key here: a feature names the inflection it belongs to by that
/// number, so no single row is ever renumbered on its own. Removing or moving an inflection rewrites
/// the entry's whole set instead, which keeps positions at <c>0 … n-1</c> and carries every feature
/// along with its inflection.
/// </para>
/// </summary>
public sealed class LInflectionArchive
{
    private readonly LDatabase _lInflectionArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens sessions through.</summary>
    public LInflectionArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lInflectionArchiveDatabase = database;
    }

    /// <summary>
    /// Adds <paramref name="inflections"/> after the inflections the entry identified by
    /// <paramref name="entryId"/> already has, each with its features in list order. The whole write is
    /// one transaction, and the new positions continue the entry's existing numbering — so adding to an
    /// entry that already has inflections extends the set rather than colliding with it.
    /// </summary>
    public void LInflectionAppend(string entryId, IReadOnlyList<LInflection> inflections)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentNullException.ThrowIfNull(inflections);

        using LDatabaseSession session = _lInflectionArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        LInflectionInsert(connection, entryId, inflections, LInflectionCountRead(connection, entryId));
        session.LDatabaseSessionCommit();
    }

    /// <summary>Reads the entry's inflections, ordered by position, each carrying its ordered features.</summary>
    public IReadOnlyList<LInflection> LInflectionRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lInflectionArchiveDatabase.LDatabaseSessionStart();
        return LInflectionSetRead(session.LDatabaseSessionConnection, entryId);
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

        using LDatabaseSession session = _lInflectionArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        LInflectionClear(connection, entryId);
        LInflectionInsert(connection, entryId, inflections, 0);
        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Deletes the single inflection at <paramref name="position"/> under <paramref name="entryId"/> and
    /// closes the gap it leaves: the inflections that remain keep their order, are renumbered
    /// <c>0 … n-1</c>, and their features move with them. Nothing happens when the entry has no
    /// inflection at that position.
    /// </summary>
    public void LInflectionDelete(string entryId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lInflectionArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LInflection> remaining = new(LInflectionSetRead(connection, entryId));
        int index = remaining.FindIndex(inflection => inflection.LInflectionPosition == position);
        if (index < 0)
        {
            return;
        }

        remaining.RemoveAt(index);
        LInflectionClear(connection, entryId);
        LInflectionInsert(connection, entryId, remaining, 0);

        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Moves the inflection at <paramref name="position"/> under <paramref name="entryId"/> to
    /// <paramref name="target"/>, rewriting the whole set so positions stay <c>0 … n-1</c> and every
    /// feature follows its inflection. A target outside the set is clamped into it, and nothing moves
    /// when the entry has no inflection at that position.
    /// </summary>
    public void LInflectionMove(string entryId, int position, int target)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lInflectionArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LInflection> ordered = new(LInflectionSetRead(connection, entryId));
        int index = ordered.FindIndex(inflection => inflection.LInflectionPosition == position);
        if (index < 0)
        {
            return;
        }

        LInflection moved = ordered[index];
        ordered.RemoveAt(index);
        ordered.Insert(Math.Clamp(target, 0, ordered.Count), moved);

        LInflectionClear(connection, entryId);
        LInflectionInsert(connection, entryId, ordered, 0);

        session.LDatabaseSessionCommit();
    }

    // The entry's inflections in order on a connection the caller already holds: one query for the
    // inflections and one for every feature of all of them, rather than a round-trip per inflection.
    private static IReadOnlyList<LInflection> LInflectionSetRead(SqliteConnection connection, string entryId)
    {
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

        IReadOnlyDictionary<int, IReadOnlyList<LFeature>> features =
            LInflectionFeatureRead(connection, entryId);

        List<LInflection> inflections = [];
        foreach ((int position, string text, string? local, string? speechId) in rows)
        {
            inflections.Add(new LInflection(
                entryId,
                position,
                text,
                local,
                speechId,
                features.TryGetValue(position, out IReadOnlyList<LFeature>? found) ? found : []));
        }

        return inflections;
    }

    // How many inflections the entry already has, so an append continues its numbering.
    private static int LInflectionCountRead(SqliteConnection connection, string entryId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM inflection WHERE entry_id = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    // Every feature of every inflection the entry has, in one query, grouped by the inflection position
    // it belongs to.
    private static IReadOnlyDictionary<int, IReadOnlyList<LFeature>> LInflectionFeatureRead(
        SqliteConnection connection, string entryId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT inflection_position, feature_id, value_id
            FROM inflection_feature WHERE entry_id = $entry
            ORDER BY inflection_position, position;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        Dictionary<int, IReadOnlyList<LFeature>> grouped = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            int position = reader.GetInt32(0);
            if (!grouped.TryGetValue(position, out IReadOnlyList<LFeature>? existing))
            {
                existing = new List<LFeature>();
                grouped[position] = existing;
            }

            ((List<LFeature>)existing).Add(new LFeature(reader.GetString(1), reader.GetString(2)));
        }

        return grouped;
    }

    private static void LInflectionInsert(
        SqliteConnection connection, string entryId, IReadOnlyList<LInflection> inflections, int first)
    {
        for (int index = 0; index < inflections.Count; index++)
        {
            LInflection inflection = inflections[index];
            int position = first + index;

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
