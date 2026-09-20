using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed partial class LEntryArchive
{
    public IReadOnlyList<LEntry> LEntryFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);
        query = query.Trim();

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT entry_id, headword, language, grasp, added_utc, updated_utc
            FROM entry
            WHERE lmatch(headword, $query)
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

    public IReadOnlyList<LEntry> LEntryHeadwordFind(string language, string headword)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(headword);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT entry_id, headword, language, grasp, added_utc, updated_utc
            FROM entry
            WHERE lfold(language) = lfold($language) AND lfold(headword) = lfold($headword)
            ORDER BY headword, entry_id;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$headword", headword);

        return LEntryListRead(command);
    }

    public IReadOnlyList<LEntry> LEntryHeadwordScan(string language, string text)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(text);

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT entry_id, headword, language, grasp, added_utc, updated_utc
            FROM entry
            WHERE language = $language AND headword <> '' AND instr(lfold($text), lfold(headword)) > 0
            ORDER BY length(headword) DESC, headword, entry_id;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$text", text);

        return LEntryListRead(command);
    }

    public IReadOnlyList<LEntry> LEntryScan(IReadOnlyList<long> ids, string query)
    {
        ArgumentNullException.ThrowIfNull(ids);
        ArgumentNullException.ThrowIfNull(query);
        query = query.Trim();

        if (ids.Count == 0)
        {
            return [];
        }

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        List<LEntry> entries = [];
        foreach (IReadOnlyList<long> batch in LEntryListDivide(ids))
        {
            using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
            command.CommandText =
                $"""
                SELECT entry_id, headword, language, grasp, added_utc, updated_utc
                FROM entry
                WHERE entry_id IN ({LEntryListFormat(command, batch)}) AND lmatch(headword, $query)
                ORDER BY entry_id;
                """;
            command.Parameters.AddWithValue("$query", query);
            entries.AddRange(LEntryListRead(command));
        }

        return entries;
    }

    public IReadOnlyDictionary<long, string> LEntryEpithetScan(IReadOnlyList<long> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);

        Dictionary<long, string> epithets = [];
        if (ids.Count == 0)
        {
            return epithets;
        }

        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        foreach (IReadOnlyList<long> batch in LEntryListDivide(ids))
        {
            using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
            command.CommandText =
                $"""
                SELECT entry_id, epithet FROM entry
                WHERE entry_id IN ({LEntryListFormat(command, batch)}) AND epithet <> '';
                """;
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                epithets[reader.GetInt64(0)] = reader.GetString(1);
            }
        }

        return epithets;
    }

    private static IEnumerable<IReadOnlyList<long>> LEntryListDivide(IReadOnlyList<long> ids)
    {
        const int limit = 500;
        for (int start = 0; start < ids.Count; start += limit)
        {
            List<long> batch = new(Math.Min(limit, ids.Count - start));
            for (int index = start; index < ids.Count && index < start + limit; index++)
            {
                batch.Add(ids[index]);
            }

            yield return batch;
        }
    }

    private static string LEntryListFormat(SqliteCommand command, IReadOnlyList<long> ids)
    {
        string[] names = new string[ids.Count];
        for (int index = 0; index < ids.Count; index++)
        {
            names[index] = "$id" + index.ToString(CultureInfo.InvariantCulture);
            command.Parameters.AddWithValue(names[index], ids[index]);
        }

        return string.Join(", ", names);
    }

    private static IReadOnlyList<LEntry> LEntryListRead(SqliteCommand command)
    {
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
            SELECT entry_id, headword, language, grasp, added_utc, updated_utc
            FROM entry
            WHERE $tag = 0
               OR entry_id IN (
                      SELECT sense.entry_parent
                      FROM sense_tag
                      JOIN sense ON sense.sense_id = sense_tag.sense_parent
                      WHERE sense_tag.tag_ref = $tag
                      UNION
                      SELECT collocation.entry_parent
                      FROM collocation_tag
                      JOIN collocation ON collocation.collocation_id = collocation_tag.collocation_parent
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
            SELECT entry_id, headword, language, grasp, added_utc, updated_utc
            FROM entry
            WHERE $register = 0
               OR entry_id IN (
                      SELECT sense.entry_parent
                      FROM sense_register
                      JOIN sense ON sense.sense_id = sense_register.sense_parent
                      WHERE sense_register.register_ref = $register
                      UNION
                      SELECT collocation.entry_parent
                      FROM collocation_register
                      JOIN collocation ON collocation.collocation_id = collocation_register.collocation_parent
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

    public IReadOnlyList<LEntry> LEntrySituationFind(long situationId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(situationId);

        return LEntryOwnerFind(
            situationId,
            """
            SELECT entry_id, headword, language, grasp, added_utc, updated_utc
            FROM entry
            WHERE $owner = 0
               OR entry_id IN (
                      SELECT sense.entry_parent
                      FROM sense_situation
                      JOIN sense ON sense.sense_id = sense_situation.sense_parent
                      WHERE sense_situation.situation_ref = $owner
                      UNION
                      SELECT collocation.entry_parent
                      FROM collocation_situation
                      JOIN collocation ON collocation.collocation_id = collocation_situation.collocation_parent
                      WHERE collocation_situation.situation_ref = $owner)
            ORDER BY headword;
            """);
    }

    public IReadOnlyList<LEntry> LEntryExampleFind(long exampleId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(exampleId);

        return LEntryOwnerFind(
            exampleId,
            """
            SELECT entry_id, headword, language, grasp, added_utc, updated_utc
            FROM entry
            WHERE $owner = 0
               OR entry_id IN (
                      SELECT sense.entry_parent
                      FROM sense_example
                      JOIN sense ON sense.sense_id = sense_example.sense_parent
                      WHERE sense_example.example_ref = $owner
                      UNION
                      SELECT collocation.entry_parent
                      FROM collocation_example
                      JOIN collocation ON collocation.collocation_id = collocation_example.collocation_parent
                      WHERE collocation_example.example_ref = $owner)
            ORDER BY headword;
            """);
    }

    public IReadOnlyList<LEntry> LEntryReferenceFind(long referenceId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(referenceId);

        return LEntryOwnerFind(
            referenceId,
            """
            SELECT entry_id, headword, language, grasp, added_utc, updated_utc
            FROM entry
            WHERE $owner = 0
               OR entry_id IN (
                      SELECT sense.entry_parent
                      FROM sense_example
                      JOIN example ON example.example_id = sense_example.example_ref
                      JOIN sense ON sense.sense_id = sense_example.sense_parent
                      WHERE example.reference_ref = $owner
                      UNION
                      SELECT collocation.entry_parent
                      FROM collocation_example
                      JOIN example ON example.example_id = collocation_example.example_ref
                      JOIN collocation ON collocation.collocation_id = collocation_example.collocation_parent
                      WHERE example.reference_ref = $owner)
            ORDER BY headword;
            """);
    }

    private IReadOnlyList<LEntry> LEntryOwnerFind(long ownerId, string statement)
    {
        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = statement;
        command.Parameters.AddWithValue("$owner", ownerId);

        List<LEntry> entries = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            entries.Add(LEntryRowRead(reader));
        }

        return entries;
    }

    public long LEntryCountRead()
    {
        using LDatabaseSession session = _lEntryArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM entry;";
        return (long)command.ExecuteScalar()!;
    }
}
