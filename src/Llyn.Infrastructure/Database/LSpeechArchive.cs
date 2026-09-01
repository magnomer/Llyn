using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LSpeechArchive
{
    private readonly LDatabase _lSpeechArchiveDatabase;

    public LSpeechArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSpeechArchiveDatabase = database;
    }

    public void LSpeechValueCreate(LSpeechValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        using LDatabaseSession session = _lSpeechArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO part_of_speech_value (language, value_id, display_name, position)
                VALUES ($language, $value, $name, $position)
                ON CONFLICT (language, value_id)
                DO UPDATE SET display_name = excluded.display_name, position = excluded.position;
                """;
            command.Parameters.AddWithValue("$language", value.LSpeechValueLanguage);
            command.Parameters.AddWithValue("$value", value.LSpeechValueId);
            command.Parameters.AddWithValue("$name", value.LSpeechValueName);
            command.Parameters.AddWithValue("$position", value.LSpeechValuePosition);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public string? LSpeechValueRead(string language, string valueId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(valueId);

        using LDatabaseSession session = _lSpeechArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT display_name FROM part_of_speech_value
            WHERE language = $language AND value_id = $value;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$value", valueId);

        object? result = command.ExecuteScalar();
        return result is string name ? name : null;
    }

    public IReadOnlyList<LSpeechValue> LSpeechValueRead(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        using LDatabaseSession session = _lSpeechArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT language, value_id, display_name, position FROM part_of_speech_value
            WHERE language = $language ORDER BY position;
            """;
        command.Parameters.AddWithValue("$language", language);

        List<LSpeechValue> values = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            values.Add(new LSpeechValue(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetInt32(3)));
        }

        return values;
    }

    public string? LSpeechValueFind(string language, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        using LDatabaseSession session = _lSpeechArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();

        command.CommandText =
            """
            SELECT value_id FROM part_of_speech_value
            WHERE language = $language AND trim(display_name) = trim($name) COLLATE NOCASE
            ORDER BY position LIMIT 1;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$name", name);

        object? result = command.ExecuteScalar();
        return result is string value ? value : null;
    }
}
