# TSchemaMigration.cs

## `public sealed class TSchemaMigration`

Covers the schema runner and the migration that rebuilds a database another build wrote.
Creating twice changes nothing.
The version is recorded and read back.
A database at any other version, older or newer, is rebuilt in the current shape rather than refused.
Every column both shapes know carries across, and a column only the old shape knew is dropped.
A copy of the old file stays beside it under its version.
The file itself is never replaced, and no scratch file is left once the rebuild is done.
A rebuild that fails, here because its scratch file cannot be created, leaves the old file untouched.
It keeps its version and every row.
The realm the workspace was minted under survives the rebuild, so rows made here keep their stamp.
A row the current schema cannot hold, such as a child whose parent is gone, is dropped.
The launch goes on without it.
The id counters carry across, so a rebuilt workspace never hands out an id a tombstone remembers.
The realm row, the stamp and the id counter on a fresh workspace are covered by `TSchemaRealm`.
A version-71 reflex carries its old note into romanization and its old remark into the new note.
Its new meaning is empty and its ownership flag is clear.

## `public void DatabaseCreate_BuildWithoutEtymology_RaisesTheTablesEmpty()`

A version-73 workspace has no etymology tables.
The rebuild adds all three empty and carries every entry across.

## `public void DatabaseCreate_CollocationSharingMeaningId_RenumbersItAndItsLinks()`

An older workspace numbered Collocations apart from Meanings, so the first of each was number 1.
The migration moves the Collocation above both sequences and its links and history follow it.
The Meaning and a Collocation with a number of its own keep theirs.

## `public void DatabaseCreate_SituationMediaAbsent_RebuildsWithSituationsIntact()`

A version-47 workspace has no Situation media tables.
The rebuild adds them empty and carries every Situation and Image across untouched.

## `public void DatabaseCreate_UnknownSourceWording_LinksTheUnknownSource()`

A version-48 workspace wrote `unknown` as an example's source state.
The rebuild points such an example at the Unknown Reference the workspace already holds, and mints no second one.

## `public void DatabaseCreate_UnknownSourceDeleted_MintsItForOldWording()`

A workspace whose Unknown Reference was deleted, yet whose old rows still carry the wording, gets the row back.
Only the old wording brings it back, since the row is ordinary data the user may remove.

## `public void DatabaseCreate_NewWorkspace_SeedsTheUnknownSource()`

A new workspace starts with exactly one Reference, titled Unknown.

## `public void DatabaseCreate_UnknownSourceDeleted_SeedsNothingAgain()`

Opening a workspace again never re-seeds the Unknown Reference, so deleting it holds.

## Inline notes

### `Assert.Equal(1, TSchemaIndexRead(workspace, "collocation", "entry_ref"));`

A child column with no index turns each parent delete into a full scan of the child table.
So the index set is part of the schema rather than an optimization applied later.

### `private static long TSchemaIndexRead(TWorkspace workspace, string table, string column)`

Whether some index on the table has the named column first.
That is what the foreign-key check and the cascade actually use.
