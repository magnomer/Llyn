using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LTombstoneArchive
{
    private readonly LDatabase _lTombstoneArchiveDatabase;

    public LTombstoneArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lTombstoneArchiveDatabase = database;
    }

    public LTombstone LTombstoneRecord(long entryId, long revisionId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(revisionId);

        LTombstone stored = new(
            entryId,
            revisionId,
            DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));

        using LDatabaseSession session = _lTombstoneArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO tombstone (entry_id, revision_ref, deleted_utc)
                VALUES ($entry, $revision, $deleted);
                """;
            command.Parameters.AddWithValue("$entry", stored.LTombstoneEntryId);
            command.Parameters.AddWithValue("$revision", stored.LTombstoneRevisionId);
            command.Parameters.AddWithValue("$deleted", stored.LTombstoneDeletedUtc);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public LTombstone? LTombstoneRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lTombstoneArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            "SELECT entry_id, revision_ref, deleted_utc FROM tombstone WHERE entry_id = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LTombstone(reader.GetInt64(0), reader.GetInt64(1), reader.GetString(2));
    }

    public IReadOnlyList<LTombstone> LTombstoneRevisionRead(long revisionId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(revisionId);

        using LDatabaseSession session = _lTombstoneArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT entry_id, revision_ref, deleted_utc
            FROM tombstone WHERE revision_ref = $revision ORDER BY deleted_utc, entry_id;
            """;
        command.Parameters.AddWithValue("$revision", revisionId);

        List<LTombstone> tombstones = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            tombstones.Add(new LTombstone(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetString(2)));
        }

        return tombstones;
    }
}
