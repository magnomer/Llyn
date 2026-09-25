using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LRevisionArchive : LRevisionVault
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
}
