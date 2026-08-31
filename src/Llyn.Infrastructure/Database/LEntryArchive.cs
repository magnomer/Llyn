using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists entries and their two owned child structures — written forms and parts of speech — in the
/// workspace database. An entry's id and timestamps are assigned here on creation; its forms and POS
/// are written as ordered child rows, so reordering rewrites <c>position</c> only and never touches
/// the entry id. Deleting an entry removes its forms and POS through the foreign-key cascade.
/// </summary>
public sealed class LEntryArchive
{
    private readonly LDatabase _lEntryArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LEntryArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lEntryArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="entry"/> with a fresh opaque id and creation/modification timestamps,
    /// writing <paramref name="forms"/> and <paramref name="speeches"/> as ordered child rows (their
    /// entry id and position are assigned from list order). Returns the stored entry with its id and
    /// timestamps filled in. The whole write is one transaction.
    /// </summary>
    public LEntry LEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(forms);
        ArgumentNullException.ThrowIfNull(speeches);

        string id = LIdentity.LIdentityCreate();
        string now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        LEntry stored = entry with
        {
            LEntryId = id,
            LEntryAddedUtc = now,
            LEntryUpdatedUtc = now,
        };

        using SqliteConnection connection = _lEntryArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO entry (id, headword, language, proficiency, frequency, added_utc, updated_utc)
                VALUES ($id, $headword, $language, $proficiency, $frequency, $added, $updated);
                """;
            command.Parameters.AddWithValue("$id", stored.LEntryId);
            command.Parameters.AddWithValue("$headword", stored.LEntryHeadword);
            command.Parameters.AddWithValue("$language", stored.LEntryLanguage);
            command.Parameters.AddWithValue("$proficiency", (object?)stored.LEntryProficiency ?? DBNull.Value);
            command.Parameters.AddWithValue("$frequency", (object?)stored.LEntryFrequency ?? DBNull.Value);
            command.Parameters.AddWithValue("$added", (object?)stored.LEntryAddedUtc ?? DBNull.Value);
            command.Parameters.AddWithValue("$updated", (object?)stored.LEntryUpdatedUtc ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        LEntryFormInsert(connection, id, forms);
        LEntrySpeechInsert(connection, id, speeches);

        transaction.Commit();
        return stored;
    }

    /// <summary>Reads the entry row for <paramref name="id"/>, or <c>null</c> when no entry has that id.</summary>
    public LEntry? LEntryRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lEntryArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT id, headword, language, proficiency, frequency, added_utc, updated_utc
            FROM entry WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LEntry(
            reader.GetString(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.IsDBNull(3) ? null : reader.GetString(3),
            reader.IsDBNull(4) ? null : reader.GetString(4),
            reader.IsDBNull(5) ? null : reader.GetString(5),
            reader.IsDBNull(6) ? null : reader.GetString(6));
    }

    /// <summary>Reads the entry's written forms, ordered by position.</summary>
    public IReadOnlyList<LForm> LEntryFormRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lEntryArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT entry_id, position, text, local, role
            FROM form WHERE entry_id = $id ORDER BY position;
            """;
        command.Parameters.AddWithValue("$id", id);

        List<LForm> forms = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            forms.Add(new LForm(
                reader.GetString(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.GetString(4)));
        }

        return forms;
    }

    /// <summary>Reads the entry's part-of-speech assignments, ordered by position.</summary>
    public IReadOnlyList<LSpeech> LEntrySpeechRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lEntryArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT entry_id, position, value_id
            FROM part_of_speech WHERE entry_id = $id ORDER BY position;
            """;
        command.Parameters.AddWithValue("$id", id);

        List<LSpeech> speeches = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            speeches.Add(new LSpeech(
                reader.GetString(0),
                reader.GetInt32(1),
                reader.GetString(2)));
        }

        return speeches;
    }

    /// <summary>
    /// Updates the headword and metadata of the entry identified by <paramref name="entry"/>'s id and
    /// stamps a fresh modification timestamp. The id, forms, and POS are untouched.
    /// </summary>
    public void LEntryUpdate(LEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentException.ThrowIfNullOrWhiteSpace(entry.LEntryId);

        string now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

        using SqliteConnection connection = _lEntryArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE entry
            SET headword = $headword, language = $language, proficiency = $proficiency,
                frequency = $frequency, updated_utc = $updated
            WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$headword", entry.LEntryHeadword);
        command.Parameters.AddWithValue("$language", entry.LEntryLanguage);
        command.Parameters.AddWithValue("$proficiency", (object?)entry.LEntryProficiency ?? DBNull.Value);
        command.Parameters.AddWithValue("$frequency", (object?)entry.LEntryFrequency ?? DBNull.Value);
        command.Parameters.AddWithValue("$updated", now);
        command.Parameters.AddWithValue("$id", entry.LEntryId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Replaces the entry's forms with <paramref name="forms"/> in list order: existing form rows are
    /// cleared and the new set written, so reordering rewrites positions while the entry id stays fixed.
    /// </summary>
    public void LEntryFormSet(string id, IReadOnlyList<LForm> forms)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(forms);

        using SqliteConnection connection = _lEntryArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();
        LEntryChildClear(connection, "form", id);
        LEntryFormInsert(connection, id, forms);
        transaction.Commit();
    }

    /// <summary>
    /// Replaces the entry's part-of-speech assignments with <paramref name="speeches"/> in list order,
    /// clearing the existing rows first. Reordering rewrites positions; the entry id stays fixed.
    /// </summary>
    public void LEntrySpeechSet(string id, IReadOnlyList<LSpeech> speeches)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(speeches);

        using SqliteConnection connection = _lEntryArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();
        LEntryChildClear(connection, "part_of_speech", id);
        LEntrySpeechInsert(connection, id, speeches);
        transaction.Commit();
    }

    /// <summary>
    /// Deletes the entry identified by <paramref name="id"/>. Its forms and POS rows are removed by the
    /// foreign-key cascade.
    /// </summary>
    public void LEntryDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lEntryArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM entry WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    private static void LEntryFormInsert(SqliteConnection connection, string id, IReadOnlyList<LForm> forms)
    {
        for (int position = 0; position < forms.Count; position++)
        {
            LForm form = forms[position];
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO form (entry_id, position, text, local, role)
                VALUES ($entry, $position, $text, $local, $role);
                """;
            command.Parameters.AddWithValue("$entry", id);
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$text", form.LFormText);
            command.Parameters.AddWithValue("$local", (object?)form.LFormLocal ?? DBNull.Value);
            command.Parameters.AddWithValue("$role", form.LFormRole);
            command.ExecuteNonQuery();
        }
    }

    private static void LEntrySpeechInsert(SqliteConnection connection, string id, IReadOnlyList<LSpeech> speeches)
    {
        for (int position = 0; position < speeches.Count; position++)
        {
            LSpeech speech = speeches[position];
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO part_of_speech (entry_id, position, value_id)
                VALUES ($entry, $position, $value);
                """;
            command.Parameters.AddWithValue("$entry", id);
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$value", speech.LSpeechValueId);
            command.ExecuteNonQuery();
        }
    }

    private static void LEntryChildClear(SqliteConnection connection, string table, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {table} WHERE entry_id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}
