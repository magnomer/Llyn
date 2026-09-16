using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LDiweiArchive
{
    private const string LDiweiArchiveTally =
        """
        (SELECT COUNT(DISTINCT e.entry_id)
         FROM fanqie_diwei l
         JOIN fanqie f ON f.fanqie_id = l.fanqie_parent
         JOIN entry e ON e.language = f.language AND instr(e.headword, f.character) > 0
         WHERE l.diwei_ref = d.diwei_id)
        """;

    private readonly LDatabase _lDiweiArchiveDatabase;

    public LDiweiArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lDiweiArchiveDatabase = database;
    }

    public void LDiweiApply(string language, string character, LHypothesis? hypothesis)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        using LDatabaseSession session = _lDiweiArchiveDatabase.LDatabaseSessionStart();
        LDiweiCharacterApply(session, language, character, hypothesis);
        LDiweiOrphanClear(session, language);
        session.LDatabaseSessionCommit();
    }

    public void LDiweiRebuild(string language, LHypothesis? hypothesis)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        using LDatabaseSession session = _lDiweiArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand clear = session.LDatabaseSessionConnection.CreateCommand())
        {
            clear.CommandText = "DELETE FROM diwei WHERE language = $language;";
            clear.Parameters.AddWithValue("$language", language);
            clear.ExecuteNonQuery();
        }

        List<string> characters = [];
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "SELECT DISTINCT character FROM fanqie WHERE language = $language;";
            command.Parameters.AddWithValue("$language", language);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                characters.Add(reader.GetString(0));
            }
        }

        foreach (string character in characters)
        {
            LDiweiCharacterApply(session, language, character, hypothesis);
        }

        session.LDatabaseSessionCommit();
    }

    public IReadOnlyList<LDiwei> LDiweiRead(string language, string kind)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(kind);

        using LDatabaseSession session = _lDiweiArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT d.diwei_id, d.language, d.kind, d.key, {LDiweiArchiveTally}
            FROM diwei d
            WHERE d.language = $language AND d.kind = $kind
            ORDER BY d.diwei_id;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$kind", kind);

        List<LDiwei> rows = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(LDiweiRowRead(reader));
        }

        return rows;
    }

    public LDiwei? LDiweiFind(string language, string kind, string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(kind);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        using LDatabaseSession session = _lDiweiArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT d.diwei_id, d.language, d.kind, d.key, {LDiweiArchiveTally}
            FROM diwei d
            WHERE d.language = $language AND d.kind = $kind AND d.key = $key;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$kind", kind);
        command.Parameters.AddWithValue("$key", key);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LDiweiRowRead(reader) : null;
    }

    public IReadOnlyList<long> LDiweiEntryScan(string language, IReadOnlyList<long> diweiIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentNullException.ThrowIfNull(diweiIds);

        if (diweiIds.Count == 0)
        {
            return [];
        }

        using LDatabaseSession session = _lDiweiArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        string wanted = string.Join(", ", diweiIds.Select((_, index) => "$diwei" + index));
        command.CommandText =
            $"""
            SELECT DISTINCT e.entry_id
            FROM fanqie f
            JOIN entry e ON e.language = f.language AND instr(e.headword, f.character) > 0
            WHERE f.language = $language
              AND (SELECT COUNT(*) FROM fanqie_diwei l
                   WHERE l.fanqie_parent = f.fanqie_id AND l.diwei_ref IN ({wanted})) = $count
            ORDER BY e.entry_id;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$count", diweiIds.Distinct().Count());
        for (int index = 0; index < diweiIds.Count; index++)
        {
            command.Parameters.AddWithValue("$diwei" + index, diweiIds[index]);
        }

        List<long> entries = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            entries.Add(reader.GetInt64(0));
        }

        return entries;
    }

    public IReadOnlyList<LFanqieRow> LDiweiFanqieRead(long diweiId)
    {
        using LDatabaseSession session = _lDiweiArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT f.character, f.book, f.position, f.text, f.initial, f.rime, f.heading, f.division, f.tone,
                   f.rounded, f.source, f.spelling, f.reading, f.tone_class
            FROM fanqie_diwei l
            JOIN fanqie f ON f.fanqie_id = l.fanqie_parent
            WHERE l.diwei_ref = $diwei
            ORDER BY f.fanqie_id;
            """;
        command.Parameters.AddWithValue("$diwei", diweiId);

        List<LFanqieRow> rows = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new LFanqieRow(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetString(6),
                reader.GetString(7),
                reader.GetString(8),
                reader.GetInt32(9) != 0,
                reader.GetString(10),
                reader.GetString(11),
                reader.GetString(12),
                reader.GetString(13)));
        }

        return rows;
    }

    private static LDiwei LDiweiRowRead(SqliteDataReader reader)
    {
        return new LDiwei(
            reader.GetInt64(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetInt32(4));
    }

    private static void LDiweiCharacterApply(
        LDatabaseSession session, string language, string character, LHypothesis? hypothesis)
    {
        List<(long, LFanqieRow)> rows = [];
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                SELECT fanqie_id, book, position, text, initial, rime, heading, division, tone, rounded,
                       source, spelling
                FROM fanqie
                WHERE language = $language AND character = $character;
                """;
            command.Parameters.AddWithValue("$language", language);
            command.Parameters.AddWithValue("$character", character);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add((reader.GetInt64(0), new LFanqieRow(
                    character,
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.GetString(6),
                    reader.GetString(7),
                    reader.GetString(8),
                    reader.GetInt32(9) != 0,
                    reader.GetString(10),
                    reader.GetString(11))));
            }
        }

        foreach ((long fanqieId, LFanqieRow row) in rows)
        {
            using (SqliteCommand clear = session.LDatabaseSessionConnection.CreateCommand())
            {
                clear.CommandText = "DELETE FROM fanqie_diwei WHERE fanqie_parent = $fanqie;";
                clear.Parameters.AddWithValue("$fanqie", fanqieId);
                clear.ExecuteNonQuery();
            }

            LHypothesisSound? sound = hypothesis?.LHypothesisResolve(row);
            LDiweiReadingSave(session, fanqieId, sound);
            foreach ((string kind, string key) in LDiweiKeyScan(row, sound))
            {
                LDiweiLinkCreate(session, fanqieId, LDiweiRowCreate(session, language, kind, key));
            }
        }
    }

    private static void LDiweiReadingSave(LDatabaseSession session, long fanqieId, LHypothesisSound? sound)
    {
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "UPDATE fanqie SET reading = $reading, tone_class = $class WHERE fanqie_id = $fanqie;";
        command.Parameters.AddWithValue("$reading", sound?.LHypothesisSoundText ?? string.Empty);
        command.Parameters.AddWithValue("$class", sound?.LHypothesisSoundClass ?? string.Empty);
        command.Parameters.AddWithValue("$fanqie", fanqieId);
        command.ExecuteNonQuery();
    }

    private static IEnumerable<(string, string)> LDiweiKeyScan(LFanqieRow row, LHypothesisSound? sound)
    {
        if (row.LFanqieRowInitial.Length > 0)
        {
            yield return (LDiwei.LDiweiInitial, row.LFanqieRowInitial);
        }

        string rime = LDiwei.LDiweiRimeNormalize(row.LFanqieRowRime);
        if (rime.Length > 0)
        {
            yield return (LDiwei.LDiweiRime, rime);
        }

        string? tone = sound?.LHypothesisSoundClass;
        if (!string.IsNullOrEmpty(tone))
        {
            yield return (LDiwei.LDiweiTone, tone);
        }
    }

    private static long LDiweiRowCreate(LDatabaseSession session, string language, string kind, string key)
    {
        using (SqliteCommand insert = session.LDatabaseSessionConnection.CreateCommand())
        {
            insert.CommandText =
                "INSERT OR IGNORE INTO diwei (language, kind, key) VALUES ($language, $kind, $key);";
            insert.Parameters.AddWithValue("$language", language);
            insert.Parameters.AddWithValue("$kind", kind);
            insert.Parameters.AddWithValue("$key", key);
            insert.ExecuteNonQuery();
        }

        using SqliteCommand select = session.LDatabaseSessionConnection.CreateCommand();
        select.CommandText =
            "SELECT diwei_id FROM diwei WHERE language = $language AND kind = $kind AND key = $key;";
        select.Parameters.AddWithValue("$language", language);
        select.Parameters.AddWithValue("$kind", kind);
        select.Parameters.AddWithValue("$key", key);
        return (long)select.ExecuteScalar()!;
    }

    private static void LDiweiLinkCreate(LDatabaseSession session, long fanqieId, long diweiId)
    {
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            "INSERT OR IGNORE INTO fanqie_diwei (fanqie_parent, diwei_ref) VALUES ($fanqie, $diwei);";
        command.Parameters.AddWithValue("$fanqie", fanqieId);
        command.Parameters.AddWithValue("$diwei", diweiId);
        command.ExecuteNonQuery();
    }

    private static void LDiweiOrphanClear(LDatabaseSession session, string language)
    {
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            DELETE FROM diwei
            WHERE language = $language
              AND NOT EXISTS (SELECT 1 FROM fanqie_diwei l WHERE l.diwei_ref = diwei.diwei_id);
            """;
        command.Parameters.AddWithValue("$language", language);
        command.ExecuteNonQuery();
    }
}
