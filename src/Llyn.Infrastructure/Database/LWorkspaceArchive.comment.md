# LWorkspaceArchive.cs

## `public sealed class LWorkspaceArchive`

Persists the workspace row — the operational state of the session, not lexical data.
There is one such row per database, carried under a fixed id.
So opening the workspace never has to search for it.
`LWorkspaceStateRead` creates the row on first use and returns it.
`LWorkspaceStateSave` writes it back in place.

The fixed id is this store's, not the caller's.
A state travels with the id it was read under, so a caller can see which row it holds.
But the save always writes the one row.
Writing a caller-chosen id would let a second workspace row appear.
The read could never find that row again.

The pane columns and the revision column are references the schema clears rather than defends.
Deleting an Entry sets the pane that showed it back to empty instead of blocking the delete.
So this store holds no lexical guard of its own — losing the whole row would lose no dictionary content.

## `private const string LWorkspaceArchiveRow = "workspace";`

Fixed id of the single workspace row this store reads and writes.

## `public LWorkspaceArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LWorkspaceState LWorkspaceStateRead()`

Opens the workspace row and returns the stored state.
It creates an empty row first when the database has none yet.
So the caller always receives a state rather than `null`.

## `public void LWorkspaceStateSave(LWorkspaceState state)`

Writes `state` back into the workspace row, creating the row when it is absent.
The row written is always this store's single one, whatever id the state carries.

## Inline notes

### `if (!reader.Read())`

The insert above guarantees the row.
Reading nothing back would mean the workspace row was removed between the two statements.
The session makes that impossible.
So say so rather than dereference an empty reader.
