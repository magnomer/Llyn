using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LEtymologyArchive : LEtymologyVault
{
    private readonly LDatabase _lEtymologyArchiveDatabase;

    public LEtymologyArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lEtymologyArchiveDatabase = database;
    }

    public LEtymology? LEtymologyRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lEtymologyArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT etymology_id, text FROM etymology WHERE entry_parent = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);

        long id;
        string text;
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            if (!reader.Read())
            {
                return null;
            }

            id = reader.GetInt64(0);
            text = reader.GetString(1);
        }

        return new LEtymology(id, entryId, text, LEtymologyMentionRead(connection, id));
    }

    public IReadOnlyList<LEtymon> LEtymologyEtymonRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lEtymologyArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT etymon_id, position, entry_ref FROM etymon
            WHERE entry_parent = $entry
            ORDER BY position;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        List<LEtymon> etymons = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            etymons.Add(new LEtymon(reader.GetInt64(0), entryId, reader.GetInt32(1), reader.GetInt64(2)));
        }

        return etymons;
    }

    public LEtymology? LEtymologySave(long entryId, LEtymology? etymology)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lEtymologyArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string text = etymology?.LEtymologyText ?? string.Empty;
        if (etymology is null || text.Trim().Length == 0)
        {
            LEtymologyClear(connection, entryId);
            session.LDatabaseSessionCommit();
            return null;
        }

        LEtymologyValidate(text, etymology.LEtymologyMentions, entryId);
        LEtymologyClear(connection, entryId);

        long id;
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO etymology (entry_parent, text) VALUES ($entry, $text)
                RETURNING etymology_id;
                """;
            command.Parameters.AddWithValue("$entry", entryId);
            command.Parameters.AddWithValue("$text", text);
            id = (long)command.ExecuteScalar()!;
        }

        List<LMention> written = [];
        foreach (LMention mention in etymology.LEtymologyMentions)
        {
            written.Add(mention with { LMentionId = LEtymologyMentionSave(connection, id, mention) });
        }

        session.LDatabaseSessionCommit();
        return new LEtymology(id, entryId, text, written);
    }

    public IReadOnlyList<LEtymon> LEtymologyEtymonSet(long entryId, IReadOnlyList<long> targetIds)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(targetIds);

        using LDatabaseSession session = _lEtymologyArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand cleared = connection.CreateCommand())
        {
            cleared.CommandText = "DELETE FROM etymon WHERE entry_parent = $entry;";
            cleared.Parameters.AddWithValue("$entry", entryId);
            cleared.ExecuteNonQuery();
        }

        List<LEtymon> written = [];
        HashSet<long> held = [];
        foreach (long targetId in targetIds)
        {
            if (targetId <= 0 || targetId == entryId || !held.Add(targetId))
            {
                continue;
            }

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO etymon (entry_parent, position, entry_ref) VALUES ($entry, $position, $target)
                RETURNING etymon_id;
                """;
            command.Parameters.AddWithValue("$entry", entryId);
            command.Parameters.AddWithValue("$position", written.Count);
            command.Parameters.AddWithValue("$target", targetId);
            written.Add(new LEtymon((long)command.ExecuteScalar()!, entryId, written.Count, targetId));
        }

        session.LDatabaseSessionCommit();
        return written;
    }

    public IReadOnlyList<LEntry> LEtymologySourceScan(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lEtymologyArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT entry.entry_id, entry.headword, entry.language, entry.grasp, entry.added_utc, entry.updated_utc
            FROM entry
            WHERE entry.entry_id IN (SELECT entry_parent FROM etymon WHERE entry_ref = $entry)
               OR entry.entry_id IN (
                   SELECT etymology.entry_parent FROM etymology_mention
                   JOIN etymology ON etymology.etymology_id = etymology_mention.etymology_parent
                   WHERE etymology_mention.entry_ref = $entry)
            ORDER BY entry.language, entry.headword;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        List<LEntry> entries = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            entries.Add(new LEntry(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetInt32(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5)));
        }

        return entries;
    }

    private static IReadOnlyList<LMention> LEtymologyMentionRead(SqliteConnection connection, long etymologyId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT etymology_mention_id, start, length, entry_ref FROM etymology_mention
            WHERE etymology_parent = $etymology
            ORDER BY start;
            """;
        command.Parameters.AddWithValue("$etymology", etymologyId);

        List<LMention> mentions = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            mentions.Add(new LMention(
                reader.GetInt64(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt64(3)));
        }

        return mentions;
    }

    private static long LEtymologyMentionSave(SqliteConnection connection, long etymologyId, LMention mention)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO etymology_mention (etymology_parent, start, length, entry_ref)
            VALUES ($etymology, $start, $length, $entry)
            RETURNING etymology_mention_id;
            """;
        command.Parameters.AddWithValue("$etymology", etymologyId);
        command.Parameters.AddWithValue("$start", mention.LMentionOffset);
        command.Parameters.AddWithValue("$length", mention.LMentionLength);
        command.Parameters.AddWithValue("$entry", mention.LMentionEntryId);
        return (long)command.ExecuteScalar()!;
    }

    private static void LEtymologyClear(SqliteConnection connection, long entryId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM etymology WHERE entry_parent = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);
        command.ExecuteNonQuery();
    }

    private static void LEtymologyValidate(string text, IReadOnlyList<LMention> mentions, long entryId)
    {
        int length = 0;
        foreach (Rune _ in text.EnumerateRunes())
        {
            length++;
        }

        foreach (LMention mention in mentions)
        {
            if (mention.LMentionEntryId <= 0)
            {
                throw new InvalidOperationException(
                    $"A span of the etymology of Entry {entryId} names no Entry.");
            }

            if (mention.LMentionOffset < 0 || mention.LMentionLength <= 0
                || mention.LMentionOffset + mention.LMentionLength > length)
            {
                throw new InvalidOperationException(
                    $"A span at {mention.LMentionOffset} of length {mention.LMentionLength} does not fit the " +
                    $"etymology of Entry {entryId}, which holds {length} character(s).");
            }
        }

        if (LMention.LMentionOverlapCheck(mentions))
        {
            throw new InvalidOperationException($"Two spans of the etymology of Entry {entryId} overlap.");
        }
    }
}
