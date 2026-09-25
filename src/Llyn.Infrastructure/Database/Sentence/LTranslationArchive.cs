using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LTranslationArchive : LTranslationVault
{
    private readonly LDatabase _lTranslationArchiveDatabase;

    public LTranslationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lTranslationArchiveDatabase = database;
    }

    public IReadOnlyList<LTranslation> LTranslationMeaningRead(long meaningId)
    {
        return LTranslationReferrerRead("sense_translation", "sense_parent", meaningId);
    }

    public IReadOnlyList<LTranslation> LTranslationCollocationRead(long collocationId)
    {
        return LTranslationReferrerRead("collocation_translation", "collocation_parent", collocationId);
    }

    public void LTranslationMeaningSave(long meaningId, IReadOnlyList<LTranslation> translations)
    {
        LTranslationReferrerSave("sense_translation", "sense_parent", meaningId, translations);
    }

    public void LTranslationCollocationSave(
        long collocationId, IReadOnlyList<LTranslation> translations)
    {
        LTranslationReferrerSave(
            "collocation_translation", "collocation_parent", collocationId, translations);
    }

    public IReadOnlyList<LTranslationTarget> LTranslationTargetRead(IReadOnlyList<long> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);

        List<long> wanted = [];
        HashSet<long> written = [];
        foreach (long id in ids)
        {
            if (id == 0 || !written.Add(id))
            {
                continue;
            }

            wanted.Add(id);
        }

        if (wanted.Count == 0)
        {
            return [];
        }

        using LDatabaseSession session = _lTranslationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();

        command.Parameters.AddWithValue("$ids", LDatabase.LDatabaseIdFormat(wanted));
        command.CommandText =
            """
            SELECT entry.entry_id, entry.headword, entry.language
            FROM entry
            WHERE entry.entry_id IN (SELECT value FROM json_each($ids));
            """;

        Dictionary<long, LTranslationTarget> found = [];
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                found[reader.GetInt64(0)] = new LTranslationTarget(
                    reader.GetInt64(0), reader.GetString(1), reader.GetString(2));
            }
        }

        List<LTranslationTarget> targets = [];
        foreach (long id in wanted)
        {
            if (found.TryGetValue(id, out LTranslationTarget? target))
            {
                targets.Add(target);
            }
        }

        return targets;
    }

    public IReadOnlyList<LUsage> LTranslationIncomingRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lTranslationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LUsage> usages = [];
        usages.AddRange(LTranslationIncomingRead(
            connection,
            entryId,
            LOwner.LOwnerMeaning,
            """
            SELECT link.sense_parent, sense.entry_parent, entry.headword, entry.language,
                   sense.title_state, sense.title,
                   sense.definition_state, sense.definition
            FROM sense_translation link
            JOIN sense ON sense.sense_id = link.sense_parent
            JOIN entry ON entry.entry_id = sense.entry_parent
            WHERE link.entry_ref = $id
            ORDER BY entry.headword, sense.position;
            """));
        usages.AddRange(LTranslationIncomingRead(
            connection,
            entryId,
            LOwner.LOwnerCollocation,
            """
            SELECT link.collocation_parent, collocation.entry_parent, entry.headword, entry.language,
                   collocation.title_state, collocation.title,
                   collocation.expression_state, collocation.expression
            FROM collocation_translation link
            JOIN collocation ON collocation.collocation_id = link.collocation_parent
            JOIN entry ON entry.entry_id = collocation.entry_parent
            WHERE link.entry_ref = $id
            ORDER BY entry.headword, collocation.position;
            """));

        return usages;
    }

    private static IReadOnlyList<LUsage> LTranslationIncomingRead(
        SqliteConnection connection,
        long entryId,
        LOwner owner,
        string statement)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = statement;
        command.Parameters.AddWithValue("$id", entryId);

        List<LUsage> usages = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            LStateValue title = LStateColumn.LStateColumnRead(reader, 4);
            if (title.LStateValueEmpty)
            {
                title = LStateColumn.LStateColumnRead(reader, 6);
            }

            usages.Add(new LUsage(
                reader.GetInt64(0),
                owner,
                reader.GetInt64(1),
                reader.GetString(2),
                reader.GetString(3),
                title));
        }

        return usages;
    }

    private static void LTranslationOwnerSave(
        SqliteConnection connection,
        string table,
        string column,
        long owner,
        IReadOnlyList<long> entryIds)
    {
        int position = 0;
        foreach (long entryId in entryIds)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                $"INSERT INTO {table} ({column}, entry_ref, position) VALUES ($owner, $entry, $position);";
            command.Parameters.AddWithValue("$owner", owner);
            command.Parameters.AddWithValue("$entry", entryId);
            command.Parameters.AddWithValue("$position", position);
            command.ExecuteNonQuery();
            position++;
        }
    }

    private void LTranslationReferrerSave(
        string table, string column, long referrerId, IReadOnlyList<LTranslation> translations)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);
        ArgumentNullException.ThrowIfNull(translations);

        using LDatabaseSession session = _lTranslationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand cleared = connection.CreateCommand())
        {
            cleared.CommandText = $"DELETE FROM {table} WHERE {column} = $referrer;";
            cleared.Parameters.AddWithValue("$referrer", referrerId);
            cleared.ExecuteNonQuery();
        }

        List<long> entryIds = [];
        HashSet<long> written = [];
        foreach (LTranslation translation in translations)
        {
            ArgumentNullException.ThrowIfNull(translation);
            long entryId = translation.LTranslationEntryId;
            if (entryId == 0 || !written.Add(entryId))
            {
                continue;
            }

            entryIds.Add(entryId);
        }

        LTranslationOwnerSave(connection, table, column, referrerId, entryIds);

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LTranslation> LTranslationReferrerRead(
        string table, string column, long referrerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);

        using LDatabaseSession session = _lTranslationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"SELECT entry_ref FROM {table} WHERE {column} = $referrer ORDER BY position;";
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LTranslation> translations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            translations.Add(new LTranslation(reader.GetInt64(0)));
        }

        return translations;
    }
}
