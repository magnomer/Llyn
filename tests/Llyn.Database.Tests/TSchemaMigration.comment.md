# TSchemaMigration.cs

## `public sealed class TSchemaMigration`

Covers the schema runner and the migration that brings an older database up to the current version: creating twice changes nothing, the version is recorded and read back, a database written by a newer build is refused, and a database in the shape an earlier build left behind is corrected rather than merely restamped.

## Inline notes

### `workspace.TWorkspaceScriptRun(`

The shape an earlier build left behind: a version table that permits several rows, and an example table whose source_id carries no foreign key because the source table did not exist when the column was declared.

### `workspace.TWorkspaceScriptRun(`

Version 13: every table this build creates except pronunciation_audio, which did not exist.

### `workspace.TWorkspaceScriptRun(`

Two collocations sharing one position under the same entry — possible before the unique index existed, and fatal to creating it now.

### `workspace.TWorkspaceScriptRun(`

A database in the version-12 shape: the collocation table as it stood, with a row in it. CREATE TABLE IF NOT EXISTS never reaches it, so only the migration step can add the column.

### `Assert.Equal(`

The row that was there before the column existed survives, expression intact, meaning empty.

### `workspace.TWorkspaceScriptRun(`

A database in the version-14 shape: neither card table had a title column, and both hold a row. CREATE TABLE IF NOT EXISTS never reaches an existing table, so only the step adds them.

### `Assert.Equal(`

Both tables end up the same shape for this field, and both rows survive with a NULL title.

### `workspace.TWorkspaceScriptRun(`

A database in the version-15 shape: part_of_speech held a stable id and nothing else, and an entry is filed under one. The column and the nullability the editable field needs are table shape, so only the rebuild step delivers them.

### `Assert.Equal(`

The assignment already stored is a declared preset and stays one: it copies across with no custom text beside it.

### `workspace.TWorkspaceScriptRun(`

And the rebuilt table takes the row the editable field produces, which the old shape could not hold at all.

### `Assert.Equal(1, TSchemaIndexRead(workspace, "collocation", "entry_id"));`

A child column with no index turns each parent delete into a full scan of the child table, so the index set is part of the schema rather than an optimization applied later.

### `private static long TSchemaIndexRead(TWorkspace workspace, string table, string column)`

Whether some index on the table has the named column first — which is what the foreign-key check and the cascade actually use.
