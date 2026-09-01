using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LInflectionArchive
{
    private readonly LDatabase _lInflectionArchiveDatabase;

    public LInflectionArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lInflectionArchiveDatabase = database;
    }

    public void LInflectionAppend(string entryId, IReadOnlyList<LInflection> inflections)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentNullException.ThrowIfNull(inflections);

        using LDatabaseSession session = _lInflectionArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        LInflectionInsert(connection, entryId, inflections, LInflectionCountRead(connection, entryId));
        session.LDatabaseSessionCommit();
    }

    public IReadOnlyList<LInflection> LInflectionRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lInflectionArchiveDatabase.LDatabaseSessionStart();
        return LInflectionSetRead(session.LDatabaseSessionConnection, entryId);
    }

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

    private static int LInflectionCountRead(SqliteConnection connection, string entryId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM inflection WHERE entry_id = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);
        return Convert.ToInt32(command.ExecuteScalar());
    }

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
