using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LShengfuArchive : LShengfuVault
{
    private readonly LDatabase _lShengfuArchiveDatabase;

    public LShengfuArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lShengfuArchiveDatabase = database;
    }

    public void LShengfuSave(string language, LShengfu row)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentNullException.ThrowIfNull(row);

        using LDatabaseSession session = _lShengfuArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO shengfu (language, character, text, source)
            VALUES ($language, $character, $text, $source)
            ON CONFLICT (language, character) DO UPDATE SET
                text = excluded.text, source = excluded.source;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$character", row.LShengfuCharacter);
        command.Parameters.AddWithValue("$text", row.LShengfuText);
        command.Parameters.AddWithValue("$source", row.LShengfuSource);
        command.ExecuteNonQuery();
        session.LDatabaseSessionCommit();
    }

    public LShengfu? LShengfuRead(string language, string character)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        using LDatabaseSession session = _lShengfuArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT text, source
            FROM shengfu
            WHERE language = $language AND character = $character;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$character", character);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? new LShengfu(character, reader.GetString(0), reader.GetString(1)) : null;
    }

    public IReadOnlyList<LShengfu> LShengfuScan(string language, IReadOnlyList<string> characters)
    {
        ArgumentNullException.ThrowIfNull(characters);

        List<LShengfu> rows = [];
        foreach (string character in characters)
        {
            if (LShengfuRead(language, character) is LShengfu row)
            {
                rows.Add(row);
            }
        }

        return rows;
    }
}
