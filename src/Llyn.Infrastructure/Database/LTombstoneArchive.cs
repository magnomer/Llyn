using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists the record that an Entry was deleted. A tombstone is written after the Entry row is gone
/// and names it as recorded text, so it survives the deletion it describes; the revision it is filed
/// under is a real reference and must already exist.
/// <para>
/// One deleted Entry leaves exactly one tombstone: <c>entry_id</c> is the primary key, so recording
/// the same Entry twice is a conflict rather than a second row. This store never deletes lexical
/// data — <see cref="LEntryArchive.LEntryDelete"/> does that, and the caller records the tombstone
/// afterwards.
/// </para>
/// </summary>
public sealed class LTombstoneArchive
{
    private readonly LDatabase _lTombstoneArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens sessions through.</summary>
    public LTombstoneArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lTombstoneArchiveDatabase = database;
    }

    /// <summary>
    /// Records that the Entry identified by <paramref name="entryId"/> was deleted under
    /// <paramref name="revisionId"/>, stamped with the current UTC time, and returns the stored
    /// tombstone.
    /// </summary>
    public LTombstone LTombstoneRecord(string entryId, string revisionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(revisionId);

        LTombstone stored = new(
            entryId,
            revisionId,
            DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));

        using LDatabaseSession session = _lTombstoneArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO tombstone (entry_id, revision_id, deleted_utc)
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

    /// <summary>
    /// Reads the tombstone for the Entry identified by <paramref name="entryId"/>, or <c>null</c> when
    /// that Entry has never been deleted.
    /// </summary>
    public LTombstone? LTombstoneRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lTombstoneArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            "SELECT entry_id, revision_id, deleted_utc FROM tombstone WHERE entry_id = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LTombstone(reader.GetString(0), reader.GetString(1), reader.GetString(2));
    }

    /// <summary>Reads every tombstone filed under <paramref name="revisionId"/>, oldest deletion first.</summary>
    public IReadOnlyList<LTombstone> LTombstoneRevisionRead(string revisionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(revisionId);

        using LDatabaseSession session = _lTombstoneArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT entry_id, revision_id, deleted_utc
            FROM tombstone WHERE revision_id = $revision ORDER BY deleted_utc, entry_id;
            """;
        command.Parameters.AddWithValue("$revision", revisionId);

        List<LTombstone> tombstones = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            tombstones.Add(new LTombstone(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2)));
        }

        return tombstones;
    }
}
