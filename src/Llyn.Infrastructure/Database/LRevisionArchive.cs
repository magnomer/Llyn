using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists revisions and the ordered changes recorded under them. A revision is written once,
/// complete: <see cref="LRevisionRecord"/> stamps it and inserts its changes in list order inside one
/// transaction, so a half-written revision never reaches the database.
/// <para>
/// The change rows name their targets as recorded text rather than by foreign key, so a revision
/// stays readable after the rows it describes are deleted — which is exactly the case the history
/// exists for. Nothing here deletes lexical data; the stores that own that data do, and the caller
/// hands the resulting change list to this store afterwards.
/// </para>
/// </summary>
public sealed class LRevisionArchive
{
    private readonly LDatabase _lRevisionArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens sessions through.</summary>
    public LRevisionArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lRevisionArchiveDatabase = database;
    }

    /// <summary>
    /// Opens a revision with a fresh opaque id and the current UTC timestamp and records
    /// <paramref name="changes"/> under it in list order — each stored position comes from that order,
    /// not from the position the caller happened to set. Returns the stored revision. The whole write
    /// is one transaction; an empty change list records an empty revision.
    /// </summary>
    public LRevision LRevisionRecord(IReadOnlyList<LRevisionChange> changes)
    {
        ArgumentNullException.ThrowIfNull(changes);

        LRevision stored = new(
            LIdentity.LIdentityCreate(),
            DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));

        using LDatabaseSession session = _lRevisionArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "INSERT INTO revision (id, created_utc) VALUES ($id, $created);";
            command.Parameters.AddWithValue("$id", stored.LRevisionId);
            command.Parameters.AddWithValue("$created", stored.LRevisionCreatedUtc);
            command.ExecuteNonQuery();
        }

        for (int position = 0; position < changes.Count; position++)
        {
            LRevisionChange change = changes[position];
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO revision_change (revision_id, position, target_id, target_type, kind, summary)
                VALUES ($revision, $position, $target, $type, $kind, $summary);
                """;
            command.Parameters.AddWithValue("$revision", stored.LRevisionId);
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$target", change.LRevisionChangeTarget);
            command.Parameters.AddWithValue("$type", change.LRevisionChangeType);
            command.Parameters.AddWithValue("$kind", change.LRevisionChangeKind);
            command.Parameters.AddWithValue("$summary", (object?)change.LRevisionChangeSummary ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    /// <summary>Reads the revision for <paramref name="id"/>, or <c>null</c> when no revision has that id.</summary>
    public LRevision? LRevisionRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lRevisionArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT id, created_utc FROM revision WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LRevision(reader.GetString(0), reader.GetString(1));
    }

    /// <summary>
    /// Reads the most recently opened revision, or <c>null</c> when the workspace has none yet.
    /// Ordering is by the stamped timestamp, then by id, so two revisions opened in the same tick
    /// still read back in one stable order.
    /// </summary>
    public LRevision? LRevisionLatestRead()
    {
        using LDatabaseSession session = _lRevisionArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            "SELECT id, created_utc FROM revision ORDER BY created_utc DESC, id DESC LIMIT 1;";

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LRevision(reader.GetString(0), reader.GetString(1));
    }

    /// <summary>Reads the changes recorded under <paramref name="revisionId"/>, in the order they were recorded.</summary>
    public IReadOnlyList<LRevisionChange> LRevisionChangeRead(string revisionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(revisionId);

        using LDatabaseSession session = _lRevisionArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT position, target_id, target_type, kind, summary
            FROM revision_change WHERE revision_id = $revision ORDER BY position;
            """;
        command.Parameters.AddWithValue("$revision", revisionId);

        List<LRevisionChange> changes = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            changes.Add(new LRevisionChange(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4)));
        }

        return changes;
    }
}
