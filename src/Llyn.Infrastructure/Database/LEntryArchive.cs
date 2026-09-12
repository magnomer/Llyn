using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LEntryArchive
{
    private readonly LDatabase _lEntryArchiveDatabase;

    public LEntryArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lEntryArchiveDatabase = database;
    }

    public LEntry LEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(forms);
        ArgumentNullException.ThrowIfNull(speeches);

        string now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        LEntry stored = entry with
        {
            LEntryAddedUtc = now,
            LEntryUpdatedUtc = now,
        };

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO entry (headword, language, proficiency, frequency, added_utc, updated_utc)
                VALUES ($headword, $language, $proficiency, $frequency, $added, $updated)
                RETURNING id;
                """;
            command.Parameters.AddWithValue("$headword", stored.LEntryHeadword);
            command.Parameters.AddWithValue("$language", stored.LEntryLanguage);
            command.Parameters.AddWithValue("$proficiency", (object?)stored.LEntryProficiency ?? DBNull.Value);
            command.Parameters.AddWithValue("$frequency", (object?)stored.LEntryFrequency ?? DBNull.Value);
            command.Parameters.AddWithValue("$added", (object?)stored.LEntryAddedUtc ?? DBNull.Value);
            command.Parameters.AddWithValue("$updated", (object?)stored.LEntryUpdatedUtc ?? DBNull.Value);
            stored = stored with { LEntryId = (long)command.ExecuteScalar()! };
        }

        LEntryFormInsert(connection, stored.LEntryId, forms);
        LEntrySpeechInsert(connection, stored.LEntryId, speeches);

        session.LDatabaseSessionCommit();
        return stored;
    }

    public LEntry? LEntryRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id, headword, language, proficiency, frequency, added_utc, updated_utc
            FROM entry WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LEntryRowRead(reader) : null;
    }

    public IReadOnlyList<LEntry> LEntryFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);
        query = query.Trim();

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id, headword, language, proficiency, frequency, added_utc, updated_utc
            FROM entry
            WHERE $query = '' OR instr(lfold(headword), lfold($query)) > 0
            ORDER BY headword;
            """;
        command.Parameters.AddWithValue("$query", query);

        List<LEntry> entries = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            entries.Add(LEntryRowRead(reader));
        }

        return entries;
    }

    public IReadOnlyList<LEntry> LEntryTagFind(long tagId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(tagId);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id, headword, language, proficiency, frequency, added_utc, updated_utc
            FROM entry
            WHERE $tag = 0
               OR id IN (
                      SELECT sense.entry_id
                      FROM sense_tag
                      JOIN sense ON sense.id = sense_tag.sense_id
                      WHERE sense_tag.tag_ref = $tag
                      UNION
                      SELECT collocation.entry_id
                      FROM collocation_tag
                      JOIN collocation ON collocation.id = collocation_tag.collocation_id
                      WHERE collocation_tag.tag_ref = $tag)
            ORDER BY headword;
            """;
        command.Parameters.AddWithValue("$tag", tagId);

        List<LEntry> entries = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            entries.Add(LEntryRowRead(reader));
        }

        return entries;
    }

    public IReadOnlyList<LEntry> LEntryRegisterFind(long registerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(registerId);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id, headword, language, proficiency, frequency, added_utc, updated_utc
            FROM entry
            WHERE $register = 0
               OR id IN (
                      SELECT sense.entry_id
                      FROM sense_register
                      JOIN sense ON sense.id = sense_register.sense_id
                      WHERE sense_register.register_ref = $register
                      UNION
                      SELECT collocation.entry_id
                      FROM collocation_register
                      JOIN collocation ON collocation.id = collocation_register.collocation_id
                      WHERE collocation_register.register_ref = $register)
            ORDER BY headword;
            """;
        command.Parameters.AddWithValue("$register", registerId);

        List<LEntry> entries = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            entries.Add(LEntryRowRead(reader));
        }

        return entries;
    }

    public IReadOnlyList<LForm> LEntryFormRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

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
                reader.GetInt64(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.GetString(4)));
        }

        return forms;
    }

    public IReadOnlyList<LSpeech> LEntrySpeechRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT entry_id, position, speech_value_ref, custom_name
            FROM part_of_speech WHERE entry_id = $id ORDER BY position;
            """;
        command.Parameters.AddWithValue("$id", id);

        List<LSpeech> speeches = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            speeches.Add(new LSpeech(
                reader.GetInt64(0),
                reader.GetInt32(1),
                reader.IsDBNull(2) ? null : reader.GetInt64(2),
                reader.IsDBNull(3) ? null : reader.GetString(3)));
        }

        return speeches;
    }

    public void LEntryUpdate(LEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entry.LEntryId);

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

    public void LEntryFormSet(long id, IReadOnlyList<LForm> forms)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(forms);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        LEntryChildClear(connection, "form", id);
        LEntryFormInsert(connection, id, forms);
        session.LDatabaseSessionCommit();
    }

    public void LEntrySpeechSet(long id, IReadOnlyList<LSpeech> speeches)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(speeches);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        LEntryChildClear(connection, "part_of_speech", id);
        LEntrySpeechInsert(connection, id, speeches);
        session.LDatabaseSessionCommit();
    }

    public void LEntryDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM entry WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    private static LEntry LEntryRowRead(SqliteDataReader reader)
    {
        return new LEntry(
            reader.GetInt64(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.IsDBNull(3) ? null : reader.GetString(3),
            reader.IsDBNull(4) ? null : reader.GetString(4),
            reader.IsDBNull(5) ? null : reader.GetString(5),
            reader.IsDBNull(6) ? null : reader.GetString(6));
    }

    private static void LEntryFormInsert(SqliteConnection connection, long id, IReadOnlyList<LForm> forms)
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

    private static void LEntrySpeechInsert(SqliteConnection connection, long id, IReadOnlyList<LSpeech> speeches)
    {
        for (int position = 0; position < speeches.Count; position++)
        {
            LSpeech speech = speeches[position];
            using SqliteCommand command = connection.CreateCommand();

            bool declared = speech.LSpeechValueId is > 0;
            command.CommandText =
                """
                INSERT INTO part_of_speech (entry_id, position, speech_value_ref, custom_name)
                VALUES ($entry, $position, $value, $custom);
                """;
            command.Parameters.AddWithValue("$entry", id);
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$value", declared ? speech.LSpeechValueId!.Value : DBNull.Value);
            command.Parameters.AddWithValue(
                "$custom", declared ? DBNull.Value : (object?)speech.LSpeechCustom ?? DBNull.Value);
            command.ExecuteNonQuery();
        }
    }

    private static void LEntryChildClear(SqliteConnection connection, string table, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {table} WHERE entry_id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}
