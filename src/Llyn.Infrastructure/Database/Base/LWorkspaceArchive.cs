using System;
using System.IO;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LWorkspaceArchive : LWorkspaceVault
{
    private const long LWorkspaceArchiveRow = 1;

    private const string LWorkspaceArchiveColumn =
        "workspace_id, left_entry_ref, right_entry_ref, revision_ref, identity_floor";

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
                $"""
                INSERT INTO workspace ({LWorkspaceArchiveColumn})
                VALUES ($id, NULL, NULL, NULL, 0)
                ON CONFLICT (workspace_id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);
            command.ExecuteNonQuery();
        }

        LWorkspaceState state;
        using (SqliteCommand read = connection.CreateCommand())
        {
            read.CommandText =
                $"""
                SELECT {LWorkspaceArchiveColumn}
                FROM workspace WHERE workspace_id = $id;
                """;
            read.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);

            using SqliteDataReader reader = read.ExecuteReader();

            if (!reader.Read())
            {
                throw new InvalidOperationException(
                    "The workspace row is missing immediately after it was created.");
            }

            state = new LWorkspaceState(
                LWorkspaceArchiveResolve(reader, 1),
                LWorkspaceArchiveResolve(reader, 2),
                LWorkspaceArchiveResolve(reader, 3),
                reader.GetInt64(4));
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
                $"""
                INSERT INTO workspace ({LWorkspaceArchiveColumn})
                VALUES ($id, $left, $right, $revision, $floor)
                ON CONFLICT (workspace_id) DO UPDATE SET
                    left_entry_ref = excluded.left_entry_ref,
                    right_entry_ref = excluded.right_entry_ref,
                    revision_ref = excluded.revision_ref,
                    identity_floor = min(identity_floor, excluded.identity_floor);
                """;
            command.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);
            command.Parameters.AddWithValue("$left", (object?)state.LWorkspaceStateLeft ?? DBNull.Value);
            command.Parameters.AddWithValue("$right", (object?)state.LWorkspaceStateRight ?? DBNull.Value);
            command.Parameters.AddWithValue("$revision", (object?)state.LWorkspaceStateRevision ?? DBNull.Value);
            command.Parameters.AddWithValue("$floor", state.LWorkspaceStateFloor);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public long LWorkspaceFloorAdjust()
    {
        using LDatabaseSession session = _lWorkspaceArchiveDatabase.LDatabaseSessionStart();
        long floor;
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO workspace (workspace_id, identity_floor)
                VALUES ($id, -1)
                ON CONFLICT (workspace_id) DO UPDATE SET identity_floor = identity_floor - 1
                RETURNING identity_floor;
                """;
            command.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);
            floor = Convert.ToInt64(command.ExecuteScalar());
        }

        session.LDatabaseSessionCommit();
        return floor;
    }

    public long LWorkspaceSizeRead()
    {
        FileInfo file = new(_lWorkspaceArchiveDatabase.LDatabaseFile);
        return file.Exists ? file.Length : 0;
    }

    private static long? LWorkspaceArchiveResolve(SqliteDataReader reader, int column)
    {
        return reader.IsDBNull(column) ? null : reader.GetInt64(column);
    }
}
