using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LRevisionArchive
{
    private readonly LDatabase _lRevisionArchiveDatabase;

    public LRevisionArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lRevisionArchiveDatabase = database;
    }

    public LRevision LRevisionRecord(IReadOnlyList<LRevisionChange> changes)
    {
        ArgumentNullException.ThrowIfNull(changes);

        LRevision stored = new(
            0,
            DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));

        using LDatabaseSession session = _lRevisionArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "INSERT INTO revision (created_utc) VALUES ($created) RETURNING revision_id;";
            command.Parameters.AddWithValue("$created", stored.LRevisionCreatedUtc);
            stored = stored with { LRevisionId = (long)command.ExecuteScalar()! };
        }

        for (int position = 0; position < changes.Count; position++)
        {
            LRevisionChange change = changes[position];
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO revision_change (revision_parent, position, target_ref, target_type, kind, summary)
                VALUES ($revision, $position, $target, $type, $kind, $summary);
                """;
            command.Parameters.AddWithValue("$revision", stored.LRevisionId);
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$target", change.LRevisionChangeTarget);
            command.Parameters.AddWithValue("$type", change.LRevisionChangeSubject);
            command.Parameters.AddWithValue("$kind", change.LRevisionChangeKind);
            command.Parameters.AddWithValue("$summary", (object?)change.LRevisionChangeSummary ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public LRevision? LRevisionRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lRevisionArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT revision_id, created_utc FROM revision WHERE revision_id = $id;";
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LRevision(reader.GetInt64(0), reader.GetString(1));
    }

    public LRevision? LRevisionLatestRead()
    {
        using LDatabaseSession session = _lRevisionArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            "SELECT revision_id, created_utc FROM revision ORDER BY created_utc DESC, revision_id DESC LIMIT 1;";

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LRevision(reader.GetInt64(0), reader.GetString(1));
    }

    public IReadOnlyList<LRevisionChange> LRevisionChangeRead(long revisionId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(revisionId);

        using LDatabaseSession session = _lRevisionArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT position, target_ref, target_type, kind, summary
            FROM revision_change WHERE revision_parent = $revision ORDER BY position;
            """;
        command.Parameters.AddWithValue("$revision", revisionId);

        List<LRevisionChange> changes = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            changes.Add(new LRevisionChange(
                reader.GetInt32(0),
                reader.GetInt64(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4)));
        }

        return changes;
    }
}
