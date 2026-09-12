using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LExampleArchive
{
    private readonly LDatabase _lExampleArchiveDatabase;

    public LExampleArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lExampleArchiveDatabase = database;
    }

    public LExample LExampleCreate(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);

        LExample stored = example;

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO example (
                    language, text_state, text, reference_state, reference_ref)
                VALUES (
                    $language, $textState, $text, $referenceState, $reference)
                RETURNING example_id;
                """;
            command.Parameters.AddWithValue("$language", stored.LExampleLanguage);
            LStateColumn.LStateColumnApply(command, "text", stored.LExampleText);
            LStateColumn.LStateColumnApply(command, "reference", stored.LExampleSource);
            stored = stored with { LExampleId = (long)command.ExecuteScalar()! };
        }

        LGlossArchive.LGlossExampleSave(connection, stored.LExampleId, stored.LExampleGloss);
        LMentionArchive.LMentionExampleSave(connection, stored.LExampleId, stored.LExampleMention);
        stored = LExampleListLoad(connection, [stored])[0];

        session.LDatabaseSessionCommit();

        return stored;
    }

    public LExample? LExampleRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        return LExampleSingleRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyList<LExample> LExampleRead()
    {
        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT example_id, language, text_state, text, reference_state, reference_ref
            FROM example
            ORDER BY rowid;
            """;

        List<LExample> examples = [];
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                examples.Add(new LExample(
                    reader.GetInt64(0),
                    reader.GetString(1),
                    LStateColumn.LStateColumnRead(reader, 2),
                    LStateColumn.LStateColumnResolve(reader, 4)));
            }
        }

        return LExampleListLoad(connection, examples);
    }

    public void LExampleUpdate(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(example.LExampleId);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE example
                SET language = $language, text_state = $textState, text = $text,
                    reference_state = $referenceState, reference_ref = $reference
                WHERE example_id = $id;
                """;
            command.Parameters.AddWithValue("$language", example.LExampleLanguage);
            LStateColumn.LStateColumnApply(command, "text", example.LExampleText);
            LStateColumn.LStateColumnApply(command, "reference", example.LExampleSource);
            command.Parameters.AddWithValue("$id", example.LExampleId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Example carries the id '{example.LExampleId}'.");
            }
        }

        LGlossArchive.LGlossExampleSave(connection, example.LExampleId, example.LExampleGloss);
        LMentionArchive.LMentionExampleSave(connection, example.LExampleId, example.LExampleMention);

        session.LDatabaseSessionCommit();
    }

    public void LExampleTextUpdate(long exampleId, LStateValue text)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);
        ArgumentNullException.ThrowIfNull(text);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "UPDATE example SET text_state = $textState, text = $text WHERE example_id = $id;";
            LStateColumn.LStateColumnApply(command, "text", text);
            command.Parameters.AddWithValue("$id", exampleId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Example carries the id '{exampleId}'.");
            }
        }

        LMentionArchive.LMentionExampleSweep(connection, exampleId);

        session.LDatabaseSessionCommit();
    }

    public void LExampleSourceUpdate(long exampleId, LStateAnchor source)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);
        ArgumentNullException.ThrowIfNull(source);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE example SET reference_state = $referenceState, reference_ref = $reference WHERE example_id = $id;";
            LStateColumn.LStateColumnApply(command, "reference", source);
            command.Parameters.AddWithValue("$id", exampleId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Example carries the id '{exampleId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public int LExampleReferenceRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        return LExampleReferenceRead(session.LDatabaseSessionConnection, id);
    }

    private static int LExampleReferenceRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM sense_example WHERE example_ref = $id)
                + (SELECT COUNT(*) FROM collocation_example WHERE example_ref = $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public IReadOnlyDictionary<long, int> LExampleReferenceRead()
    {
        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT example_ref, COUNT(*) FROM (
                SELECT example_ref FROM sense_example WHERE example_ref IS NOT NULL
                UNION ALL
                SELECT example_ref FROM collocation_example WHERE example_ref IS NOT NULL
            )
            GROUP BY example_ref;
            """;

        Dictionary<long, int> counts = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            counts[reader.GetInt64(0)] = reader.GetInt32(1);
        }

        return counts;
    }

    public void LExampleDelete(long id)
    {
        LExampleDelete(id, false);
    }

    public void LExampleDelete(long id, bool detach)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        if (detach)
        {
            LExampleLink.LExampleLinkClear(connection, id);
        }

        int references = LExampleReferenceRead(connection, id);
        if (references > 0)
        {
            throw new InvalidOperationException(
                $"Example {id} is still referenced {references} time(s); detach every reference before deleting it.");
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM example WHERE example_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    internal static LExample? LExampleSingleRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT language, text_state, text, reference_state, reference_ref
            FROM example WHERE example_id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);
        LExample? example;
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            if (!reader.Read())
            {
                return null;
            }

            example = new LExample(
                id,
                reader.GetString(0),
                LStateColumn.LStateColumnRead(reader, 1),
                LStateColumn.LStateColumnResolve(reader, 3));
        }

        return LExampleListLoad(connection, [example])[0];
    }

    internal static IReadOnlyList<LExample> LExampleListLoad(
        SqliteConnection connection, IReadOnlyList<LExample> examples)
    {
        if (examples.Count == 0)
        {
            return examples;
        }

        List<long> ids = examples.Select(static example => example.LExampleId).ToList();
        IReadOnlyDictionary<long, IReadOnlyList<LGloss>> glosses =
            LGlossArchive.LGlossExampleRead(connection, ids);
        IReadOnlyDictionary<long, IReadOnlyList<LMention>> mentions =
            LMentionArchive.LMentionExampleRead(connection, ids);

        List<LExample> filled = new(examples.Count);
        foreach (LExample example in examples)
        {
            filled.Add(example with
            {
                LExampleGloss = glosses.TryGetValue(example.LExampleId, out IReadOnlyList<LGloss>? gloss)
                    ? gloss
                    : [],
                LExampleMention = mentions.TryGetValue(example.LExampleId, out IReadOnlyList<LMention>? mention)
                    ? mention
                    : [],
            });
        }

        return filled;
    }
}
