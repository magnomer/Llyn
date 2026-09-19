using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LScriptArchive : LScriptVault
{
    private readonly LDatabase _lScriptArchiveDatabase;

    public LScriptArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lScriptArchiveDatabase = database;
    }

    public void LScriptSave(string language, string character, IReadOnlyList<LScriptImage> images)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);
        ArgumentNullException.ThrowIfNull(images);

        using LDatabaseSession session = _lScriptArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand clear = session.LDatabaseSessionConnection.CreateCommand())
        {
            clear.CommandText = "DELETE FROM script WHERE language = $language AND character = $character;";
            clear.Parameters.AddWithValue("$language", language);
            clear.Parameters.AddWithValue("$character", character);
            clear.ExecuteNonQuery();
        }

        foreach (LScriptImage image in images)
        {
            using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO script (language, character, style, position, caption, gloss, data)
                VALUES ($language, $character, $style, $position, $caption, $gloss, $data);
                """;
            command.Parameters.AddWithValue("$language", language);
            command.Parameters.AddWithValue("$character", character);
            command.Parameters.AddWithValue("$style", image.LScriptImageStyle);
            command.Parameters.AddWithValue("$position", image.LScriptImagePosition);
            command.Parameters.AddWithValue("$caption", image.LScriptImageCaption);
            command.Parameters.AddWithValue("$gloss", image.LScriptImageGloss);
            command.Parameters.AddWithValue("$data", image.LScriptImageData);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public IReadOnlyList<LScriptImage> LScriptRead(string language, string character)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        using LDatabaseSession session = _lScriptArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT style, position, caption, gloss, data
            FROM script
            WHERE language = $language AND character = $character
            ORDER BY script_id;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$character", character);

        List<LScriptImage> images = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            images.Add(new LScriptImage(
                character,
                reader.GetString(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetString(3),
                (byte[])reader.GetValue(4)));
        }

        return images;
    }
}
