# LSchemaRevision.cs

## `public static class LSchemaRevision`

Creates the operational and history block of the schema.
That is the workspace row, revisions and their ordered changes, and entry tombstones.
It lives beside `LSchema` rather than inside it, because these tables carry no lexical ownership.
Nothing here owns an Entry or is owned by one.
Keeping them apart keeps each file to one responsibility.

Ordering matters.
`workspace` points at `entry` and at `revision`.
So this block runs after `LSchema` has created the lexical tables.
It creates `revision` before `workspace`.
A revision id counts strictly upward and is never given to a later revision once the row is gone.
SQLite refuses to prepare a statement writing to a child table whose parent is missing.
That holds even for NULL keys.

## `public static void LSchemaRevisionCreate(SqliteConnection connection)`

Creates the workspace, revision, revision-change, and tombstone tables when they do not yet exist.
Called by `LSchema.LSchemaCreate` once the lexical tables are in place.

## Inline notes

### `command.CommandText =`

A revision is a stamped point in history.
Its changes are ordered rows identified by (revision_parent, position).
The position is the order within the revision.
So a change has no id of its own and cascades when its revision is dropped.
target_ref and target_type name what the change touched without a foreign key.
A change frequently records a row that no longer exists.
That is the whole point of keeping the history.

A tombstone is the same idea for a deleted Entry.
entry_ref deliberately carries no foreign key to entry.
The row it names has been deleted by the time the tombstone is written.
entry_ref is the primary key, so one deleted Entry leaves exactly one tombstone.
The revision it was deleted under is a real reference that must exist.

### `command.CommandText =`

The workspace row is operational state.
It holds which Entry each duplex side shows and the tab standing open.
It also holds whether that tab shows its editor and the ordering each browse panel lists by.
The session revision is held with them.
So is the identity floor, the lowest temporary id ever issued, which the schema keeps at or below zero.
Its ordering columns hold the stored name of an ordering.
A build that adds one still reads back what an earlier one wrote.
It owns nothing lexical.
Its entry columns are ON DELETE SET NULL, so deleting an Entry empties the duplex side that showed it.
It neither blocks the delete nor drags the workspace row down with it.
Its revision column behaves the same way.
