# TSchemaMigration.cs

## `public sealed class TSchemaMigration`

Covers the schema runner and the migration that rebuilds a database another build wrote.
Creating twice changes nothing.
The version is recorded and read back.
A database at any other version, older or newer, is rebuilt in the current shape rather than refused.
Every column both shapes know carries across, and a column only the old shape knew is dropped.
The old file stays beside the new one under its version.
The realm the workspace was minted under survives the rebuild, so rows made here keep their stamp.
A row the current schema cannot hold, such as a child whose parent is gone, is dropped.
The launch goes on without it.
The id counters carry across, so a rebuilt workspace never hands out an id a tombstone remembers.
The realm row is single, minted once, and stamps every row made here.

## Inline notes

### `Assert.Equal(1, TSchemaIndexRead(workspace, "collocation", "entry_ref"));`

A child column with no index turns each parent delete into a full scan of the child table.
So the index set is part of the schema rather than an optimization applied later.

### `private static long TSchemaIndexRead(TWorkspace workspace, string table, string column)`

Whether some index on the table has the named column first.
That is what the foreign-key check and the cascade actually use.
