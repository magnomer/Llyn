using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LDeckArchive : LDeckVault
{
    private const string LDeckColumns =
        """
        deck_id, folder_ref, position, name, grain, stencil_ref, side_first, study_order,
        round_goal, round_spaced, set_min_goal, new_pool, target_streak, target_hours,
        deck_code, basic, synced_utc, added_utc, updated_utc
        """;

    private readonly LDatabase _lDeckArchiveDatabase;

    public LDeckArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lDeckArchiveDatabase = database;
    }

    public LDeck LDeckCreate(LDeck deck)
    {
        ArgumentNullException.ThrowIfNull(deck);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deck.LDeckStencilId);

        string now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

        using LDatabaseSession session = _lDeckArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LDeck stored = deck with
        {
            LDeckPosition = LDeckSiblingRead(connection, deck.LDeckFolderId).Count,
            LDeckAddedUtc = now,
            LDeckUpdatedUtc = now,
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO deck (
                    folder_ref, position, name, grain, stencil_ref, side_first, study_order,
                    round_goal, round_spaced, set_min_goal, new_pool, target_streak, target_hours,
                    deck_code, basic, synced_utc, added_utc, updated_utc)
                VALUES (
                    $folder, $position, $name, $grain, $stencil, $lead, $order,
                    $goal, $spaced, $quorum, $pool, $streak, $target,
                    $code, $basic, $synced, $added, $updated)
                RETURNING deck_id;
                """;
            LDeckValueApply(command, stored);
            command.Parameters.AddWithValue("$grain", (int)stored.LDeckGrain);
            command.Parameters.AddWithValue("$stencil", stored.LDeckStencilId);
            command.Parameters.AddWithValue("$added", stored.LDeckAddedUtc);
            stored = stored with { LDeckId = (long)command.ExecuteScalar()! };
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LDeck> LDeckRead()
    {
        using LDatabaseSession session = _lDeckArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = $"SELECT {LDeckColumns} FROM deck ORDER BY ifnull(folder_ref, 0), position;";

        List<LDeck> decks = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            decks.Add(LDeckRowRead(reader));
        }

        return decks;
    }

    public LDeck? LDeckRead(long deckId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deckId);

        using LDatabaseSession session = _lDeckArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = $"SELECT {LDeckColumns} FROM deck WHERE deck_id = $id;";
        command.Parameters.AddWithValue("$id", deckId);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LDeckRowRead(reader) : null;
    }

    public void LDeckUpdate(LDeck deck)
    {
        ArgumentNullException.ThrowIfNull(deck);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deck.LDeckId);

        LDeck stored = deck with
        {
            LDeckUpdatedUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
        };

        using LDatabaseSession session = _lDeckArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE deck
                SET folder_ref = $folder, position = $position, name = $name, side_first = $lead,
                    study_order = $order, round_goal = $goal, round_spaced = $spaced, set_min_goal = $quorum,
                    new_pool = $pool, target_streak = $streak, target_hours = $target,
                    deck_code = $code, basic = $basic, synced_utc = $synced, updated_utc = $updated
                WHERE deck_id = $id;
                """;
            LDeckValueApply(command, stored);
            command.Parameters.AddWithValue("$id", stored.LDeckId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No deck carries the id '{stored.LDeckId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LDeckOrderSet(long? folderId, IReadOnlyList<long> order)
    {
        ArgumentNullException.ThrowIfNull(order);

        using LDatabaseSession session = _lDeckArchiveDatabase.LDatabaseSessionStart();
        LDeckSiblingNormalize(session.LDatabaseSessionConnection, folderId, order);
        session.LDatabaseSessionCommit();
    }

    public void LDeckDelete(long deckId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deckId);

        using LDatabaseSession session = _lDeckArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LDeck? held;
        using (SqliteCommand read = connection.CreateCommand())
        {
            read.CommandText = $"SELECT {LDeckColumns} FROM deck WHERE deck_id = $id;";
            read.Parameters.AddWithValue("$id", deckId);
            using SqliteDataReader reader = read.ExecuteReader();
            held = reader.Read() ? LDeckRowRead(reader) : null;
        }

        if (held is null)
        {
            return;
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                DELETE FROM deck WHERE deck_id = $id;
                DELETE FROM stencil WHERE stencil_id = $stencil AND saved = 0;
                """;
            command.Parameters.AddWithValue("$id", deckId);
            command.Parameters.AddWithValue("$stencil", held.LDeckStencilId);
            command.ExecuteNonQuery();
        }

        LDeckSiblingNormalize(connection, held.LDeckFolderId, LDeckSiblingRead(connection, held.LDeckFolderId));

        session.LDatabaseSessionCommit();
    }

    public IReadOnlyList<LBearing> LBearingRead(long deckId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deckId);

        using LDatabaseSession session = _lDeckArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT direction, front_sides, back_sides, statistics_set
            FROM bearing WHERE deck_parent = $deck
            ORDER BY direction;
            """;
        command.Parameters.AddWithValue("$deck", deckId);

        List<LBearing> bearings = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            bearings.Add(new LBearing(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3)));
        }

        return bearings;
    }

    public void LBearingSave(long deckId, IReadOnlyList<LBearing> bearings)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deckId);
        ArgumentNullException.ThrowIfNull(bearings);

        using LDatabaseSession session = _lDeckArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand cleared = connection.CreateCommand())
        {
            cleared.CommandText = "DELETE FROM bearing WHERE deck_parent = $deck;";
            cleared.Parameters.AddWithValue("$deck", deckId);
            cleared.ExecuteNonQuery();
        }

        foreach (LBearing bearing in bearings)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO bearing (deck_parent, direction, front_sides, back_sides, statistics_set)
                VALUES ($deck, $direction, $front, $back, $statistics);
                """;
            command.Parameters.AddWithValue("$deck", deckId);
            command.Parameters.AddWithValue("$direction", bearing.LBearingDirection);
            command.Parameters.AddWithValue("$front", bearing.LBearingFront);
            command.Parameters.AddWithValue("$back", bearing.LBearingBack);
            command.Parameters.AddWithValue("$statistics", bearing.LBearingStatistics);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    private static IReadOnlyList<long> LDeckSiblingRead(SqliteConnection connection, long? folderId)
    {
        return folderId is null
            ? LDatabaseOrder.LDatabaseOrderRead(connection, "deck", "folder_ref IS NULL", null, "deck_id")
            : LDatabaseOrder.LDatabaseOrderRead(connection, "deck", "folder_ref = $owner", folderId, "deck_id");
    }

    private static void LDeckSiblingNormalize(SqliteConnection connection, long? folderId, IReadOnlyList<long> order)
    {
        if (folderId is null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(connection, "deck", "folder_ref IS NULL", null, "deck_id", order);
            return;
        }

        LDatabaseOrder.LDatabaseOrderNormalize(connection, "deck", "folder_ref = $owner", folderId, "deck_id", order);
    }

    private static void LDeckValueApply(SqliteCommand command, LDeck deck)
    {
        LDeckSchedule schedule = deck.LDeckSchedule;
        command.Parameters.AddWithValue("$folder", (object?)deck.LDeckFolderId ?? DBNull.Value);
        command.Parameters.AddWithValue("$position", deck.LDeckPosition);
        command.Parameters.AddWithValue("$name", deck.LDeckName);
        command.Parameters.AddWithValue("$lead", (int)deck.LDeckLead);
        command.Parameters.AddWithValue("$order", (int)schedule.LDeckScheduleOrder);
        command.Parameters.AddWithValue("$goal", schedule.LDeckScheduleGoal);
        command.Parameters.AddWithValue("$spaced", schedule.LDeckScheduleSpaced);
        command.Parameters.AddWithValue("$quorum", (object?)schedule.LDeckScheduleQuorum ?? DBNull.Value);
        command.Parameters.AddWithValue("$pool", schedule.LDeckSchedulePool);
        command.Parameters.AddWithValue("$streak", schedule.LDeckScheduleStreak);
        command.Parameters.AddWithValue("$target", schedule.LDeckScheduleTarget);
        command.Parameters.AddWithValue("$code", deck.LDeckCode);
        command.Parameters.AddWithValue("$basic", deck.LDeckBasic ? 1 : 0);
        command.Parameters.AddWithValue("$synced", (object?)deck.LDeckSyncedUtc ?? DBNull.Value);
        command.Parameters.AddWithValue("$updated", deck.LDeckUpdatedUtc);
    }

    private static LDeck LDeckRowRead(SqliteDataReader reader)
    {
        LDeckSchedule schedule = new(
            (LAlmanacOrder)reader.GetInt32(7),
            reader.GetInt32(8),
            reader.GetInt32(9),
            reader.GetInt32(11),
            reader.GetInt32(12),
            reader.GetInt32(13),
            reader.IsDBNull(10) ? null : reader.GetInt32(10));

        return new LDeck(
            reader.GetInt64(0),
            reader.IsDBNull(1) ? null : reader.GetInt64(1),
            reader.GetInt32(2),
            reader.GetString(3),
            (LGrain)reader.GetInt32(4),
            reader.GetInt64(5),
            (LBearingLead)reader.GetInt32(6),
            schedule,
            reader.GetString(14),
            reader.GetInt32(15) != 0,
            reader.IsDBNull(16) ? null : reader.GetString(16),
            reader.GetString(17),
            reader.GetString(18));
    }
}
