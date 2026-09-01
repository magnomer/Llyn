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

        string id = LIdentity.LIdentityCreate();
        LExample stored = example with
        {
            LExampleId = id,
            LExampleTranslations = LExampleTranslationPrepare(example.LExampleTranslations),
        };

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO example (id, language, text, local, source_id)
                VALUES ($id, $language, $text, $local, $source);
                """;
            command.Parameters.AddWithValue("$id", stored.LExampleId);
            command.Parameters.AddWithValue("$language", stored.LExampleLanguage);
            command.Parameters.AddWithValue("$text", stored.LExampleText);
            command.Parameters.AddWithValue("$local", (object?)stored.LExampleLocal ?? DBNull.Value);
            command.Parameters.AddWithValue("$source", (object?)stored.LExampleSourceId ?? DBNull.Value);
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
                SET language = $language, text = $text, local = $local, source_id = $source
                WHERE id = $id;
                """;
            command.Parameters.AddWithValue("$language", example.LExampleLanguage);
            command.Parameters.AddWithValue("$text", example.LExampleText);
            command.Parameters.AddWithValue("$local", (object?)example.LExampleLocal ?? DBNull.Value);
            command.Parameters.AddWithValue("$source", (object?)example.LExampleSourceId ?? DBNull.Value);
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

    public void LExampleSourceUpdate(string exampleId, string? sourceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "UPDATE example SET source_id = $source WHERE id = $id;";
            command.Parameters.AddWithValue("$source", (object?)sourceId ?? DBNull.Value);
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

    public void LExampleDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

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
        string text;
        string? local;
        string? source;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT language, text, local, source_id FROM example WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            using SqliteDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            language = reader.GetString(0);
            text = reader.GetString(1);
            local = reader.IsDBNull(2) ? null : reader.GetString(2);
            source = reader.IsDBNull(3) ? null : reader.GetString(3);
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

    // A translation keeps the id it arrives with, so an id a caller already holds stays valid across an
    // update; only one that has never been stored is given a fresh id. The position is not taken from
    // the record at all — it is the index the caller put the translation at, assigned on insert, which
    // is how every other ordered child in the schema works.
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
