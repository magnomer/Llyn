using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LAuthorUsage
{
    private readonly LDatabase _lAuthorUsageDatabase;

    public LAuthorUsage(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lAuthorUsageDatabase = database;
    }

    public IReadOnlyList<LUsage> LAuthorUsageRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lAuthorUsageDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LUsage> usages = [];
        usages.AddRange(LAuthorCardRead(
            connection,
            id,
            LOwner.LOwnerMeaning,
            """
            SELECT sense.sense_id, sense.entry_parent, entry.headword, entry.language,
                   sense.title_state, sense.title,
                   sense.definition_state, sense.definition
            FROM reference_author credit
            JOIN example ON example.reference_ref = credit.reference_parent
            JOIN sense_example link ON link.example_ref = example.example_id
            JOIN sense ON sense.sense_id = link.sense_parent
            JOIN entry ON entry.entry_id = sense.entry_parent
            WHERE credit.author_ref = $id
            GROUP BY sense.sense_id
            ORDER BY entry.headword, sense.position;
            """));
        usages.AddRange(LAuthorCardRead(
            connection,
            id,
            LOwner.LOwnerCollocation,
            """
            SELECT collocation.collocation_id, collocation.entry_parent, entry.headword, entry.language,
                   collocation.title_state, collocation.title,
                   collocation.expression_state, collocation.expression
            FROM reference_author credit
            JOIN example ON example.reference_ref = credit.reference_parent
            JOIN collocation_example link ON link.example_ref = example.example_id
            JOIN collocation ON collocation.collocation_id = link.collocation_parent
            JOIN entry ON entry.entry_id = collocation.entry_parent
            WHERE credit.author_ref = $id
            GROUP BY collocation.collocation_id
            ORDER BY entry.headword, collocation.position;
            """));
        usages.AddRange(LAuthorExampleRead(connection, id));
        return usages;
    }

    private static IReadOnlyList<LUsage> LAuthorCardRead(
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

    private static IReadOnlyList<LUsage> LAuthorExampleRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT example.example_id, example.language,
                   example.text_state, example.text,
                   example.translation_state, example.translation
            FROM reference_author credit
            JOIN example ON example.reference_ref = credit.reference_parent
            WHERE credit.author_ref = $id
            GROUP BY example.example_id
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
