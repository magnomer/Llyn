using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists the workspace row — the operational state of the session, not lexical data. There is one
/// such row per database, carried under a fixed id, so opening the workspace never has to search for
/// it: <see cref="LWorkspaceStateRead"/> creates the row on first use and returns it, and
/// <see cref="LWorkspaceStateSave"/> writes it back in place.
/// <para>
/// The pane columns and the revision column are references the schema clears rather than defends:
/// deleting an Entry sets the pane that showed it back to empty instead of blocking the delete. So
/// this store holds no lexical guard of its own — losing the whole row would lose no dictionary
/// content.
/// </para>
/// </summary>
public sealed class LWorkspaceArchive
{
    /// <summary>Fixed id of the single workspace row this store reads and writes.</summary>
    private const string LWorkspaceArchiveRow = "workspace";

    private readonly LDatabase _lWorkspaceArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LWorkspaceArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lWorkspaceArchiveDatabase = database;
    }

    /// <summary>
    /// Opens the workspace row: returns the stored state, creating an empty row first when the
    /// database has none yet, so the caller always receives a state rather than <c>null</c>.
    /// </summary>
    public LWorkspaceState LWorkspaceStateRead()
    {
        using SqliteConnection connection = _lWorkspaceArchiveDatabase.LDatabaseRead();

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

        using SqliteCommand read = connection.CreateCommand();
        read.CommandText =
            """
            SELECT id, left_entry, right_entry, mode, split, revision
            FROM workspace WHERE id = $id;
            """;
        read.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);

        using SqliteDataReader reader = read.ExecuteReader();
        reader.Read();

        return new LWorkspaceState(
            reader.GetString(0),
            reader.IsDBNull(1) ? null : reader.GetString(1),
            reader.IsDBNull(2) ? null : reader.GetString(2),
            reader.IsDBNull(3) ? null : reader.GetString(3),
            reader.IsDBNull(4) ? null : reader.GetString(4),
            reader.IsDBNull(5) ? null : reader.GetString(5));
    }

    /// <summary>
    /// Writes <paramref name="state"/> back into the workspace row, creating the row when it is
    /// absent. The id travels with the state so a caller that read the row writes the same one back.
    /// </summary>
    public void LWorkspaceStateSave(LWorkspaceState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentException.ThrowIfNullOrWhiteSpace(state.LWorkspaceStateId);

        using SqliteConnection connection = _lWorkspaceArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
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
        command.Parameters.AddWithValue("$id", state.LWorkspaceStateId);
        command.Parameters.AddWithValue("$left", (object?)state.LWorkspaceStateLeft ?? DBNull.Value);
        command.Parameters.AddWithValue("$right", (object?)state.LWorkspaceStateRight ?? DBNull.Value);
        command.Parameters.AddWithValue("$mode", (object?)state.LWorkspaceStateMode ?? DBNull.Value);
        command.Parameters.AddWithValue("$split", (object?)state.LWorkspaceStateSplit ?? DBNull.Value);
        command.Parameters.AddWithValue("$revision", (object?)state.LWorkspaceStateRevision ?? DBNull.Value);
        command.ExecuteNonQuery();
    }
}
