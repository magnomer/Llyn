using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LReflexArchive
{
    private readonly LDatabase _lReflexArchiveDatabase;

    public LReflexArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lReflexArchiveDatabase = database;
    }

    public IReadOnlyList<LReflex> LReflexRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lReflexArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT reflex_id, position, language, kind, text, main, note, respelling
            FROM reflex WHERE entry_parent = $entry ORDER BY position, reflex_id;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        List<LReflex> rows = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new LReflex(
                reader.GetInt64(0),
                entryId,
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetInt32(5) != 0,
                reader.GetString(6),
                reader.GetString(7)));
        }

        return rows;
    }

    public IReadOnlyList<LReflex> LReflexSet(long entryId, IReadOnlyList<LReflex> reflexes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(reflexes);

        using LDatabaseSession session = _lReflexArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        HashSet<long> kept = [];
        foreach (LReflex reflex in reflexes)
        {
            ArgumentNullException.ThrowIfNull(reflex.LReflexLanguage);
            ArgumentNullException.ThrowIfNull(reflex.LReflexKind);
            ArgumentNullException.ThrowIfNull(reflex.LReflexText);
            ArgumentNullException.ThrowIfNull(reflex.LReflexNote);
            if (reflex.LReflexId > 0)
            {
                kept.Add(reflex.LReflexId);
            }
        }

        LReflexLeftoverDelete(connection, entryId, kept);

        List<LReflex> stored = new(reflexes.Count);
        for (int position = 0; position < reflexes.Count; position++)
        {
            LReflex row = reflexes[position] with
            {
                LReflexEntryId = entryId,
                LReflexPosition = position,
                LReflexLanguage = reflexes[position].LReflexLanguage.Trim(),
                LReflexKind = reflexes[position].LReflexKind.Trim(),
                LReflexText = reflexes[position].LReflexText.Trim(),
                LReflexNote = reflexes[position].LReflexNote.Trim(),
                LReflexRespelling = reflexes[position].LReflexRespelling.Trim(),
            };

            stored.Add(row.LReflexId > 0 ? LReflexRowSave(connection, row) : LReflexInsert(connection, row));
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    private static void LReflexLeftoverDelete(SqliteConnection connection, long entryId, HashSet<long> kept)
    {
        foreach (long id in LDatabaseOrder.LDatabaseOrderRead(
            connection, "reflex", "entry_parent = $owner", entryId, "reflex_id"))
        {
            if (kept.Contains(id))
            {
                continue;
            }

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM reflex WHERE reflex_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
    }

    private static LReflex LReflexRowSave(SqliteConnection connection, LReflex row)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE reflex SET position = $position, language = $language, kind = $kind, text = $text, main = $main,
                note = $note, respelling = $respelling
            WHERE reflex_id = $id AND entry_parent = $entry;
            """;
        LReflexParameterApply(command, row);
        command.Parameters.AddWithValue("$id", row.LReflexId);
        if (command.ExecuteNonQuery() == 0)
        {
            throw new InvalidOperationException(
                $"No reflex of the entry {row.LReflexEntryId} carries the id {row.LReflexId}.");
        }

        return row;
    }

    private static LReflex LReflexInsert(SqliteConnection connection, LReflex row)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO reflex (entry_parent, position, language, kind, text, main, note, respelling)
            VALUES ($entry, $position, $language, $kind, $text, $main, $note, $respelling)
            RETURNING reflex_id;
            """;
        LReflexParameterApply(command, row);
        return row with { LReflexId = (long)command.ExecuteScalar()! };
    }

    private static void LReflexParameterApply(SqliteCommand command, LReflex row)
    {
        command.Parameters.AddWithValue("$entry", row.LReflexEntryId);
        command.Parameters.AddWithValue("$position", row.LReflexPosition);
        command.Parameters.AddWithValue("$language", row.LReflexLanguage);
        command.Parameters.AddWithValue("$kind", row.LReflexKind);
        command.Parameters.AddWithValue("$text", row.LReflexText);
        command.Parameters.AddWithValue("$main", row.LReflexMain ? 1 : 0);
        command.Parameters.AddWithValue("$note", row.LReflexNote);
        command.Parameters.AddWithValue("$respelling", row.LReflexRespelling);
    }
}
