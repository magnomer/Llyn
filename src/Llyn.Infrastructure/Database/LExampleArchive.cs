using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists Examples — the first entity no Entry, Meaning, or Collocation owns. An Example is created
/// once with an opaque id, then <em>referenced</em> by any number of referrers through the association
/// tables, each carrying the position the Example takes for that referrer alone. Attaching and detaching
/// therefore only ever write association rows: detaching leaves the Example and its other references
/// untouched, and <see cref="LExampleDelete"/> refuses to run while any reference remains. Translations
/// are owned text, rewritten wholesale on update and removed with the Example; the single Source an
/// Example cites is a reference only and is never created, updated, or deleted from here.
/// <para>
/// The association tables live in <see cref="LExampleLink"/>, which owns attaching, detaching, and
/// reading an Example set by referrer; this file owns the Example itself.
/// </para>
/// </summary>
public sealed class LExampleArchive
{
    private readonly LDatabase _lExampleArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens sessions through.</summary>
    public LExampleArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lExampleArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="example"/> and its translations with fresh opaque ids and returns the
    /// stored Example with those ids filled in. The new Example is referenced by nothing until it is
    /// attached to a referrer.
    /// </summary>
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

    /// <summary>
    /// Reads the Example identified by <paramref name="id"/> with its translations in order, or
    /// <c>null</c> when no such Example exists.
    /// </summary>
    public LExample? LExampleRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        return LExampleSingleRead(session.LDatabaseSessionConnection, id);
    }

    /// <summary>
    /// Rewrites the Example identified by <paramref name="example"/>'s id: its language, text, local
    /// rendering, and Source reference, plus its translations, which are replaced wholesale. A
    /// translation that arrives with an id keeps it, so an id a caller is holding stays valid; only a
    /// translation without one is given a fresh id. The Example's own id and every reference pointing at
    /// it are untouched, so an update never changes where the Example appears or in what order. Throws
    /// when no Example carries that id.
    /// </summary>
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

    /// <summary>
    /// Sets or clears the single Source the Example identified by <paramref name="exampleId"/> cites —
    /// pass <c>null</c> for <paramref name="sourceId"/> to clear it. Only the reference moves: the Source
    /// row itself is never created, changed, or removed here. Throws when no Example carries that id.
    /// </summary>
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

    /// <summary>
    /// Counts the references that still point at the Example identified by <paramref name="id"/> — the
    /// number <see cref="LExampleDelete"/> refuses a delete over. A caller that has just detached one
    /// reference reads this to learn whether the row it detached from was the last one, without a store
    /// of its own having to know which association tables exist.
    /// </summary>
    public int LExampleReferenceRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lExampleArchiveDatabase.LDatabaseSessionStart();
        return LExampleReferenceRead(session.LDatabaseSessionConnection, id);
    }

    // The same count on a connection the caller already holds, so a guard and the delete it guards run
    // in one transaction and nothing can attach the row between them.
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

    /// <summary>
    /// Deletes the Example identified by <paramref name="id"/> together with its translations. Guarded:
    /// while any Entry, Meaning, or Collocation still references the Example, nothing is deleted and an
    /// <see cref="InvalidOperationException"/> is thrown — detach every reference first. A Source the
    /// Example cited is left standing; only the reference to it disappears with the row. The guard and
    /// the delete share one transaction, so nothing can attach the Example between them.
    /// </summary>
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

    /// <summary>
    /// Reads one Example with its translations on a connection the caller already holds. Shared with
    /// <see cref="LExampleLink"/>, which resolves a whole referrer's set at once.
    /// </summary>
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

    /// <summary>
    /// Reads the translations of every Example a referrer names, in one query, grouped by Example id.
    /// Reading them one Example at a time is what turns a referrer's set into a round-trip per row.
    /// </summary>
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
