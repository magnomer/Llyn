using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LMentionArchive : LMentionVault
{
    private const long LMentionShelf = 1_000_000_000L;

    private readonly LDatabase _lMentionArchiveDatabase;

    public LMentionArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lMentionArchiveDatabase = database;
    }

    public IReadOnlyList<LMention> LMentionExampleRead(long exampleId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);

        using LDatabaseSession session = _lMentionArchiveDatabase.LDatabaseSessionStart();
        IReadOnlyDictionary<long, IReadOnlyList<LMention>> read =
            LMentionExampleRead(session.LDatabaseSessionConnection, [exampleId]);
        return read.TryGetValue(exampleId, out IReadOnlyList<LMention>? mentions) ? mentions : [];
    }

    public IReadOnlyList<long> LMentionExampleSave(long exampleId, IReadOnlyList<LMention> mentions)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);
        ArgumentNullException.ThrowIfNull(mentions);

        using LDatabaseSession session = _lMentionArchiveDatabase.LDatabaseSessionStart();
        IReadOnlyList<long> written = LMentionExampleSave(session.LDatabaseSessionConnection, exampleId, mentions);
        session.LDatabaseSessionCommit();
        return written;
    }

    public void LMentionExampleCopy(long fromId, long toId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fromId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(toId);

        using LDatabaseSession session = _lMentionArchiveDatabase.LDatabaseSessionStart();
        LMentionExampleCopy(session.LDatabaseSessionConnection, fromId, toId);
        session.LDatabaseSessionCommit();
    }

    public IReadOnlyList<long> LMentionSenseClear(long meaningId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(meaningId);

        using LDatabaseSession session = _lMentionArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            UPDATE example_mention SET sense_ref = NULL
            WHERE sense_ref = $sense
            RETURNING example_parent;
            """;
        command.Parameters.AddWithValue("$sense", meaningId);

        List<long> affected = [];
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                long exampleId = reader.GetInt64(0);
                if (!affected.Contains(exampleId))
                {
                    affected.Add(exampleId);
                }
            }
        }

        session.LDatabaseSessionCommit();
        return affected;
    }

    public bool LMentionSenseSet(long mentionId, long senseId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(mentionId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(senseId);

        using LDatabaseSession session = _lMentionArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            UPDATE example_mention SET sense_ref = $sense
            WHERE example_mention_id = $id AND entry_ref IS NOT NULL;
            """;
        command.Parameters.AddWithValue("$id", mentionId);
        command.Parameters.AddWithValue("$sense", senseId);
        bool changed = command.ExecuteNonQuery() == 1;
        session.LDatabaseSessionCommit();
        return changed;
    }

    internal static IReadOnlyDictionary<long, IReadOnlyList<LMention>> LMentionExampleRead(
        SqliteConnection connection, IReadOnlyList<long> exampleIds)
    {
        Dictionary<long, List<LMention>> read = [];
        if (exampleIds.Count == 0)
        {
            return read.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<LMention>)pair.Value);
        }

        using SqliteCommand command = connection.CreateCommand();
        command.Parameters.AddWithValue("$examples", LDatabase.LDatabaseIdFormat(exampleIds));

        command.CommandText =
            $"""
            SELECT example_mention_id, example_parent, start, length, entry_ref, sense_ref
            FROM example_mention
            WHERE example_parent IN (SELECT value FROM json_each($examples))
            ORDER BY example_parent, start;
            """;

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            long exampleId = reader.GetInt64(1);
            if (!read.TryGetValue(exampleId, out List<LMention>? mentions))
            {
                mentions = [];
                read[exampleId] = mentions;
            }

            mentions.Add(new LMention(
                reader.GetInt64(0),
                reader.GetInt32(2),
                reader.GetInt32(3),
                reader.IsDBNull(4) ? 0 : reader.GetInt64(4),
                reader.IsDBNull(5) ? 0 : reader.GetInt64(5)));
        }

        return read.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<LMention>)pair.Value);
    }

    internal static IReadOnlyList<long> LMentionExampleSave(
        SqliteConnection connection, long exampleId, IReadOnlyList<LMention> mentions)
    {
        int length = LMentionTextRead(connection, exampleId);
        foreach (LMention mention in mentions)
        {
            if (mention.LMentionOffset < 0 || mention.LMentionLength <= 0
                || mention.LMentionOffset + mention.LMentionLength > length)
            {
                throw new InvalidOperationException(
                    $"Mention at {mention.LMentionOffset} of length {mention.LMentionLength} " +
                    $"does not fit the text of Example {exampleId}, which holds {length} character(s).");
            }
        }

        if (LMention.LMentionOverlapCheck(mentions))
        {
            LMention clash = LMentionClashRead(mentions);
            throw new InvalidOperationException(
                $"Mention at {clash.LMentionOffset} of length {clash.LMentionLength} " +
                $"overlaps another Mention of Example {exampleId}.");
        }

        LMentionOffsetAdjust(connection, exampleId);
        List<long> written = new(mentions.Count);
        foreach (LMention mention in mentions)
        {
            bool kept = mention.LMentionId > 0
                && !written.Contains(mention.LMentionId)
                && LMentionChange(connection, exampleId, mention);
            written.Add(kept ? mention.LMentionId : LMentionSave(connection, exampleId, mention));
        }

        LMentionShelfClear(connection, exampleId);
        return written;
    }

    internal static void LMentionExampleSweep(SqliteConnection connection, long exampleId)
    {
        int length = LMentionTextRead(connection, exampleId);
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "DELETE FROM example_mention WHERE example_parent = $example AND start + length > $length;";
        command.Parameters.AddWithValue("$example", exampleId);
        command.Parameters.AddWithValue("$length", length);
        command.ExecuteNonQuery();
    }

    internal static void LMentionExampleCopy(SqliteConnection connection, long fromId, long toId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO example_mention (example_parent, start, length, entry_ref, sense_ref)
            SELECT $to, start, length, entry_ref, sense_ref
            FROM example_mention
            WHERE example_parent = $from
            ORDER BY start;
            """;
        command.Parameters.AddWithValue("$from", fromId);
        command.Parameters.AddWithValue("$to", toId);
        command.ExecuteNonQuery();
    }

    internal static void LMentionExampleClear(SqliteConnection connection, long exampleId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM example_mention WHERE example_parent = $example;";
        command.Parameters.AddWithValue("$example", exampleId);
        command.ExecuteNonQuery();
    }

    private static LMention LMentionClashRead(IReadOnlyList<LMention> mentions)
    {
        IReadOnlyList<LMention> sorted = LMention.LMentionSort(mentions);
        for (int index = 1; index < sorted.Count; index++)
        {
            LMention previous = sorted[index - 1];
            if (sorted[index].LMentionOffset < previous.LMentionOffset + previous.LMentionLength)
            {
                return sorted[index];
            }
        }

        return sorted[0];
    }

    private static int LMentionTextRead(SqliteConnection connection, long exampleId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT text FROM example WHERE example_id = $example;";
        command.Parameters.AddWithValue("$example", exampleId);
        object? text = command.ExecuteScalar();
        if (text is null)
        {
            throw new InvalidOperationException($"No Example carries the id '{exampleId}'.");
        }

        if (text is not string stored)
        {
            return 0;
        }

        int length = 0;
        foreach (Rune _ in stored.EnumerateRunes())
        {
            length++;
        }

        return length;
    }

    private static void LMentionOffsetAdjust(SqliteConnection connection, long exampleId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "UPDATE example_mention SET start = start + $shift WHERE example_parent = $example;";
        command.Parameters.AddWithValue("$shift", LMentionShelf);
        command.Parameters.AddWithValue("$example", exampleId);
        command.ExecuteNonQuery();
    }

    private static void LMentionShelfClear(SqliteConnection connection, long exampleId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "DELETE FROM example_mention WHERE example_parent = $example AND start >= $shift;";
        command.Parameters.AddWithValue("$shift", LMentionShelf);
        command.Parameters.AddWithValue("$example", exampleId);
        command.ExecuteNonQuery();
    }

    private static bool LMentionChange(SqliteConnection connection, long exampleId, LMention mention)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE example_mention
            SET start = $start, length = $length, entry_ref = $entry, sense_ref = $sense
            WHERE example_mention_id = $id AND example_parent = $example;
            """;
        command.Parameters.AddWithValue("$id", mention.LMentionId);
        command.Parameters.AddWithValue("$example", exampleId);
        LMentionParameterApply(command, mention);
        return command.ExecuteNonQuery() == 1;
    }

    private static long LMentionSave(SqliteConnection connection, long exampleId, LMention mention)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO example_mention (example_parent, start, length, entry_ref, sense_ref)
            VALUES ($example, $start, $length, $entry, $sense)
            RETURNING example_mention_id;
            """;
        command.Parameters.AddWithValue("$example", exampleId);
        LMentionParameterApply(command, mention);
        return (long)command.ExecuteScalar()!;
    }

    private static void LMentionParameterApply(SqliteCommand command, LMention mention)
    {
        command.Parameters.AddWithValue("$start", mention.LMentionOffset);
        command.Parameters.AddWithValue("$length", mention.LMentionLength);
        command.Parameters.AddWithValue(
            "$entry", mention.LMentionEntryId > 0 ? mention.LMentionEntryId : DBNull.Value);
        command.Parameters.AddWithValue(
            "$sense", mention.LMentionSenseId > 0 ? mention.LMentionSenseId : DBNull.Value);
    }
}
