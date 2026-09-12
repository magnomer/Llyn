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

        LExample stored = example;

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO example (
                    language, text_state, text, translation_state, translation,
                    source_state, source_ref)
                VALUES (
                    $language, $textState, $text, $translationState, $translation,
                    $sourceState, $source)
                RETURNING example_id;
                """;
            command.Parameters.AddWithValue("$language", stored.LExampleLanguage);
            LStateColumn.LStateColumnApply(command, "text", stored.LExampleText);
            LStateColumn.LStateColumnApply(command, "translation", stored.LExampleTranslation);
            LStateColumn.LStateColumnApply(command, "source", stored.LExampleSource);
            stored = stored with { LExampleId = (long)command.ExecuteScalar()! };
        }

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
            SELECT example_id, language, text_state, text, translation_state, translation,
                   source_state, source_ref
            FROM example
            ORDER BY rowid;
            """;

        List<LExample> examples = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            examples.Add(new LExample(
                reader.GetInt64(0),
                reader.GetString(1),
                LStateColumn.LStateColumnRead(reader, 2),
                LStateColumn.LStateColumnRead(reader, 4),
                LStateColumn.LStateColumnResolve(reader, 6)));
        }

        return examples;
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
                    translation_state = $translationState, translation = $translation,
                    source_state = $sourceState, source_ref = $source
                WHERE example_id = $id;
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

    public void LExampleTextUpdate(long exampleId, LStateValue text)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);
        ArgumentNullException.ThrowIfNull(text);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
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
                "UPDATE example SET source_state = $sourceState, source_ref = $source WHERE example_id = $id;";
            LStateColumn.LStateColumnApply(command, "source", source);
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
            SELECT language, text_state, text, translation_state, translation,
                   source_state, source_ref
            FROM example WHERE example_id = $id;
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
            LStateColumn.LStateColumnResolve(reader, 5));
    }
}
