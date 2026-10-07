using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LSpeechArchive : LSpeechVault
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

    public LSpeechPack LSpeechLoad(string language)
    {
        return LSpeechLoader.LSpeechLoaderLoad(language);
    }

    public void LSpeechRetirementApply(string language, LSpeechRetirement row)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentNullException.ThrowIfNull(row);

        using LDatabaseSession session = _lSpeechArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        if (LSpeechRetirementFind(connection, language, row.LSpeechRetirementCode) is not long retired)
        {
            return;
        }

        long? target = row.LSpeechRetirementTarget > 0
            ? LSpeechRetirementFind(connection, language, row.LSpeechRetirementTarget)
            : null;
        List<long> entries = LSpeechRetirementScan(connection, retired);

        if (row.LSpeechRetirementUnit != LUnit.LUnitEmpty)
        {
            LSpeechRetirementRun(
                connection,
                """
                UPDATE entry SET unit = $unit
                WHERE unit = 0
                  AND entry_id IN (SELECT entry_parent FROM part_of_speech WHERE speech_value_ref = $retired);
                """,
                retired,
                (int)row.LSpeechRetirementUnit);
        }

        if (target is long kept)
        {
            LSpeechRetirementRun(
                connection,
                """
                DELETE FROM part_of_speech
                WHERE speech_value_ref = $retired
                  AND entry_parent IN (SELECT entry_parent FROM part_of_speech WHERE speech_value_ref = $target);
                UPDATE part_of_speech SET speech_value_ref = $target WHERE speech_value_ref = $retired;
                UPDATE inflection SET speech_value_ref = $target WHERE speech_value_ref = $retired;
                UPDATE OR IGNORE morphology_feature SET speech_value_parent = $target
                WHERE speech_value_parent = $retired;
                """,
                retired,
                kept);
        }
        else
        {
            LSpeechRetirementRun(
                connection,
                """
                DELETE FROM part_of_speech WHERE speech_value_ref = $retired;
                UPDATE inflection SET speech_value_ref = NULL WHERE speech_value_ref = $retired;
                """,
                retired,
                0);
        }

        LSpeechRetirementRun(connection, "DELETE FROM speech_value WHERE speech_value_id = $retired;", retired, 0);
        foreach (long entry in entries)
        {
            LSpeechPositionReset(connection, entry);
        }

        session.LDatabaseSessionCommit();
    }

    private static long? LSpeechRetirementFind(SqliteConnection connection, string language, long code)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "SELECT speech_value_id FROM speech_value WHERE language = $language AND pack_code = $code;";
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$code", code);
        return command.ExecuteScalar() is long id ? id : null;
    }

    private static List<long> LSpeechRetirementScan(SqliteConnection connection, long retired)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "SELECT DISTINCT entry_parent FROM part_of_speech WHERE speech_value_ref = $retired;";
        command.Parameters.AddWithValue("$retired", retired);

        List<long> entries = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            entries.Add(reader.GetInt64(0));
        }

        return entries;
    }

    private static void LSpeechRetirementRun(SqliteConnection connection, string text, long retired, long value)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = text;
        command.Parameters.AddWithValue("$retired", retired);
        command.Parameters.AddWithValue("$target", value);
        command.Parameters.AddWithValue("$unit", value);
        command.ExecuteNonQuery();
    }

    private static void LSpeechPositionReset(SqliteConnection connection, long entry)
    {
        List<long> positions = [];
        using (SqliteCommand read = connection.CreateCommand())
        {
            read.CommandText = "SELECT position FROM part_of_speech WHERE entry_parent = $entry ORDER BY position;";
            read.Parameters.AddWithValue("$entry", entry);
            using SqliteDataReader reader = read.ExecuteReader();
            while (reader.Read())
            {
                positions.Add(reader.GetInt64(0));
            }
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "UPDATE part_of_speech SET position = $next WHERE entry_parent = $entry AND position = $position;";
        SqliteParameter next = command.Parameters.Add("$next", SqliteType.Integer);
        SqliteParameter position = command.Parameters.Add("$position", SqliteType.Integer);
        command.Parameters.AddWithValue("$entry", entry);
        for (int index = 0; index < positions.Count; index++)
        {
            position.Value = positions[index];
            next.Value = -1 - index;
            command.ExecuteNonQuery();
        }

        for (int index = 0; index < positions.Count; index++)
        {
            position.Value = -1 - index;
            next.Value = index;
            command.ExecuteNonQuery();
        }
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
