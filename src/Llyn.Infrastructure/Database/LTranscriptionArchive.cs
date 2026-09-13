using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LTranscriptionArchive
{
    private const long LTranscriptionArchiveShift = 1_000_000_000L;

    private readonly LDatabase _lTranscriptionArchiveDatabase;

    public LTranscriptionArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lTranscriptionArchiveDatabase = database;
    }

    public IReadOnlyList<LTranscription> LTranscriptionRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lTranscriptionArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT transcription_id, position, scheme, text
            FROM transcription WHERE entry_parent = $entry ORDER BY position;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        List<LTranscription> rows = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new LTranscription(
                reader.GetInt64(0),
                entryId,
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetString(3)));
        }

        return rows;
    }

    public IReadOnlyList<LTranscription> LTranscriptionSet(long entryId, IReadOnlyList<LTranscription> transcriptions)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(transcriptions);

        using LDatabaseSession session = _lTranscriptionArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        HashSet<string> schemes = new(StringComparer.Ordinal);
        HashSet<long> kept = [];
        foreach (LTranscription transcription in transcriptions)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(transcription.LTranscriptionScheme);
            ArgumentNullException.ThrowIfNull(transcription.LTranscriptionText);
            if (!schemes.Add(transcription.LTranscriptionScheme.Trim()))
            {
                throw new InvalidOperationException(
                    $"The entry {entryId} was given two transcriptions in the scheme " +
                    $"{transcription.LTranscriptionScheme}.");
            }

            if (transcription.LTranscriptionId > 0)
            {
                kept.Add(transcription.LTranscriptionId);
            }
        }

        LTranscriptionLeftoverDelete(connection, entryId, kept);
        LTranscriptionAsideMove(connection, entryId);

        List<LTranscription> stored = new(transcriptions.Count);
        for (int position = 0; position < transcriptions.Count; position++)
        {
            LTranscription row = transcriptions[position] with
            {
                LTranscriptionEntryId = entryId,
                LTranscriptionPosition = position,
                LTranscriptionScheme = transcriptions[position].LTranscriptionScheme.Trim(),
                LTranscriptionText = transcriptions[position].LTranscriptionText.Trim(),
            };

            stored.Add(row.LTranscriptionId > 0
                ? LTranscriptionRowSave(connection, row)
                : LTranscriptionInsert(connection, row));
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    private static void LTranscriptionLeftoverDelete(SqliteConnection connection, long entryId, HashSet<long> kept)
    {
        foreach (long id in LDatabaseOrder.LDatabaseOrderRead(
            connection, "transcription", "entry_parent = $owner", entryId, "transcription_id"))
        {
            if (kept.Contains(id))
            {
                continue;
            }

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM transcription WHERE transcription_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
    }

    private static void LTranscriptionAsideMove(SqliteConnection connection, long entryId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE transcription
            SET position = position + $shift, scheme = char(0) || scheme
            WHERE entry_parent = $entry;
            """;
        command.Parameters.AddWithValue("$shift", LTranscriptionArchiveShift);
        command.Parameters.AddWithValue("$entry", entryId);
        command.ExecuteNonQuery();
    }

    private static LTranscription LTranscriptionRowSave(SqliteConnection connection, LTranscription row)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE transcription SET position = $position, scheme = $scheme, text = $text
            WHERE transcription_id = $id AND entry_parent = $entry;
            """;
        command.Parameters.AddWithValue("$position", row.LTranscriptionPosition);
        command.Parameters.AddWithValue("$scheme", row.LTranscriptionScheme);
        command.Parameters.AddWithValue("$text", row.LTranscriptionText);
        command.Parameters.AddWithValue("$id", row.LTranscriptionId);
        command.Parameters.AddWithValue("$entry", row.LTranscriptionEntryId);
        if (command.ExecuteNonQuery() == 0)
        {
            throw new InvalidOperationException(
                $"No transcription of the entry {row.LTranscriptionEntryId} carries the id {row.LTranscriptionId}.");
        }

        return row;
    }

    private static LTranscription LTranscriptionInsert(SqliteConnection connection, LTranscription row)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO transcription (entry_parent, position, scheme, text)
            VALUES ($entry, $position, $scheme, $text)
            RETURNING transcription_id;
            """;
        command.Parameters.AddWithValue("$entry", row.LTranscriptionEntryId);
        command.Parameters.AddWithValue("$position", row.LTranscriptionPosition);
        command.Parameters.AddWithValue("$scheme", row.LTranscriptionScheme);
        command.Parameters.AddWithValue("$text", row.LTranscriptionText);
        return row with { LTranscriptionId = (long)command.ExecuteScalar()! };
    }
}
