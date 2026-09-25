using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LTombstoneArchive : LTombstoneVault
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
                INSERT INTO tombstone (entry_ref, revision_ref, deleted_utc)
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
}
