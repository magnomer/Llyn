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

    public IReadOnlyDictionary<long, int> LReferenceUsageRead()
    {
        using LDatabaseSession session = _lReferenceUsageDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT reference_ref, COUNT(*) FROM example
            WHERE reference_ref IS NOT NULL
            GROUP BY reference_ref;
            """;

        Dictionary<long, int> counts = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            counts[reader.GetInt64(0)] = reader.GetInt32(1);
        }

        return counts;
    }

    public IReadOnlyList<LUsage> LReferenceUsageRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lReferenceUsageDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LUsage> usages = [];
        usages.AddRange(LReferenceCardRead(
            connection,
            id,
            LOwner.LOwnerMeaning,
            """
            SELECT sense.sense_id, sense.entry_parent, entry.headword, entry.language,
                   sense.title_state, sense.title,
                   sense.definition_state, sense.definition
            FROM sense_example link
            JOIN example ON example.example_id = link.example_ref
            JOIN sense ON sense.sense_id = link.sense_parent
            JOIN entry ON entry.entry_id = sense.entry_parent
            WHERE example.reference_ref = $id
            GROUP BY sense.sense_id
            ORDER BY entry.headword, sense.position;
            """));
        usages.AddRange(LReferenceCardRead(
            connection,
            id,
            LOwner.LOwnerCollocation,
            """
            SELECT collocation.collocation_id, collocation.entry_parent, entry.headword, entry.language,
                   collocation.title_state, collocation.title,
                   collocation.expression_state, collocation.expression
            FROM collocation_example link
            JOIN example ON example.example_id = link.example_ref
            JOIN collocation ON collocation.collocation_id = link.collocation_parent
            JOIN entry ON entry.entry_id = collocation.entry_parent
            WHERE example.reference_ref = $id
            GROUP BY collocation.collocation_id
            ORDER BY entry.headword, collocation.position;
            """));
        usages.AddRange(LReferenceExampleRead(connection, id));
        return usages;
    }

    internal static int LReferenceUsageRead(SqliteConnection connection, long id)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT COUNT(*) FROM example WHERE reference_ref = $id;
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    internal static void LReferenceUsageClear(SqliteConnection connection, long id)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "UPDATE example SET reference_state = 'unspecified', reference_ref = NULL WHERE reference_ref = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    private static IReadOnlyList<LUsage> LReferenceCardRead(
        SqliteConnection connection,
        long id,
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
                reader.GetInt64(0),
                owner,
                reader.GetInt64(1),
                reader.GetString(2),
                reader.GetString(3),
                title));
        }

        return usages;
    }

    private static IReadOnlyList<LUsage> LReferenceExampleRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT example.example_id, example.language,
                   example.text_state, example.text,
                   COALESCE(gloss.text_state, 'unspecified'), gloss.text
            FROM example
            LEFT JOIN example_translation gloss
                ON gloss.example_parent = example.example_id AND gloss.position = 0
            WHERE example.reference_ref = $id
            ORDER BY example.text;
            """;
        command.Parameters.AddWithValue("$id", id);

        List<LUsage> usages = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            long example = reader.GetInt64(0);
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
