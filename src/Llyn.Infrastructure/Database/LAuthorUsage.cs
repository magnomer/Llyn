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

    public IReadOnlyList<LUsage> LAuthorUsageRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lAuthorUsageDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LUsage> usages = [];
        usages.AddRange(LAuthorCardRead(
            connection,
            id,
            LOwner.LOwnerMeaning,
            """
            SELECT sense.id, sense.entry_id, entry.headword, entry.language,
                   sense.title_state, sense.title,
                   CASE WHEN sense.gloss IS NOT NULL THEN 'specified' ELSE sense.definition_state END,
                   COALESCE(sense.gloss, sense.definition)
            FROM source_author credit
            JOIN example ON example.source_id = credit.source_id
            JOIN sense_example link ON link.example_id = example.id
            JOIN sense ON sense.id = link.sense_id
            JOIN entry ON entry.id = sense.entry_id
            WHERE credit.author_id = $id
            GROUP BY sense.id
            ORDER BY entry.headword, sense.position;
            """));
        usages.AddRange(LAuthorCardRead(
            connection,
            id,
            LOwner.LOwnerCollocation,
            """
            SELECT collocation.id, collocation.entry_id, entry.headword, entry.language,
                   collocation.title_state, collocation.title,
                   collocation.expression_state, collocation.expression
            FROM source_author credit
            JOIN example ON example.source_id = credit.source_id
            JOIN collocation_example link ON link.example_id = example.id
            JOIN collocation ON collocation.id = link.collocation_id
            JOIN entry ON entry.id = collocation.entry_id
            WHERE credit.author_id = $id
            GROUP BY collocation.id
            ORDER BY entry.headword, collocation.position;
            """));
        usages.AddRange(LAuthorExampleRead(connection, id));
        return usages;
    }

    private static IReadOnlyList<LUsage> LAuthorCardRead(
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

    private static IReadOnlyList<LUsage> LAuthorExampleRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT example.id, example.language,
                   example.text_state, example.text,
                   example.translation_state, example.translation
            FROM source_author credit
            JOIN example ON example.source_id = credit.source_id
            WHERE credit.author_id = $id
            GROUP BY example.id
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
