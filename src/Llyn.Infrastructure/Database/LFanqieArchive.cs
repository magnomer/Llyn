using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LFanqieArchive
{
    private readonly LDatabase _lFanqieArchiveDatabase;

    public LFanqieArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lFanqieArchiveDatabase = database;
    }

    public void LFanqieSave(string language, string character, IReadOnlyList<LFanqieRow> rows)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);
        ArgumentNullException.ThrowIfNull(rows);

        using LDatabaseSession session = _lFanqieArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand clear = session.LDatabaseSessionConnection.CreateCommand())
        {
            clear.CommandText = "DELETE FROM fanqie WHERE language = $language AND character = $character;";
            clear.Parameters.AddWithValue("$language", language);
            clear.Parameters.AddWithValue("$character", character);
            clear.ExecuteNonQuery();
        }

        foreach (LFanqieRow row in rows)
        {
            using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO fanqie (
                    language, character, book, position, text,
                    initial, rime, heading, division, tone, rounded, source, spelling)
                VALUES (
                    $language, $character, $book, $position, $text,
                    $initial, $rime, $heading, $division, $tone, $rounded, $source, $spelling);
                """;
            command.Parameters.AddWithValue("$language", language);
            command.Parameters.AddWithValue("$character", character);
            command.Parameters.AddWithValue("$book", row.LFanqieRowBook);
            command.Parameters.AddWithValue("$position", row.LFanqieRowPosition);
            command.Parameters.AddWithValue("$text", row.LFanqieRowText);
            command.Parameters.AddWithValue("$initial", row.LFanqieRowInitial);
            command.Parameters.AddWithValue("$rime", row.LFanqieRowRime);
            command.Parameters.AddWithValue("$heading", row.LFanqieRowHeading);
            command.Parameters.AddWithValue("$division", row.LFanqieRowDivision);
            command.Parameters.AddWithValue("$tone", row.LFanqieRowTone);
            command.Parameters.AddWithValue("$rounded", row.LFanqieRowRounded ? 1 : 0);
            command.Parameters.AddWithValue("$source", row.LFanqieRowSource);
            command.Parameters.AddWithValue("$spelling", row.LFanqieRowSpelling);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public IReadOnlyList<LFanqieRow> LFanqieRead(string language, string character)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        using LDatabaseSession session = _lFanqieArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT book, position, text, initial, rime, heading, division, tone, rounded, source, spelling
            FROM fanqie
            WHERE language = $language AND character = $character
            ORDER BY fanqie_id;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$character", character);

        List<LFanqieRow> rows = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new LFanqieRow(
                character,
                reader.GetString(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetString(6),
                reader.GetString(7),
                reader.GetInt32(8) != 0,
                reader.GetString(9),
                reader.GetString(10)));
        }

        return rows;
    }
}
