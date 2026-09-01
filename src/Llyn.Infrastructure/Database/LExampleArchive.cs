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
/// </summary>
public sealed class LExampleArchive
{
    private readonly LDatabase _lExampleArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
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

        using SqliteConnection connection = _lExampleArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.Transaction = transaction;
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

        LExampleTranslationInsert(connection, transaction, stored.LExampleId, stored.LExampleTranslations);
        transaction.Commit();

        return stored;
    }

    /// <summary>
    /// Reads the Example identified by <paramref name="id"/> with its translations in order, or
    /// <c>null</c> when no such Example exists.
    /// </summary>
    public LExample? LExampleRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lExampleArchiveDatabase.LDatabaseRead();
        return LExampleSingleRead(connection, id);
    }

    /// <summary>Reads the Examples an Entry references, in the order that Entry gives them.</summary>
    public IReadOnlyList<LExample> LExampleEntryRead(string entryId)
    {
        return LExampleReferrerRead("entry_example", "entry_id", entryId);
    }

    /// <summary>Reads the Examples a Meaning references, in the order that Meaning gives them.</summary>
    public IReadOnlyList<LExample> LExampleSenseRead(string senseId)
    {
        return LExampleReferrerRead("sense_example", "sense_id", senseId);
    }

    /// <summary>Reads the Examples a Collocation references, in the order that Collocation gives them.</summary>
    public IReadOnlyList<LExample> LExampleCollocationRead(string collocationId)
    {
        return LExampleReferrerRead("collocation_example", "collocation_id", collocationId);
    }

    /// <summary>
    /// Rewrites the Example identified by <paramref name="example"/>'s id: its language, text, local
    /// rendering, and Source reference, plus its translations, which are replaced wholesale with fresh
    /// ids. The Example's own id and every reference pointing at it are untouched, so an update never
    /// changes where the Example appears or in what order.
    /// </summary>
    public void LExampleUpdate(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        ArgumentException.ThrowIfNullOrWhiteSpace(example.LExampleId);

        IReadOnlyList<LTranslation> translations = LExampleTranslationPrepare(example.LExampleTranslations);

        using SqliteConnection connection = _lExampleArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.Transaction = transaction;
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
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "DELETE FROM example_translation WHERE example_id = $example;";
            command.Parameters.AddWithValue("$example", example.LExampleId);
            command.ExecuteNonQuery();
        }

        LExampleTranslationInsert(connection, transaction, example.LExampleId, translations);
        transaction.Commit();
    }

    /// <summary>
    /// Sets or clears the single Source the Example identified by <paramref name="exampleId"/> cites —
    /// pass <c>null</c> for <paramref name="sourceId"/> to clear it. Only the reference moves: the Source
    /// row itself is never created, changed, or removed here.
    /// </summary>
    public void LExampleSourceUpdate(string exampleId, string? sourceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using SqliteConnection connection = _lExampleArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE example SET source_id = $source WHERE id = $id;";
        command.Parameters.AddWithValue("$source", (object?)sourceId ?? DBNull.Value);
        command.Parameters.AddWithValue("$id", exampleId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Deletes the Example identified by <paramref name="id"/> together with its translations. Guarded:
    /// while any Entry, Meaning, or Collocation still references the Example, nothing is deleted and an
    /// <see cref="InvalidOperationException"/> is thrown — detach every reference first. A Source the
    /// Example cited is left standing; only the reference to it disappears with the row.
    /// </summary>
    public void LExampleDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lExampleArchiveDatabase.LDatabaseRead();

        using (SqliteCommand guard = connection.CreateCommand())
        {
            guard.CommandText =
                """
                SELECT
                    (SELECT COUNT(*) FROM entry_example WHERE example_id = $id)
                    + (SELECT COUNT(*) FROM sense_example WHERE example_id = $id)
                    + (SELECT COUNT(*) FROM collocation_example WHERE example_id = $id);
                """;
            guard.Parameters.AddWithValue("$id", id);
            long references = Convert.ToInt64(guard.ExecuteScalar());
            if (references > 0)
            {
                throw new InvalidOperationException(
                    $"Example {id} is still referenced {references} time(s); detach every reference before deleting it.");
            }
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM example WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    /// <summary>References an existing Example from an Entry at <paramref name="position"/> in that Entry's order.</summary>
    public void LExampleEntryAttach(string entryId, string exampleId, int position)
    {
        LExampleReferenceAttach("entry_example", "entry_id", entryId, exampleId, position);
    }

    /// <summary>References an existing Example from a Meaning at <paramref name="position"/> in that Meaning's order.</summary>
    public void LExampleSenseAttach(string senseId, string exampleId, int position)
    {
        LExampleReferenceAttach("sense_example", "sense_id", senseId, exampleId, position);
    }

    /// <summary>References an existing Example from a Collocation at <paramref name="position"/> in that Collocation's order.</summary>
    public void LExampleCollocationAttach(string collocationId, string exampleId, int position)
    {
        LExampleReferenceAttach("collocation_example", "collocation_id", collocationId, exampleId, position);
    }

    /// <summary>Removes an Entry's reference to an Example. The Example and its other references survive.</summary>
    public void LExampleEntryDetach(string entryId, string exampleId)
    {
        LExampleReferenceDetach("entry_example", "entry_id", entryId, exampleId);
    }

    /// <summary>Removes a Meaning's reference to an Example. The Example and its other references survive.</summary>
    public void LExampleSenseDetach(string senseId, string exampleId)
    {
        LExampleReferenceDetach("sense_example", "sense_id", senseId, exampleId);
    }

    /// <summary>Removes a Collocation's reference to an Example. The Example and its other references survive.</summary>
    public void LExampleCollocationDetach(string collocationId, string exampleId)
    {
        LExampleReferenceDetach("collocation_example", "collocation_id", collocationId, exampleId);
    }

    // The three association tables differ only in their name and their referrer column, so the reference
    // operations share one implementation each. Both identifiers are store-owned literals chosen by the
    // methods above, never caller input, so composing them into the statement text opens no injection
    // seam; every value still travels as a parameter.
    private void LExampleReferenceAttach(string table, string column, string referrerId, string exampleId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using SqliteConnection connection = _lExampleArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"INSERT INTO {table} ({column}, example_id, position) VALUES ($referrer, $example, $position);";
        command.Parameters.AddWithValue("$referrer", referrerId);
        command.Parameters.AddWithValue("$example", exampleId);
        command.Parameters.AddWithValue("$position", position);
        command.ExecuteNonQuery();
    }

    private void LExampleReferenceDetach(string table, string column, string referrerId, string exampleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using SqliteConnection connection = _lExampleArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {table} WHERE {column} = $referrer AND example_id = $example;";
        command.Parameters.AddWithValue("$referrer", referrerId);
        command.Parameters.AddWithValue("$example", exampleId);
        command.ExecuteNonQuery();
    }

    private IReadOnlyList<LExample> LExampleReferrerRead(string table, string column, string referrerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);

        using SqliteConnection connection = _lExampleArchiveDatabase.LDatabaseRead();

        List<string> ids = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"SELECT example_id FROM {table} WHERE {column} = $referrer ORDER BY position;";
            command.Parameters.AddWithValue("$referrer", referrerId);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ids.Add(reader.GetString(0));
            }
        }

        List<LExample> examples = [];
        foreach (string id in ids)
        {
            LExample? example = LExampleSingleRead(connection, id);
            if (example is not null)
            {
                examples.Add(example);
            }
        }

        return examples;
    }

    private static LExample? LExampleSingleRead(SqliteConnection connection, string id)
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

    private static IReadOnlyList<LTranslation> LExampleTranslationPrepare(IReadOnlyList<LTranslation> translations)
    {
        List<LTranslation> identified = [];
        foreach (LTranslation translation in translations)
        {
            identified.Add(translation with { LTranslationId = LIdentity.LIdentityCreate() });
        }

        return identified;
    }

    private static IReadOnlyList<LTranslation> LExampleTranslationRead(SqliteConnection connection, string exampleId)
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
        SqliteTransaction transaction,
        string exampleId,
        IReadOnlyList<LTranslation> translations)
    {
        foreach (LTranslation translation in translations)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText =
                """
                INSERT INTO example_translation (id, example_id, language, text, position)
                VALUES ($id, $example, $language, $text, $position);
                """;
            command.Parameters.AddWithValue("$id", translation.LTranslationId);
            command.Parameters.AddWithValue("$example", exampleId);
            command.Parameters.AddWithValue("$language", translation.LTranslationLanguage);
            command.Parameters.AddWithValue("$text", translation.LTranslationText);
            command.Parameters.AddWithValue("$position", translation.LTranslationPosition);
            command.ExecuteNonQuery();
        }
    }
}
