using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LFanqieArchive : LFanqieVault
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
        List<long> kept = [];
        foreach (LFanqieRow row in rows)
        {
            using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO fanqie (
                    language, character, book, position, text,
                    initial, rime, heading, division, tone, rounded, source, spelling, reading, tone_class)
                VALUES (
                    $language, $character, $book, $position, $text,
                    $initial, $rime, $heading, $division, $tone, $rounded, $source, $spelling, $reading, $class)
                ON CONFLICT (language, character, book, source, position) DO UPDATE SET
                    text = excluded.text, initial = excluded.initial, rime = excluded.rime,
                    heading = excluded.heading, division = excluded.division, tone = excluded.tone,
                    rounded = excluded.rounded, spelling = excluded.spelling,
                    reading = excluded.reading, tone_class = excluded.tone_class
                RETURNING fanqie_id;
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
            command.Parameters.AddWithValue("$reading", row.LFanqieRowReading);
            command.Parameters.AddWithValue("$class", row.LFanqieRowClass);
            kept.Add((long)command.ExecuteScalar()!);
        }

        LFanqieLeftoverDelete(session.LDatabaseSessionConnection, language, character, kept);
        LFanqieRankApply(
            session.LDatabaseSessionConnection,
            LFanqieMarkedRead(session.LDatabaseSessionConnection, language, character));
        session.LDatabaseSessionCommit();
    }

    private static void LFanqieLeftoverDelete(
        SqliteConnection connection, string language, string character, List<long> kept)
    {
        using SqliteCommand command = connection.CreateCommand();
        string held = kept.Count == 0 ? "0" : string.Join(", ", kept);
        command.CommandText =
            $"DELETE FROM fanqie WHERE language = $language AND character = $character AND fanqie_id NOT IN ({held});";
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$character", character);
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<LFanqieRow> LFanqieRead(string language, string character)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        using LDatabaseSession session = _lFanqieArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT book, position, text, initial, rime, heading, division, tone, rounded, source, spelling,
                   reading, tone_class, fanqie_id, representative
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
                reader.GetString(10),
                reader.GetString(11),
                reader.GetString(12),
                reader.GetInt64(13),
                LFanqieRowRepresentative: reader.GetInt32(14)));
        }

        return rows;
    }

    public void LFanqieRepresentativeSet(long fanqieId, int rank)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fanqieId);

        using LDatabaseSession session = _lFanqieArchiveDatabase.LDatabaseSessionStart();
        if (LFanqiePlaceRead(session.LDatabaseSessionConnection, fanqieId) is not (string language, string character))
        {
            return;
        }

        List<long> marked = LFanqieMarkedRead(session.LDatabaseSessionConnection, language, character);
        marked.Remove(fanqieId);
        if (rank > 0)
        {
            marked.Insert(Math.Min(rank - 1, marked.Count), fanqieId);
        }
        else
        {
            LFanqieRankSave(session.LDatabaseSessionConnection, fanqieId, 0);
        }

        LFanqieRankApply(session.LDatabaseSessionConnection, marked);
        session.LDatabaseSessionCommit();
    }

    private static (string, string)? LFanqiePlaceRead(SqliteConnection connection, long fanqieId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT language, character FROM fanqie WHERE fanqie_id = $id;";
        command.Parameters.AddWithValue("$id", fanqieId);
        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? (reader.GetString(0), reader.GetString(1)) : null;
    }

    private static List<long> LFanqieMarkedRead(SqliteConnection connection, string language, string character)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT fanqie_id
            FROM fanqie
            WHERE language = $language AND character = $character AND representative > 0
            ORDER BY representative, fanqie_id;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$character", character);

        List<long> marked = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            marked.Add(reader.GetInt64(0));
        }

        return marked;
    }

    private static void LFanqieRankApply(SqliteConnection connection, IReadOnlyList<long> marked)
    {
        for (int place = 0; place < marked.Count; place++)
        {
            LFanqieRankSave(connection, marked[place], place + 1);
        }
    }

    private static void LFanqieRankSave(SqliteConnection connection, long fanqieId, int rank)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE fanqie SET representative = $representative WHERE fanqie_id = $id;";
        command.Parameters.AddWithValue("$representative", rank);
        command.Parameters.AddWithValue("$id", fanqieId);
        command.ExecuteNonQuery();
    }
}
