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
            SELECT source_id, COUNT(*) FROM example
            WHERE source_id IS NOT NULL
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
        usages.AddRange(LReferenceCardRead(
            connection,
            id,
            LOwner.LOwnerMeaning,
            """
            SELECT sense.id, sense.entry_id, entry.headword, entry.language,
                   sense.title_state, sense.title,
                   CASE WHEN sense.gloss IS NOT NULL THEN 'specified' ELSE sense.definition_state END,
                   COALESCE(sense.gloss, sense.definition)
            FROM sense_example link
            JOIN example ON example.id = link.example_id
            JOIN sense ON sense.id = link.sense_id
            JOIN entry ON entry.id = sense.entry_id
            WHERE example.source_id = $id
            GROUP BY sense.id
            ORDER BY entry.headword, sense.position;
            """));
        usages.AddRange(LReferenceCardRead(
            connection,
            id,
            LOwner.LOwnerCollocation,
            """
            SELECT collocation.id, collocation.entry_id, entry.headword, entry.language,
                   collocation.title_state, collocation.title,
                   collocation.expression_state, collocation.expression
            FROM collocation_example link
            JOIN example ON example.id = link.example_id
            JOIN collocation ON collocation.id = link.collocation_id
            JOIN entry ON entry.id = collocation.entry_id
            WHERE example.source_id = $id
            GROUP BY collocation.id
            ORDER BY entry.headword, collocation.position;
            """));
        usages.AddRange(LReferenceExampleRead(connection, id));
        return usages;
    }

    internal static int LReferenceUsageRead(SqliteConnection connection, string id)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT COUNT(*) FROM example WHERE source_id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    internal static void LReferenceUsageClear(SqliteConnection connection, string id)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "UPDATE example SET source_state = 'unspecified', source_id = NULL WHERE source_id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    private static IReadOnlyList<LUsage> LReferenceCardRead(
        SqliteConnection connection,
        string id,
        LOwner owner,
        string statement)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = statement;
        command.Parameters.AddWithValue("$id", id);

        List<LUsage> usages = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            LStateValue title = LStateColumn.LStateColumnRead(reader, 4);
            if (title.LStateValueEmpty)
            {
                title = LStateColumn.LStateColumnRead(reader, 6);
            }

            usages.Add(new LUsage(
                reader.GetString(0),
                owner,
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                title));
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
