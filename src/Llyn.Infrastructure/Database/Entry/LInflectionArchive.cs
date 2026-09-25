using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LInflectionArchive : LInflectionVault
{
    private readonly LDatabase _lInflectionArchiveDatabase;

    public LInflectionArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lInflectionArchiveDatabase = database;
    }

    public IReadOnlyList<LInflection> LInflectionAppend(long entryId, IReadOnlyList<LInflection> inflections)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(inflections);

        using LDatabaseSession session = _lInflectionArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        IReadOnlyList<LInflection> stored =
            LInflectionInsert(connection, entryId, inflections, LInflectionCountRead(connection, entryId));
        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LInflection> LInflectionRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lInflectionArchiveDatabase.LDatabaseSessionStart();
        return LInflectionSetRead(session.LDatabaseSessionConnection, entryId);
    }

    public IReadOnlyList<LInflection> LInflectionSet(long entryId, IReadOnlyList<LInflection> inflections)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(inflections);

        using LDatabaseSession session = _lInflectionArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        LInflectionClear(connection, entryId);
        IReadOnlyList<LInflection> stored = LInflectionInsert(connection, entryId, inflections, 0);
        session.LDatabaseSessionCommit();
        return stored;
    }

    public void LInflectionRegularSave(long inflectionId, bool regular)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(inflectionId);

        using LDatabaseSession session = _lInflectionArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "UPDATE inflection SET regular = $regular WHERE inflection_id = $id;";
            command.Parameters.AddWithValue("$regular", regular ? 1 : 0);
            command.Parameters.AddWithValue("$id", inflectionId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    private static IReadOnlyList<LInflection> LInflectionSetRead(SqliteConnection connection, long entryId)
    {
        List<(long LInflectionId, int LInflectionPosition, string LInflectionText, string? LInflectionLocal,
            long? LInflectionSpeechId, bool LInflectionRegular)> rows = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                SELECT inflection_id, position, text, local, speech_value_ref, regular
                FROM inflection WHERE entry_parent = $entry ORDER BY position;
                """;
            command.Parameters.AddWithValue("$entry", entryId);

            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add((
                    reader.GetInt64(0),
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.IsDBNull(3) ? null : reader.GetString(3),
                    reader.IsDBNull(4) ? null : reader.GetInt64(4),
                    reader.GetInt32(5) != 0));
            }
        }

        IReadOnlyDictionary<long, IReadOnlyList<long>> morphology =
            LInflectionMorphologyRead(connection, entryId);

        List<LInflection> inflections = [];
        foreach ((long id, int position, string text, string? local, long? speechValueId, bool regular) in rows)
        {
            inflections.Add(new LInflection(
                id,
                entryId,
                position,
                text,
                local,
                speechValueId,
                morphology.TryGetValue(id, out IReadOnlyList<long>? found) ? found : [],
                regular));
        }

        return inflections;
    }

    private static int LInflectionCountRead(SqliteConnection connection, long entryId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM inflection WHERE entry_parent = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static IReadOnlyDictionary<long, IReadOnlyList<long>> LInflectionMorphologyRead(
        SqliteConnection connection, long entryId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT inflection_feature.inflection_parent, inflection_feature.morphology_value_ref
            FROM inflection_feature
            JOIN inflection ON inflection.inflection_id = inflection_feature.inflection_parent
            WHERE inflection.entry_parent = $entry
            ORDER BY inflection.position, inflection_feature.position;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        Dictionary<long, IReadOnlyList<long>> grouped = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            long inflectionId = reader.GetInt64(0);
            if (!grouped.TryGetValue(inflectionId, out IReadOnlyList<long>? existing))
            {
                existing = new List<long>();
                grouped[inflectionId] = existing;
            }

            ((List<long>)existing).Add(reader.GetInt64(1));
        }

        return grouped;
    }

    private static IReadOnlyList<LInflection> LInflectionInsert(
        SqliteConnection connection, long entryId, IReadOnlyList<LInflection> inflections, int first)
    {
        List<LInflection> stored = new(inflections.Count);
        for (int index = 0; index < inflections.Count; index++)
        {
            LInflection inflection = inflections[index];
            int position = first + index;

            long id;
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    """
                    INSERT INTO inflection (entry_parent, position, text, local, speech_value_ref, regular)
                    VALUES ($entry, $position, $text, $local, $speech, $regular)
                    RETURNING inflection_id;
                    """;
                command.Parameters.AddWithValue("$entry", entryId);
                command.Parameters.AddWithValue("$position", position);
                command.Parameters.AddWithValue("$text", inflection.LInflectionText);
                command.Parameters.AddWithValue("$local", (object?)inflection.LInflectionLocal ?? DBNull.Value);
                command.Parameters.AddWithValue(
                    "$speech", (object?)inflection.LInflectionSpeechId ?? DBNull.Value);
                command.Parameters.AddWithValue("$regular", inflection.LInflectionRegular ? 1 : 0);
                id = Convert.ToInt64(command.ExecuteScalar());
            }

            LInflectionMorphologyInsert(connection, id, inflection.LInflectionMorphology);
            stored.Add(inflection with
            {
                LInflectionId = id,
                LInflectionEntryId = entryId,
                LInflectionPosition = position,
            });
        }

        return stored;
    }

    private static void LInflectionMorphologyInsert(
        SqliteConnection connection, long inflectionId, IReadOnlyList<long> morphology)
    {
        for (int position = 0; position < morphology.Count; position++)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO inflection_feature (inflection_parent, position, morphology_value_ref)
                VALUES ($inflection, $position, $value);
                """;
            command.Parameters.AddWithValue("$inflection", inflectionId);
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$value", morphology[position]);
            command.ExecuteNonQuery();
        }
    }

    private static void LInflectionClear(SqliteConnection connection, long entryId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM inflection WHERE entry_parent = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);
        command.ExecuteNonQuery();
    }
}
