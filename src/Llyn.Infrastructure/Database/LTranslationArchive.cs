using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LTranslationArchive
{
    private readonly LDatabase _lTranslationArchiveDatabase;

    public LTranslationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lTranslationArchiveDatabase = database;
    }

    public IReadOnlyList<LTranslation> LTranslationMeaningRead(string meaningId)
    {
        return LTranslationReferrerRead("sense_translation", "sense_id", meaningId);
    }

    public IReadOnlyList<LTranslation> LTranslationCollocationRead(string collocationId)
    {
        return LTranslationReferrerRead("collocation_translation", "collocation_id", collocationId);
    }

    public void LTranslationMeaningSave(string meaningId, IReadOnlyList<LTranslation> translations)
    {
        LTranslationReferrerSave("sense_translation", "sense_id", meaningId, translations);
    }

    public void LTranslationCollocationSave(
        string collocationId, IReadOnlyList<LTranslation> translations)
    {
        LTranslationReferrerSave(
            "collocation_translation", "collocation_id", collocationId, translations);
    }

    public IReadOnlyList<LTranslationTarget> LTranslationTargetRead(IReadOnlyList<string> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);

        List<string> wanted = [];
        HashSet<string> written = new(StringComparer.Ordinal);
        foreach (string id in ids)
        {
            string trimmed = (id ?? string.Empty).Trim();
            if (trimmed.Length == 0 || !written.Add(trimmed))
            {
                continue;
            }

            wanted.Add(trimmed);
        }

        if (wanted.Count == 0)
        {
            return [];
        }

        using LDatabaseSession session = _lTranslationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();

        StringBuilder placeholders = new();
        for (int index = 0; index < wanted.Count; index++)
        {
            string placeholder = $"$id{index}";
            if (index > 0)
            {
                placeholders.Append(", ");
            }

            placeholders.Append(placeholder);
            command.Parameters.AddWithValue(placeholder, wanted[index]);
        }

        command.CommandText =
            $"""
            SELECT entry.id, entry.headword, entry.language
            FROM entry
            WHERE entry.id IN ({placeholders});
            """;

        Dictionary<string, LTranslationTarget> found = new(StringComparer.Ordinal);
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                found[reader.GetString(0)] = new LTranslationTarget(
                    reader.GetString(0), reader.GetString(1), reader.GetString(2));
            }
        }

        List<LTranslationTarget> targets = [];
        foreach (string id in wanted)
        {
            if (found.TryGetValue(id, out LTranslationTarget? target))
            {
                targets.Add(target);
            }
        }

        return targets;
    }

    public IReadOnlyList<LUsage> LTranslationIncomingRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lTranslationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LUsage> usages = [];
        usages.AddRange(LTranslationIncomingRead(
            connection,
            entryId,
            LOwner.LOwnerMeaning,
            """
            SELECT link.sense_id, sense.entry_id, entry.headword, entry.language,
                   sense.title_state, sense.title,
                   CASE WHEN sense.gloss IS NOT NULL THEN 'specified' ELSE sense.definition_state END,
                   COALESCE(sense.gloss, sense.definition)
            FROM sense_translation link
            JOIN sense ON sense.id = link.sense_id
            JOIN entry ON entry.id = sense.entry_id
            WHERE link.entry_id = $id
            ORDER BY entry.headword, sense.position;
            """));
        usages.AddRange(LTranslationIncomingRead(
            connection,
            entryId,
            LOwner.LOwnerCollocation,
            """
            SELECT link.collocation_id, collocation.entry_id, entry.headword, entry.language,
                   collocation.title_state, collocation.title,
                   collocation.expression_state, collocation.expression
            FROM collocation_translation link
            JOIN collocation ON collocation.id = link.collocation_id
            JOIN entry ON entry.id = collocation.entry_id
            WHERE link.entry_id = $id
            ORDER BY entry.headword, collocation.position;
            """));

        return usages;
    }

    private static IReadOnlyList<LUsage> LTranslationIncomingRead(
        SqliteConnection connection,
        string entryId,
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
                reader.GetString(0),
                owner,
                reader.GetString(1),
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
        string owner,
        IReadOnlyList<string> entryIds)
    {
        int position = 0;
        foreach (string entryId in entryIds)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                $"INSERT INTO {table} ({column}, entry_id, position) VALUES ($owner, $entry, $position);";
            command.Parameters.AddWithValue("$owner", owner);
            command.Parameters.AddWithValue("$entry", entryId);
            command.Parameters.AddWithValue("$position", position);
            command.ExecuteNonQuery();
            position++;
        }
    }

    private void LTranslationReferrerSave(
        string table, string column, string referrerId, IReadOnlyList<LTranslation> translations)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentNullException.ThrowIfNull(translations);

        using LDatabaseSession session = _lTranslationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand cleared = connection.CreateCommand())
        {
            cleared.CommandText = $"DELETE FROM {table} WHERE {column} = $referrer;";
            cleared.Parameters.AddWithValue("$referrer", referrerId);
            cleared.ExecuteNonQuery();
        }

        List<string> entryIds = [];
        HashSet<string> written = new(StringComparer.Ordinal);
        foreach (LTranslation translation in translations)
        {
            ArgumentNullException.ThrowIfNull(translation);
            string entryId = translation.LTranslationEntryId.Trim();
            if (entryId.Length == 0 || !written.Add(entryId))
            {
                continue;
            }

            entryIds.Add(entryId);
        }

        LTranslationOwnerSave(connection, table, column, referrerId, entryIds);

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LTranslation> LTranslationReferrerRead(
        string table, string column, string referrerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);

        using LDatabaseSession session = _lTranslationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"SELECT entry_id, position FROM {table} WHERE {column} = $referrer ORDER BY position;";
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LTranslation> translations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            translations.Add(new LTranslation(reader.GetString(0), reader.GetInt32(1)));
        }

        return translations;
    }
}
