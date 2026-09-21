using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LStemArchive : LStemVault
{
    private const string LStemArchiveTally =
        """
        (SELECT COUNT(DISTINCT e.entry_id)
         FROM entry e
         WHERE e.language = s.language
           AND EXISTS (SELECT 1
                       FROM shengfu_stem l
                       JOIN shengfu f ON f.shengfu_id = l.shengfu_parent
                       WHERE l.stem_ref = s.stem_id AND instr(e.headword, f.character) > 0))
        """;

    private readonly LDatabase _lStemArchiveDatabase;

    public LStemArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lStemArchiveDatabase = database;
    }

    public void LStemApply(string language, string character, IReadOnlyList<string> keys)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);
        ArgumentNullException.ThrowIfNull(keys);

        using LDatabaseSession session = _lStemArchiveDatabase.LDatabaseSessionStart();
        LStemCharacterApply(session, language, character, keys);
        LStemOrphanClear(session, language);
        session.LDatabaseSessionCommit();
    }

    public void LStemRebuild(string language, string separator)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentNullException.ThrowIfNull(separator);

        using LDatabaseSession session = _lStemArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand clear = session.LDatabaseSessionConnection.CreateCommand())
        {
            clear.CommandText = "DELETE FROM stem WHERE language = $language;";
            clear.Parameters.AddWithValue("$language", language);
            clear.ExecuteNonQuery();
        }

        List<(string LStemCharacter, string LStemText)> rows = [];
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "SELECT character, text FROM shengfu WHERE language = $language;";
            command.Parameters.AddWithValue("$language", language);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add((reader.GetString(0), reader.GetString(1)));
            }
        }

        foreach ((string character, string text) in rows)
        {
            LStemCharacterApply(session, language, character, LStem.LStemKeyScan(text, separator));
        }

        session.LDatabaseSessionCommit();
    }

    public IReadOnlyList<LStem> LStemRead(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        using LDatabaseSession session = _lStemArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT s.stem_id, s.language, s.key, {LStemArchiveTally}
            FROM stem s
            WHERE s.language = $language
            ORDER BY s.stem_id;
            """;
        command.Parameters.AddWithValue("$language", language);

        List<LStem> rows = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(LStemRowRead(reader));
        }

        return rows;
    }

    public LStem? LStemRead(long stemId)
    {
        using LDatabaseSession session = _lStemArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT s.stem_id, s.language, s.key, {LStemArchiveTally}
            FROM stem s
            WHERE s.stem_id = $id;
            """;
        command.Parameters.AddWithValue("$id", stemId);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LStemRowRead(reader) : null;
    }

    public LStem? LStemFind(string language, string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        using LDatabaseSession session = _lStemArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT s.stem_id, s.language, s.key, {LStemArchiveTally}
            FROM stem s
            WHERE s.language = $language AND s.key = $key;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$key", key);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LStemRowRead(reader) : null;
    }

    public IReadOnlyList<string> LStemCharacterRead(long stemId)
    {
        using LDatabaseSession session = _lStemArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT f.character
            FROM shengfu_stem l
            JOIN shengfu f ON f.shengfu_id = l.shengfu_parent
            WHERE l.stem_ref = $stem
            ORDER BY f.shengfu_id;
            """;
        command.Parameters.AddWithValue("$stem", stemId);

        List<string> characters = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            characters.Add(reader.GetString(0));
        }

        return characters;
    }

    public IReadOnlyList<long> LStemEntryScan(string language, IReadOnlyList<long> stemIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentNullException.ThrowIfNull(stemIds);

        if (stemIds.Count == 0)
        {
            return [];
        }

        using LDatabaseSession session = _lStemArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        string wanted = string.Join(", ", stemIds.Select((_, index) => "$stem" + index));
        command.CommandText =
            $"""
            SELECT e.entry_id
            FROM entry e
            WHERE e.language = $language
              AND (SELECT COUNT(DISTINCT l.stem_ref)
                   FROM shengfu_stem l
                   JOIN shengfu f ON f.shengfu_id = l.shengfu_parent
                   WHERE l.stem_ref IN ({wanted}) AND instr(e.headword, f.character) > 0) = $count
            ORDER BY e.entry_id;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$count", stemIds.Distinct().Count());
        for (int index = 0; index < stemIds.Count; index++)
        {
            command.Parameters.AddWithValue("$stem" + index, stemIds[index]);
        }

        List<long> entries = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            entries.Add(reader.GetInt64(0));
        }

        return entries;
    }

    private static void LStemCharacterApply(
        LDatabaseSession session, string language, string character, IReadOnlyList<string> keys)
    {
        if (LStemShengfuRead(session, language, character) is not long shengfuId)
        {
            return;
        }

        using (SqliteCommand clear = session.LDatabaseSessionConnection.CreateCommand())
        {
            clear.CommandText = "DELETE FROM shengfu_stem WHERE shengfu_parent = $shengfu;";
            clear.Parameters.AddWithValue("$shengfu", shengfuId);
            clear.ExecuteNonQuery();
        }

        foreach (string key in keys)
        {
            LStemLinkCreate(session, shengfuId, LStemRowCreate(session, language, key));
        }
    }

    private static long? LStemShengfuRead(LDatabaseSession session, string language, string character)
    {
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            "SELECT shengfu_id FROM shengfu WHERE language = $language AND character = $character;";
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$character", character);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? reader.GetInt64(0) : null;
    }

    private static long LStemRowCreate(LDatabaseSession session, string language, string key)
    {
        using (SqliteCommand insert = session.LDatabaseSessionConnection.CreateCommand())
        {
            insert.CommandText = "INSERT OR IGNORE INTO stem (language, key) VALUES ($language, $key);";
            insert.Parameters.AddWithValue("$language", language);
            insert.Parameters.AddWithValue("$key", key);
            insert.ExecuteNonQuery();
        }

        using SqliteCommand select = session.LDatabaseSessionConnection.CreateCommand();
        select.CommandText = "SELECT stem_id FROM stem WHERE language = $language AND key = $key;";
        select.Parameters.AddWithValue("$language", language);
        select.Parameters.AddWithValue("$key", key);
        return (long)select.ExecuteScalar()!;
    }

    private static void LStemLinkCreate(LDatabaseSession session, long shengfuId, long stemId)
    {
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            INSERT OR IGNORE INTO shengfu_stem (shengfu_parent, stem_ref)
            VALUES ($shengfu, $stem);
            """;
        command.Parameters.AddWithValue("$shengfu", shengfuId);
        command.Parameters.AddWithValue("$stem", stemId);
        command.ExecuteNonQuery();
    }

    private static void LStemOrphanClear(LDatabaseSession session, string language)
    {
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            DELETE FROM stem
            WHERE language = $language
              AND NOT EXISTS (SELECT 1 FROM shengfu_stem l WHERE l.stem_ref = stem.stem_id);
            """;
        command.Parameters.AddWithValue("$language", language);
        command.ExecuteNonQuery();
    }

    private static LStem LStemRowRead(SqliteDataReader reader)
    {
        return new LStem(
            reader.GetInt64(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetInt32(3));
    }
}
