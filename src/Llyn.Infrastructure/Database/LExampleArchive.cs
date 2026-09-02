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
            LExampleTranslations = LExampleTranslationPrepare(example.LExampleTranslations),
        };

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO example (id, language, text_state, text, local, source_state, source_id)
                VALUES ($id, $language, $textState, $text, $local, $sourceState, $source);
                """;
            command.Parameters.AddWithValue("$id", stored.LExampleId);
            command.Parameters.AddWithValue("$language", stored.LExampleLanguage);
            LStateColumn.LStateColumnApply(command, "text", stored.LExampleText);
            command.Parameters.AddWithValue("$local", (object?)stored.LExampleLocal ?? DBNull.Value);
            LStateColumn.LStateColumnApply(command, "source", stored.LExampleSource);
            command.ExecuteNonQuery();
        }

        LExampleTranslationInsert(connection, stored.LExampleId, stored.LExampleTranslations);
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

        IReadOnlyDictionary<string, IReadOnlyList<LTranslation>> translations =
            LExampleTranslationRead(connection);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT id, language, text_state, text, local, source_state, source_id
            FROM example
            ORDER BY rowid;
            """;

        List<LExample> examples = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            string id = reader.GetString(0);
            examples.Add(new LExample(
                id,
                reader.GetString(1),
                LStateColumn.LStateColumnRead(reader, 2),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                LStateColumn.LStateColumnRead(reader, 5),
                translations.TryGetValue(id, out IReadOnlyList<LTranslation>? found) ? found : []));
        }

        return examples;
    }

    public void LExampleUpdate(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        ArgumentException.ThrowIfNullOrWhiteSpace(example.LExampleId);

        IReadOnlyList<LTranslation> translations = LExampleTranslationPrepare(example.LExampleTranslations);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE example
                SET language = $language, text_state = $textState, text = $text, local = $local,
                    source_state = $sourceState, source_id = $source
                WHERE id = $id;
                """;
            command.Parameters.AddWithValue("$language", example.LExampleLanguage);
            LStateColumn.LStateColumnApply(command, "text", example.LExampleText);
            command.Parameters.AddWithValue("$local", (object?)example.LExampleLocal ?? DBNull.Value);
            LStateColumn.LStateColumnApply(command, "source", example.LExampleSource);
            command.Parameters.AddWithValue("$id", example.LExampleId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Example carries the id '{example.LExampleId}'.");
            }
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM example_translation WHERE example_id = $example;";
            command.Parameters.AddWithValue("$example", example.LExampleId);
            command.ExecuteNonQuery();
        }

        LExampleTranslationInsert(connection, example.LExampleId, translations);
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
                SELECT example_id FROM sense_example
                UNION ALL
                SELECT example_id FROM collocation_example
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
        string language;
        LStateValue text;
        string? local;
        LStateValue source;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                SELECT language, text_state, text, local, source_state, source_id
                FROM example WHERE id = $id;
                """;
            command.Parameters.AddWithValue("$id", id);
            using SqliteDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            language = reader.GetString(0);
            text = LStateColumn.LStateColumnRead(reader, 1);
            local = reader.IsDBNull(3) ? null : reader.GetString(3);
            source = LStateColumn.LStateColumnRead(reader, 4);
        }

        return new LExample(id, language, text, local, source, LExampleTranslationRead(connection, id));
    }

    internal static IReadOnlyDictionary<string, IReadOnlyList<LTranslation>> LExampleTranslationRead(
        SqliteConnection connection, string table, string column, string referrerId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            SELECT translation.id, translation.example_id, translation.language,
                   translation.text, translation.position
            FROM example_translation translation
            JOIN {table} link ON link.example_id = translation.example_id
            WHERE link.{column} = $referrer
            ORDER BY translation.example_id, translation.position;
            """;
        command.Parameters.AddWithValue("$referrer", referrerId);

        Dictionary<string, IReadOnlyList<LTranslation>> grouped = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            string exampleId = reader.GetString(1);
            if (grouped.TryGetValue(exampleId, out IReadOnlyList<LTranslation>? existing) is false)
            {
                existing = new List<LTranslation>();
                grouped[exampleId] = existing;
            }

            ((List<LTranslation>)existing).Add(new LTranslation(
                reader.GetString(0),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetInt32(4)));
        }

        return grouped;
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<LTranslation>> LExampleTranslationRead(
        SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT id, example_id, language, text, position
            FROM example_translation
            ORDER BY example_id, position;
            """;

        Dictionary<string, IReadOnlyList<LTranslation>> grouped = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            string exampleId = reader.GetString(1);
            if (grouped.TryGetValue(exampleId, out IReadOnlyList<LTranslation>? existing) is false)
            {
                existing = new List<LTranslation>();
                grouped[exampleId] = existing;
            }

            ((List<LTranslation>)existing).Add(new LTranslation(
                reader.GetString(0),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetInt32(4)));
        }

        return grouped;
    }

    private static IReadOnlyList<LTranslation> LExampleTranslationPrepare(
        IReadOnlyList<LTranslation> translations)
    {
        List<LTranslation> identified = [];
        for (int position = 0; position < translations.Count; position++)
        {
            LTranslation translation = translations[position];
            identified.Add(translation with
            {
                LTranslationId = string.IsNullOrWhiteSpace(translation.LTranslationId)
                    ? LIdentity.LIdentityCreate()
                    : translation.LTranslationId,
                LTranslationPosition = position,
            });
        }

        return identified;
    }

    private static IReadOnlyList<LTranslation> LExampleTranslationRead(
        SqliteConnection connection, string exampleId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT id, language, text, position
            FROM example_translation WHERE example_id = $example
            ORDER BY position;
            """;
        command.Parameters.AddWithValue("$example", exampleId);

        List<LTranslation> translations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            translations.Add(new LTranslation(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetInt32(3)));
        }

        return translations;
    }

    private static void LExampleTranslationInsert(
        SqliteConnection connection,
        string exampleId,
        IReadOnlyList<LTranslation> translations)
    {
        for (int position = 0; position < translations.Count; position++)
        {
            LTranslation translation = translations[position];
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO example_translation (id, example_id, language, text, position)
                VALUES ($id, $example, $language, $text, $position);
                """;
            command.Parameters.AddWithValue("$id", translation.LTranslationId);
            command.Parameters.AddWithValue("$example", exampleId);
            command.Parameters.AddWithValue("$language", translation.LTranslationLanguage);
            command.Parameters.AddWithValue("$text", translation.LTranslationText);
            command.Parameters.AddWithValue("$position", position);
            command.ExecuteNonQuery();
        }
    }
}
