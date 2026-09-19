# LWorkspaceArchive.cs

## `public sealed class LWorkspaceArchive : LWorkspaceVault`

It is the adapter of `LWorkspaceVault`, the port the engine holds.

Persists the workspace row — the operational state of the session, not lexical data.
There is one such row per database, carried under id 1.
So opening the workspace never has to search for it.
`LWorkspaceStateRead` creates the row on first use and returns it.
`LWorkspaceStateSave` writes it back in place.

The fixed id is this store's, not the caller's.
A state travels with the id it was read under, so a caller can see which row it holds.
But the save always writes the one row.
Writing a caller-chosen id would let a second workspace row appear.
The read could never find that row again.

The row names only rows of this database: the Entry each duplex side shows and the session revision.
How a panel orders, what it hides and which tab stands open name no row.
They live in the settings file instead.

The pane columns and the revision column are references the schema clears rather than defends.
Deleting an Entry sets the pane that showed it back to empty instead of blocking the delete.
So this store holds no lexical guard of its own — losing the whole row would lose no dictionary content.

## `private const long LWorkspaceArchiveRow = 1;`

Fixed id of the single workspace row this store reads and writes.

## `private const string LWorkspaceArchiveColumn`

The column list the row is created under and read back by, in the order the reader indexes them.

## `public LWorkspaceArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LWorkspaceState LWorkspaceStateRead()`

Opens the workspace row and returns the stored state.
It creates an empty row first when the database has none yet.
So the caller always receives a state rather than `null`.

## `public void LWorkspaceStateSave(LWorkspaceState state)`

Writes `state` back into the workspace row, creating the row when it is absent.
The row written is always this store's single one, whatever id the state carries.
The identity floor is written only downward.
A state read before another engine issued an id would otherwise raise the floor.
That id could then come round again.

## `public long LWorkspaceFloorAdjust()`

Lowers the identity floor by one and returns the new floor.

One statement lowers and reads, so two engines on one workspace never receive the same number.
The row is created at minus one when the database has none yet.

## `private static long? LWorkspaceArchiveResolve(SqliteDataReader reader, int column)`

Reads one reference column, returning nothing for a side or revision that stands empty.

## Inline notes

### `if (!reader.Read())`

The insert above guarantees the row.
Reading nothing back would mean the workspace row was removed between the two statements.
The session makes that impossible.
So say so rather than dereference an empty reader.
