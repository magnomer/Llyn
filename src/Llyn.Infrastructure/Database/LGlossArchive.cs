using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LGlossArchive
{
    private const long LGlossShelf = 1_000_000_000L;

    private readonly LDatabase _lGlossArchiveDatabase;

    public LGlossArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lGlossArchiveDatabase = database;
    }

    public IReadOnlyList<LGloss> LGlossExampleRead(long exampleId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);

        using LDatabaseSession session = _lGlossArchiveDatabase.LDatabaseSessionStart();
        IReadOnlyDictionary<long, IReadOnlyList<LGloss>> read =
            LGlossExampleRead(session.LDatabaseSessionConnection, [exampleId]);
        return read.TryGetValue(exampleId, out IReadOnlyList<LGloss>? glosses) ? glosses : [];
    }

    public IReadOnlyList<long> LGlossExampleSave(long exampleId, IReadOnlyList<LGloss> glosses)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);
        ArgumentNullException.ThrowIfNull(glosses);

        using LDatabaseSession session = _lGlossArchiveDatabase.LDatabaseSessionStart();
        IReadOnlyList<long> written = LGlossExampleSave(session.LDatabaseSessionConnection, exampleId, glosses);
        session.LDatabaseSessionCommit();
        return written;
    }

    internal static IReadOnlyDictionary<long, IReadOnlyList<LGloss>> LGlossExampleRead(
        SqliteConnection connection, IReadOnlyList<long> exampleIds)
    {
        Dictionary<long, List<LGloss>> read = [];
        if (exampleIds.Count == 0)
        {
            return read.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<LGloss>)pair.Value);
        }

        using SqliteCommand command = connection.CreateCommand();
        List<string> names = new(exampleIds.Count);
        foreach (long exampleId in exampleIds.Distinct())
        {
            string name = $"$example{names.Count}";
            names.Add(name);
            command.Parameters.AddWithValue(name, exampleId);
        }

        command.CommandText =
            $"""
            SELECT example_translation_id, example_parent, language, text_state, text
            FROM example_translation
            WHERE example_parent IN ({string.Join(", ", names)})
            ORDER BY example_parent, position;
            """;

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            long exampleId = reader.GetInt64(1);
            if (!read.TryGetValue(exampleId, out List<LGloss>? glosses))
            {
                glosses = [];
                read[exampleId] = glosses;
            }

            glosses.Add(new LGloss(
                reader.GetInt64(0),
                reader.GetString(2),
                LStateColumn.LStateColumnRead(reader, 3)));
        }

        return read.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<LGloss>)pair.Value);
    }

    internal static IReadOnlyList<long> LGlossExampleSave(
        SqliteConnection connection, long exampleId, IReadOnlyList<LGloss> glosses)
    {
        LGlossPositionAdjust(connection, exampleId);
        List<long> written = new(glosses.Count);
        for (int position = 0; position < glosses.Count; position++)
        {
            LGloss gloss = glosses[position];
            bool kept = gloss.LGlossId > 0
                && !written.Contains(gloss.LGlossId)
                && LGlossChange(connection, exampleId, position, gloss);
            written.Add(kept ? gloss.LGlossId : LGlossSave(connection, exampleId, position, gloss));
        }

        LGlossShelfClear(connection, exampleId);
        return written;
    }

    private static void LGlossPositionAdjust(SqliteConnection connection, long exampleId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "UPDATE example_translation SET position = position + $shift WHERE example_parent = $example;";
        command.Parameters.AddWithValue("$shift", LGlossShelf);
        command.Parameters.AddWithValue("$example", exampleId);
        command.ExecuteNonQuery();
    }

    private static void LGlossShelfClear(SqliteConnection connection, long exampleId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "DELETE FROM example_translation WHERE example_parent = $example AND position >= $shift;";
        command.Parameters.AddWithValue("$shift", LGlossShelf);
        command.Parameters.AddWithValue("$example", exampleId);
        command.ExecuteNonQuery();
    }

    private static bool LGlossChange(SqliteConnection connection, long exampleId, int position, LGloss gloss)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE example_translation
            SET position = $position, language = $language, text_state = $textState, text = $text
            WHERE example_translation_id = $id AND example_parent = $example;
            """;
        command.Parameters.AddWithValue("$id", gloss.LGlossId);
        command.Parameters.AddWithValue("$example", exampleId);
        LGlossParameterApply(command, position, gloss);
        return command.ExecuteNonQuery() == 1;
    }

    private static long LGlossSave(SqliteConnection connection, long exampleId, int position, LGloss gloss)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO example_translation (example_parent, position, language, text_state, text)
            VALUES ($example, $position, $language, $textState, $text)
            RETURNING example_translation_id;
            """;
        command.Parameters.AddWithValue("$example", exampleId);
        LGlossParameterApply(command, position, gloss);
        return (long)command.ExecuteScalar()!;
    }

    private static void LGlossParameterApply(SqliteCommand command, int position, LGloss gloss)
    {
        command.Parameters.AddWithValue("$position", position);
        command.Parameters.AddWithValue("$language", gloss.LGlossLanguage);
        LStateColumn.LStateColumnApply(command, "text", gloss.LGlossText);
    }
}
