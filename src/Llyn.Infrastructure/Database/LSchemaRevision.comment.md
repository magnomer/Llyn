# LSchemaRevision.cs

## `public static class LSchemaRevision`

Creates the operational and history block of the schema — the workspace row, revisions and their ordered changes, and entry tombstones. It lives beside `LSchema` rather than inside it because these tables carry no lexical ownership: nothing here owns an Entry or is owned by one, and keeping them apart keeps each file to one responsibility.

Ordering matters. `workspace` points at `entry` and at `revision`, so this block runs after `LSchema` has created the lexical tables and creates `revision` before `workspace`: SQLite refuses to prepare a statement writing to a child table whose parent is missing, even for NULL keys.

## `public static void LSchemaRevisionCreate(SqliteConnection connection)`

Creates the workspace, revision, revision-change, and tombstone tables when they do not yet exist. Called by `LSchema.LSchemaCreate` once the lexical tables are in place.

## Inline notes

### `command.CommandText =`

A revision is a stamped point in history, and its changes are ordered rows identified by (revision_id, position) — the position is the order within the revision, so a change has no id of its own and cascades when its revision is dropped. target_id/target_type name what the change touched without a foreign key: a change frequently records a row that no longer exists, which is the whole point of keeping the history.

A tombstone is the same idea for a deleted Entry: entry_id deliberately carries no foreign key to entry, because the row it names has been deleted by the time the tombstone is written. entry_id is the primary key, so one deleted Entry leaves exactly one tombstone, and the revision it was deleted under is a real reference that must exist.

### `command.CommandText =`

The workspace row is operational state — which entries the two panes show, the display mode, the split, and the revision the session is on. It owns nothing lexical: its entry columns are ON DELETE SET NULL so deleting an Entry empties the pane instead of blocking the delete or dragging the workspace row down with it, and its revision column behaves the same way.
