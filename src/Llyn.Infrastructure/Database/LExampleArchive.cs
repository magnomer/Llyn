using System;
using System.Collections.Generic;
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
        ArgumentException.ThrowIfNullOrWhiteSpace(example.LExampleLanguage);

        LExample stored = example with
        {
            LExampleId = string.IsNullOrWhiteSpace(example.LExampleId)
                ? LIdentity.LIdentityCreate()
                : example.LExampleId,
        };

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO example (
                    id, language, text_state, text, translation_state, translation,
                    source_state, source_id)
                VALUES (
                    $id, $language, $textState, $text, $translationState, $translation,
                    $sourceState, $source);
                """;
            command.Parameters.AddWithValue("$id", stored.LExampleId);
            command.Parameters.AddWithValue("$language", stored.LExampleLanguage);
            LStateColumn.LStateColumnApply(command, "text", stored.LExampleText);
            LStateColumn.LStateColumnApply(command, "translation", stored.LExampleTranslation);
            LStateColumn.LStateColumnApply(command, "source", stored.LExampleSource);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();

        return stored;
    }

    public LExample? LExampleRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

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
            SELECT id, language, text_state, text, translation_state, translation,
                   source_state, source_id
            FROM example
            ORDER BY rowid;
            """;

        List<LExample> examples = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            examples.Add(new LExample(
                reader.GetString(0),
                reader.GetString(1),
                LStateColumn.LStateColumnRead(reader, 2),
                LStateColumn.LStateColumnRead(reader, 4),
                LStateColumn.LStateColumnRead(reader, 6)));
        }

        return examples;
    }

    public void LExampleUpdate(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        ArgumentException.ThrowIfNullOrWhiteSpace(example.LExampleId);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE example
                SET language = $language, text_state = $textState, text = $text,
                    translation_state = $translationState, translation = $translation,
                    source_state = $sourceState, source_id = $source
                WHERE id = $id;
                """;
            command.Parameters.AddWithValue("$language", example.LExampleLanguage);
            LStateColumn.LStateColumnApply(command, "text", example.LExampleText);
            LStateColumn.LStateColumnApply(command, "translation", example.LExampleTranslation);
            LStateColumn.LStateColumnApply(command, "source", example.LExampleSource);
            command.Parameters.AddWithValue("$id", example.LExampleId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Example carries the id '{example.LExampleId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LExampleTextUpdate(string exampleId, LStateValue text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);
        ArgumentNullException.ThrowIfNull(text);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE example SET text_state = $textState, text = $text WHERE id = $id;";
            LStateColumn.LStateColumnApply(command, "text", text);
            command.Parameters.AddWithValue("$id", exampleId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Example carries the id '{exampleId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LExampleSourceUpdate(string exampleId, LStateValue source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);
        ArgumentNullException.ThrowIfNull(source);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE example SET source_state = $sourceState, source_id = $source WHERE id = $id;";
            LStateColumn.LStateColumnApply(command, "source", source);
            command.Parameters.AddWithValue("$id", exampleId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Example carries the id '{exampleId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public int LExampleReferenceRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        return LExampleReferenceRead(session.LDatabaseSessionConnection, id);
    }

    private static int LExampleReferenceRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM entry_example WHERE example_id = $id)
                + (SELECT COUNT(*) FROM sense_example WHERE example_id = $id)
                + (SELECT COUNT(*) FROM collocation_example WHERE example_id = $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public IReadOnlyDictionary<string, int> LExampleReferenceRead()
    {
        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT example_id, COUNT(*) FROM (
                SELECT example_id FROM entry_example
                UNION ALL
                SELECT example_id FROM sense_example WHERE example_id IS NOT NULL
                UNION ALL
                SELECT example_id FROM collocation_example WHERE example_id IS NOT NULL
            )
            GROUP BY example_id;
            """;

        Dictionary<string, int> counts = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            counts[reader.GetString(0)] = reader.GetInt32(1);
        }

        return counts;
    }

    public void LExampleDelete(string id)
    {
        LExampleDelete(id, false);
    }

    public void LExampleDelete(string id, bool detach)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

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
            command.CommandText = "DELETE FROM example WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    internal static LExample? LExampleSingleRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT language, text_state, text, translation_state, translation,
                   source_state, source_id
            FROM example WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LExample(
            id,
            reader.GetString(0),
            LStateColumn.LStateColumnRead(reader, 1),
            LStateColumn.LStateColumnRead(reader, 3),
            LStateColumn.LStateColumnRead(reader, 5));
    }
}
