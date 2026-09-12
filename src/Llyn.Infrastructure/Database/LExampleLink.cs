using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LExampleLink
{
    private readonly LDatabase _lExampleLinkDatabase;

    public LExampleLink(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lExampleLinkDatabase = database;
    }

    public IReadOnlyList<LUsage> LExampleUsageRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lExampleLinkDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LUsage> usages = [];
        usages.AddRange(LExampleUsageRead(
            connection,
            id,
            LOwner.LOwnerMeaning,
            """
            SELECT link.sense_parent, sense.entry_parent, entry.headword, entry.language,
                   sense.title_state, sense.title,
                   sense.definition_state, sense.definition
            FROM sense_example link
            JOIN sense ON sense.sense_id = link.sense_parent
            JOIN entry ON entry.entry_id = sense.entry_parent
            WHERE link.example_ref = $id
            ORDER BY entry.headword, sense.position;
            """));
        usages.AddRange(LExampleUsageRead(
            connection,
            id,
            LOwner.LOwnerCollocation,
            """
            SELECT link.collocation_parent, collocation.entry_parent, entry.headword, entry.language,
                   collocation.title_state, collocation.title,
                   collocation.expression_state, collocation.expression
            FROM collocation_example link
            JOIN collocation ON collocation.collocation_id = link.collocation_parent
            JOIN entry ON entry.entry_id = collocation.entry_parent
            WHERE link.example_ref = $id
            ORDER BY entry.headword, collocation.position;
            """));

        return usages;
    }

    private static IReadOnlyList<LUsage> LExampleUsageRead(
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

    internal static void LExampleLinkClear(SqliteConnection connection, long exampleId)
    {
        LSentenceArchive.LSentenceExampleClear(connection, exampleId);
    }
}
