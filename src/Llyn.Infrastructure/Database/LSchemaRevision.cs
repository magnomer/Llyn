using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Creates the operational and history block of the schema — the workspace row, revisions and their
/// ordered changes, and entry tombstones. It lives beside <see cref="LSchema"/> rather than inside it
/// because these tables carry no lexical ownership: nothing here owns an Entry or is owned by one, and
/// keeping them apart keeps each file to one responsibility.
/// <para>
/// Ordering matters. <c>workspace</c> points at <c>entry</c> and at <c>revision</c>, so this block runs
/// after <see cref="LSchema"/> has created the lexical tables and creates <c>revision</c> before
/// <c>workspace</c>: SQLite refuses to prepare a statement writing to a child table whose parent is
/// missing, even for NULL keys.
/// </para>
/// </summary>
public static class LSchemaRevision
{
    /// <summary>
    /// Creates the workspace, revision, revision-change, and tombstone tables when they do not yet
    /// exist. Called by <see cref="LSchema.LSchemaCreate"/> once the lexical tables are in place.
    /// </summary>
    public static void LSchemaRevisionCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        // A revision is a stamped point in history, and its changes are ordered rows identified by
        // (revision_id, position) — the position is the order within the revision, so a change has no
        // id of its own and cascades when its revision is dropped. target_id/target_type name what the
        // change touched without a foreign key: a change frequently records a row that no longer
        // exists, which is the whole point of keeping the history.
        //
        // A tombstone is the same idea for a deleted Entry: entry_id deliberately carries no foreign
        // key to entry, because the row it names has been deleted by the time the tombstone is written.
        // entry_id is the primary key, so one deleted Entry leaves exactly one tombstone, and the
        // revision it was deleted under is a real reference that must exist.
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS revision (
                id TEXT NOT NULL PRIMARY KEY,
                created_utc TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS revision_change (
                revision_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                target_id TEXT NOT NULL,
                target_type TEXT NOT NULL,
                kind TEXT NOT NULL,
                summary TEXT,
                PRIMARY KEY (revision_id, position),
                FOREIGN KEY (revision_id) REFERENCES revision (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS tombstone (
                entry_id TEXT NOT NULL PRIMARY KEY,
                revision_id TEXT NOT NULL,
                deleted_utc TEXT NOT NULL,
                FOREIGN KEY (revision_id) REFERENCES revision (id)
            );
            """;
        command.ExecuteNonQuery();

        // The workspace row is operational state — which entries the two panes show, the display mode,
        // the split, and the revision the session is on. It owns nothing lexical: its entry columns
        // are ON DELETE SET NULL so deleting an Entry empties the pane instead of blocking the delete
        // or dragging the workspace row down with it, and its revision column behaves the same way.
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS workspace (
                id TEXT NOT NULL PRIMARY KEY,
                left_entry TEXT,
                right_entry TEXT,
                mode TEXT,
                split TEXT,
                revision TEXT,
                FOREIGN KEY (left_entry) REFERENCES entry (id) ON DELETE SET NULL,
                FOREIGN KEY (right_entry) REFERENCES entry (id) ON DELETE SET NULL,
                FOREIGN KEY (revision) REFERENCES revision (id) ON DELETE SET NULL
            );
            """;
        command.ExecuteNonQuery();
    }
}
