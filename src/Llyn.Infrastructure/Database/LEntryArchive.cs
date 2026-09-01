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
/// <para>
/// Every method runs inside a <see cref="LDatabaseSession"/>, so a caller that opens one of its own
/// around several stores gets one transaction across all of them.
/// </para>
/// </summary>
public sealed class LEntryArchive
{
    private readonly LDatabase _lEntryArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens sessions through.</summary>
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

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

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

        session.LDatabaseSessionCommit();
        return stored;
    }

    /// <summary>Reads the entry row for <paramref name="id"/>, or <c>null</c> when no entry has that id.</summary>
    public LEntry? LEntryRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
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

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
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

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
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
    /// stamps a fresh modification timestamp. The id, forms, and POS are untouched. Throws when no
    /// entry carries that id, rather than reporting success for a write that reached nothing.
    /// </summary>
    public void LEntryUpdate(LEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentException.ThrowIfNullOrWhiteSpace(entry.LEntryId);

        string now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
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
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No entry carries the id '{entry.LEntryId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Replaces the entry's forms with <paramref name="forms"/> in list order: existing form rows are
    /// cleared and the new set written, so reordering rewrites positions while the entry id stays fixed.
    /// </summary>
    public void LEntryFormSet(string id, IReadOnlyList<LForm> forms)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(forms);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        LEntryChildClear(connection, "form", id);
        LEntryFormInsert(connection, id, forms);
        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Replaces the entry's part-of-speech assignments with <paramref name="speeches"/> in list order,
    /// clearing the existing rows first. Reordering rewrites positions; the entry id stays fixed.
    /// </summary>
    public void LEntrySpeechSet(string id, IReadOnlyList<LSpeech> speeches)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(speeches);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        LEntryChildClear(connection, "part_of_speech", id);
        LEntrySpeechInsert(connection, id, speeches);
        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Deletes the entry identified by <paramref name="id"/> and everything it owns. The foreign-key
    /// cascade carries away its forms and parts of speech, its inflections and their features, its
    /// meanings (each with its inline definition field), the relations originating from those meanings,
    /// its single pronunciation with its syllables and representations, its collocations, its single
    /// note, and every association row hanging from the entry, its meanings, or its collocations. The
    /// independent Examples, Tags, Situations, References, and Authors those associations pointed at
    /// are left standing — only the rows linking them to this entry disappear.
    /// <para>
    /// Guarded against the one thing a cascade must not decide on its own: a lexical link from
    /// <em>another</em> entry pointing at this entry or at one of its meanings — a relation target or a
    /// collocation synonym. While any such link exists nothing is deleted and an
    /// <see cref="InvalidOperationException"/> is thrown; remove those links first. Links pointing here
    /// from inside this entry are cleared as part of the delete, since they are owned by rows that are
    /// going anyway. The guard and the delete share one transaction, so nothing can slip between them.
    /// </para>
    /// </summary>
    public void LEntryDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LEntryLinkValidate(connection, id);
        LEntryLinkClear(connection, id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM entry WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    // Counts the lexical links that reach this entry from outside it: relations elsewhere whose target
    // is this entry or one of its meanings, and collocation synonyms elsewhere pointing at either. The
    // schema declares those columns without a cascade precisely so they cannot be swept away silently;
    // this turns the resulting foreign-key error into a message that names the reason.
    private static void LEntryLinkValidate(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM relation_entry target
                    JOIN relation origin ON origin.id = target.relation_id
                    JOIN sense holder ON holder.id = origin.sense_id
                 WHERE target.entry_id = $id AND holder.entry_id <> $id)
                + (SELECT COUNT(*) FROM relation_sense target
                    JOIN relation origin ON origin.id = target.relation_id
                    JOIN sense holder ON holder.id = origin.sense_id
                    JOIN sense aimed ON aimed.id = target.sense_id
                   WHERE aimed.entry_id = $id AND holder.entry_id <> $id)
                + (SELECT COUNT(*) FROM collocation_synonym link
                    JOIN collocation holder ON holder.id = link.collocation_id
                   WHERE link.target_entry_id = $id AND holder.entry_id <> $id)
                + (SELECT COUNT(*) FROM collocation_synonym link
                    JOIN collocation holder ON holder.id = link.collocation_id
                    JOIN sense aimed ON aimed.id = link.target_sense_id
                   WHERE aimed.entry_id = $id AND holder.entry_id <> $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        long links = Convert.ToInt64(command.ExecuteScalar());
        if (links > 0)
        {
            throw new InvalidOperationException(
                $"Entry {id} is still the target of {links} lexical link(s) from other entries; remove those links before deleting it.");
        }
    }

    // Clears the links this entry owns before the entry row goes. They would cascade anyway — a
    // relation's target row hangs from the relation, a synonym from its collocation — but a link that
    // points back at this same entry would be checked against a row already being deleted, and the
    // order the cascade visits tables in is not ours to rely on. Removing them first makes the delete
    // deterministic.
    private static void LEntryLinkClear(SqliteConnection connection, string id)
    {
        string[] statements =
        [
            """
            DELETE FROM relation_entry WHERE relation_id IN
                (SELECT origin.id FROM relation origin
                    JOIN sense holder ON holder.id = origin.sense_id
                 WHERE holder.entry_id = $id);
            """,
            """
            DELETE FROM relation_sense WHERE relation_id IN
                (SELECT origin.id FROM relation origin
                    JOIN sense holder ON holder.id = origin.sense_id
                 WHERE holder.entry_id = $id);
            """,
            """
            DELETE FROM collocation_synonym WHERE collocation_id IN
                (SELECT id FROM collocation WHERE entry_id = $id);
            """,
        ];

        foreach (string statement in statements)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = statement;
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
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
