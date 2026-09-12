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

    public LSpeechValue LSpeechValueCreate(LSpeechValue value)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(value.LSpeechValueLanguage);
        ArgumentException.ThrowIfNullOrWhiteSpace(value.LSpeechValueName);

        using LDatabaseSession session = _lSpeechArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        long packId = value.LSpeechValueCode == 0
            ? LSpeechCodeCreate(connection, value.LSpeechValueLanguage)
            : value.LSpeechValueCode;

        long id;
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO speech_value (language, pack_code, name, position)
                VALUES ($language, $pack, $name, $position)
                ON CONFLICT (language, pack_code)
                DO UPDATE SET name = excluded.name, position = excluded.position
                RETURNING speech_value_id;
                """;
            command.Parameters.AddWithValue("$language", value.LSpeechValueLanguage);
            command.Parameters.AddWithValue("$pack", packId);
            command.Parameters.AddWithValue("$name", value.LSpeechValueName);
            command.Parameters.AddWithValue("$position", value.LSpeechValuePosition);
            id = Convert.ToInt64(command.ExecuteScalar());
        }

        session.LDatabaseSessionCommit();
        return value with { LSpeechValueId = id, LSpeechValueCode = packId };
    }

    public LSpeechValue? LSpeechValueRead(long id)
    {
        if (id <= 0)
        {
            return null;
        }

        using LDatabaseSession session = _lSpeechArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT speech_value_id, language, pack_code, name, position FROM speech_value WHERE speech_value_id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LSpeechRowRead(reader) : null;
    }

    public IReadOnlyList<LSpeechValue> LSpeechValueRead(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        using LDatabaseSession session = _lSpeechArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT speech_value_id, language, pack_code, name, position FROM speech_value
            WHERE language = $language ORDER BY position, speech_value_id;
            """;
        command.Parameters.AddWithValue("$language", language);

        List<LSpeechValue> values = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            values.Add(LSpeechRowRead(reader));
        }

        return values;
    }

    public LSpeechValue? LSpeechValueFind(string language, string name)
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
            SELECT speech_value_id, language, pack_code, name, position FROM speech_value
            WHERE language = $language AND trim(name) = trim($name) COLLATE NOCASE
            ORDER BY position, speech_value_id LIMIT 1;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$name", name);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LSpeechRowRead(reader) : null;
    }

    private static long LSpeechCodeCreate(SqliteConnection connection, string language)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "SELECT min(0, ifnull(MIN(pack_code), 0)) - 1 FROM speech_value WHERE language = $language;";
        command.Parameters.AddWithValue("$language", language);
        return Convert.ToInt64(command.ExecuteScalar());
    }

    private static LSpeechValue LSpeechRowRead(SqliteDataReader reader)
    {
        return new LSpeechValue(
            reader.GetInt64(0),
            reader.GetString(1),
            reader.GetInt64(2),
            reader.GetString(3),
            reader.GetInt32(4));
    }
}
