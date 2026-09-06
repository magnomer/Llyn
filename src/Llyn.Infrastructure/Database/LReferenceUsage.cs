using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LReferenceUsage
{
    private readonly LDatabase _lReferenceUsageDatabase;

    public LReferenceUsage(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lReferenceUsageDatabase = database;
    }

    public IReadOnlyDictionary<string, int> LReferenceUsageRead()
    {
        using LDatabaseSession session = _lReferenceUsageDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT source_id, COUNT(*) FROM (
                SELECT source_id FROM entry_source
                UNION ALL
                SELECT source_id FROM example WHERE source_id IS NOT NULL
            )
            GROUP BY source_id;
            """;

        Dictionary<string, int> counts = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            counts[reader.GetString(0)] = reader.GetInt32(1);
        }

        return counts;
    }

    public IReadOnlyList<LUsage> LReferenceUsageRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lReferenceUsageDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LUsage> usages = [];
        usages.AddRange(LReferenceEntryRead(connection, id));
        usages.AddRange(LReferenceExampleRead(connection, id));
        return usages;
    }

    internal static int LReferenceUsageRead(SqliteConnection connection, string id)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM entry_source WHERE source_id = $id)
                + (SELECT COUNT(*) FROM example WHERE source_id = $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    internal static void LReferenceUsageClear(SqliteConnection connection, string id)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM entry_source WHERE source_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "UPDATE example SET source_state = 'unspecified', source_id = NULL WHERE source_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
    }

    private static IReadOnlyList<LUsage> LReferenceEntryRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT entry.id, entry.headword, entry.language,
                   COALESCE((
                       SELECT CASE WHEN sense.gloss IS NOT NULL THEN 'specified' ELSE sense.definition_state END
                       FROM sense WHERE sense.entry_id = entry.id ORDER BY sense.position LIMIT 1), 'unspecified'),
                   (SELECT COALESCE(sense.gloss, sense.definition)
                    FROM sense WHERE sense.entry_id = entry.id ORDER BY sense.position LIMIT 1)
            FROM entry_source link
            JOIN entry ON entry.id = link.entry_id
            WHERE link.source_id = $id
            ORDER BY entry.headword;
            """;
        command.Parameters.AddWithValue("$id", id);

        List<LUsage> usages = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            string entry = reader.GetString(0);
            usages.Add(new LUsage(
                entry,
                LOwner.LOwnerEntry,
                entry,
                reader.GetString(1),
                reader.GetString(2),
                LStateColumn.LStateColumnRead(reader, 3)));
        }

        return usages;
    }

    private static IReadOnlyList<LUsage> LReferenceExampleRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT example.id, example.language,
                   example.text_state, example.text,
                   example.translation_state, example.translation
            FROM example
            WHERE example.source_id = $id
            ORDER BY example.text;
            """;
        command.Parameters.AddWithValue("$id", id);

        List<LUsage> usages = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            string example = reader.GetString(0);
            usages.Add(new LUsage(
                example,
                LOwner.LOwnerExample,
                example,
                LStateColumn.LStateColumnRead(reader, 2).LStateValueShow(),
                reader.GetString(1),
                LStateColumn.LStateColumnRead(reader, 4)));
        }

        return usages;
    }
}
