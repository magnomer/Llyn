using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LCueArchive : LCueVault
{
    private const string LCueColumns =
        """
        cue_id, deck_parent, position, grain, entry_ref, sense_ref, collocation_ref,
        example_ref, target_ref, flag, excluded, text1, text2, added_utc
        """;

    private readonly LDatabase _lCueArchiveDatabase;

    public LCueArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lCueArchiveDatabase = database;
    }

    public IReadOnlyList<LCue> LCueCreate(long deckId, IReadOnlyList<LCue> cues)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deckId);
        ArgumentNullException.ThrowIfNull(cues);

        string now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

        using LDatabaseSession session = _lCueArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        int next = LCueSiblingRead(connection, deckId).Count;

        List<LCue> created = [];
        foreach (LCue cue in cues)
        {
            LCue stored = cue with { LCueDeckId = deckId, LCuePosition = next++, LCueAddedUtc = now };

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO cue (
                    deck_parent, position, grain, entry_ref, sense_ref, collocation_ref,
                    example_ref, target_ref, flag, excluded, text1, text2, added_utc)
                VALUES (
                    $deck, $position, $grain, $entry, $sense, $collocation,
                    $example, $target, $flag, $excluded, $text1, $text2, $added)
                RETURNING cue_id;
                """;
            command.Parameters.AddWithValue("$deck", stored.LCueDeckId);
            command.Parameters.AddWithValue("$position", stored.LCuePosition);
            command.Parameters.AddWithValue("$grain", (int)stored.LCueGrain);
            command.Parameters.AddWithValue("$entry", (object?)stored.LCueEntryId ?? DBNull.Value);
            command.Parameters.AddWithValue("$sense", (object?)stored.LCueMeaningId ?? DBNull.Value);
            command.Parameters.AddWithValue("$collocation", (object?)stored.LCueCollocationId ?? DBNull.Value);
            command.Parameters.AddWithValue("$example", (object?)stored.LCueExampleId ?? DBNull.Value);
            command.Parameters.AddWithValue("$target", (object?)stored.LCueTargetId ?? DBNull.Value);
            command.Parameters.AddWithValue("$flag", stored.LCueFlag);
            command.Parameters.AddWithValue("$excluded", stored.LCueExcluded ? 1 : 0);
            command.Parameters.AddWithValue("$text1", stored.LCueText1);
            command.Parameters.AddWithValue("$text2", stored.LCueText2);
            command.Parameters.AddWithValue("$added", stored.LCueAddedUtc);
            created.Add(stored with { LCueId = (long)command.ExecuteScalar()! });
        }

        session.LDatabaseSessionCommit();
        return created;
    }

    public IReadOnlyList<LCue> LCueRead()
    {
        using LDatabaseSession session = _lCueArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = $"SELECT {LCueColumns} FROM cue ORDER BY deck_parent, position;";

        List<LCue> cues = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            cues.Add(LCueRowRead(reader));
        }

        return cues;
    }

    public IReadOnlyList<LCue> LCueRead(long deckId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deckId);

        using LDatabaseSession session = _lCueArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = $"SELECT {LCueColumns} FROM cue WHERE deck_parent = $deck ORDER BY position;";
        command.Parameters.AddWithValue("$deck", deckId);

        List<LCue> cues = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            cues.Add(LCueRowRead(reader));
        }

        return cues;
    }

    public long? LCueHolderRead(LCue cue)
    {
        ArgumentNullException.ThrowIfNull(cue);

        using LDatabaseSession session = _lCueArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT COALESCE(
                (SELECT entry_id FROM entry WHERE entry_id = $entry),
                (SELECT entry_parent FROM sense WHERE sense_id = $sense),
                (SELECT entry_parent FROM collocation WHERE collocation_id = $collocation),
                (SELECT MIN(holder) FROM (
                    SELECT sense.entry_parent AS holder
                    FROM sense_example JOIN sense ON sense.sense_id = sense_example.sense_parent
                    WHERE sense_example.example_ref = $example
                    UNION ALL
                    SELECT collocation.entry_parent
                    FROM collocation_example
                    JOIN collocation ON collocation.collocation_id = collocation_example.collocation_parent
                    WHERE collocation_example.example_ref = $example)));
            """;
        command.Parameters.AddWithValue("$entry", (object?)cue.LCueEntryId ?? DBNull.Value);
        command.Parameters.AddWithValue("$sense", (object?)cue.LCueMeaningId ?? DBNull.Value);
        command.Parameters.AddWithValue("$collocation", (object?)cue.LCueCollocationId ?? DBNull.Value);
        command.Parameters.AddWithValue("$example", (object?)cue.LCueExampleId ?? DBNull.Value);
        return command.ExecuteScalar() is long holder ? holder : null;
    }

    public void LCueUpdate(LCue cue)
    {
        ArgumentNullException.ThrowIfNull(cue);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cue.LCueId);

        using LDatabaseSession session = _lCueArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE cue
                SET flag = $flag, excluded = $excluded, text1 = $text1, text2 = $text2
                WHERE cue_id = $id;
                """;
            command.Parameters.AddWithValue("$flag", cue.LCueFlag);
            command.Parameters.AddWithValue("$excluded", cue.LCueExcluded ? 1 : 0);
            command.Parameters.AddWithValue("$text1", cue.LCueText1);
            command.Parameters.AddWithValue("$text2", cue.LCueText2);
            command.Parameters.AddWithValue("$id", cue.LCueId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No cue carries the id '{cue.LCueId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LCueOrderSet(long deckId, IReadOnlyList<long> order)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deckId);
        ArgumentNullException.ThrowIfNull(order);

        using LDatabaseSession session = _lCueArchiveDatabase.LDatabaseSessionStart();
        LCueSiblingNormalize(session.LDatabaseSessionConnection, deckId, order);
        session.LDatabaseSessionCommit();
    }

    public void LCueDelete(IReadOnlyList<long> cueIds)
    {
        ArgumentNullException.ThrowIfNull(cueIds);

        using LDatabaseSession session = _lCueArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        HashSet<long> decks = [];
        foreach (long cueId in cueIds)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM cue WHERE cue_id = $id RETURNING deck_parent;";
            command.Parameters.AddWithValue("$id", cueId);
            if (command.ExecuteScalar() is long deckId)
            {
                decks.Add(deckId);
            }
        }

        foreach (long deckId in decks)
        {
            LCueSiblingNormalize(connection, deckId, LCueSiblingRead(connection, deckId));
        }

        session.LDatabaseSessionCommit();
    }

    public IReadOnlyList<LCueCell> LCueCellRead(long cueId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cueId);

        using LDatabaseSession session = _lCueArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT column_kind, column_index, value
            FROM cue_cell WHERE cue_parent = $cue
            ORDER BY column_kind, column_index;
            """;
        command.Parameters.AddWithValue("$cue", cueId);

        List<LCueCell> cells = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            cells.Add(new LCueCell((LCueColumn)reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2)));
        }

        return cells;
    }

    public void LCueCellSave(long cueId, IReadOnlyList<LCueCell> cells)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cueId);
        ArgumentNullException.ThrowIfNull(cells);

        using LDatabaseSession session = _lCueArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand cleared = connection.CreateCommand())
        {
            cleared.CommandText = "DELETE FROM cue_cell WHERE cue_parent = $cue;";
            cleared.Parameters.AddWithValue("$cue", cueId);
            cleared.ExecuteNonQuery();
        }

        foreach (LCueCell cell in cells)
        {
            LCueCellInsert(connection, cueId, cell);
        }

        session.LDatabaseSessionCommit();
    }

    private static void LCueCellInsert(SqliteConnection connection, long cueId, LCueCell cell)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO cue_cell (cue_parent, column_kind, column_index, value)
            VALUES ($cue, $kind, $index, $value);
            """;
        command.Parameters.AddWithValue("$cue", cueId);
        command.Parameters.AddWithValue("$kind", (int)cell.LCueCellColumn);
        command.Parameters.AddWithValue("$index", cell.LCueCellIndex);
        command.Parameters.AddWithValue("$value", cell.LCueCellValue);
        command.ExecuteNonQuery();
    }

    private static IReadOnlyList<long> LCueSiblingRead(SqliteConnection connection, long deckId) =>
        LDatabaseOrder.LDatabaseOrderRead(connection, "cue", "deck_parent = $owner", deckId, "cue_id");

    private static void LCueSiblingNormalize(SqliteConnection connection, long deckId, IReadOnlyList<long> order)
    {
        LDatabaseOrder.LDatabaseOrderNormalize(connection, "cue", "deck_parent = $owner", deckId, "cue_id", order);
    }

    private static LCue LCueRowRead(SqliteDataReader reader) =>
        new(
            reader.GetInt64(0),
            reader.GetInt64(1),
            reader.GetInt32(2),
            (LGrain)reader.GetInt32(3),
            reader.IsDBNull(4) ? null : reader.GetInt64(4),
            reader.IsDBNull(5) ? null : reader.GetInt64(5),
            reader.IsDBNull(6) ? null : reader.GetInt64(6),
            reader.IsDBNull(7) ? null : reader.GetInt64(7),
            reader.IsDBNull(8) ? null : reader.GetInt64(8),
            reader.GetInt32(9),
            reader.GetInt32(10) != 0,
            reader.GetString(11),
            reader.GetString(12),
            reader.GetString(13));
}
