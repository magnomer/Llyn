using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LWorkspaceArchive
{
    private const string LWorkspaceArchiveRow = "workspace";

    private readonly LDatabase _lWorkspaceArchiveDatabase;

    public LWorkspaceArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lWorkspaceArchiveDatabase = database;
    }

    public LWorkspaceState LWorkspaceStateRead()
    {
        using LDatabaseSession session = _lWorkspaceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO workspace (id, left_entry, right_entry, mode, split, revision)
                VALUES ($id, NULL, NULL, NULL, NULL, NULL)
                ON CONFLICT (id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);
            command.ExecuteNonQuery();
        }

        LWorkspaceState state;
        using (SqliteCommand read = connection.CreateCommand())
        {
            read.CommandText =
                """
                SELECT id, left_entry, right_entry, mode, split, revision
                FROM workspace WHERE id = $id;
                """;
            read.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);

            using SqliteDataReader reader = read.ExecuteReader();

            if (!reader.Read())
            {
                throw new InvalidOperationException(
                    "The workspace row is missing immediately after it was created.");
            }

            state = new LWorkspaceState(
                reader.GetString(0),
                reader.IsDBNull(1) ? null : reader.GetString(1),
                reader.IsDBNull(2) ? null : reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5));
        }

        session.LDatabaseSessionCommit();
        return state;
    }

    public void LWorkspaceStateSave(LWorkspaceState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        using LDatabaseSession session = _lWorkspaceArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO workspace (id, left_entry, right_entry, mode, split, revision)
                VALUES ($id, $left, $right, $mode, $split, $revision)
                ON CONFLICT (id) DO UPDATE SET
                    left_entry = excluded.left_entry,
                    right_entry = excluded.right_entry,
                    mode = excluded.mode,
                    split = excluded.split,
                    revision = excluded.revision;
                """;
            command.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);
            command.Parameters.AddWithValue("$left", (object?)state.LWorkspaceStateLeft ?? DBNull.Value);
            command.Parameters.AddWithValue("$right", (object?)state.LWorkspaceStateRight ?? DBNull.Value);
            command.Parameters.AddWithValue("$mode", (object?)state.LWorkspaceStateMode ?? DBNull.Value);
            command.Parameters.AddWithValue("$split", (object?)state.LWorkspaceStateSplit ?? DBNull.Value);
            command.Parameters.AddWithValue("$revision", (object?)state.LWorkspaceStateRevision ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }
}
