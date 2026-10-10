# LSchema.cs
Hash: `b30f64ac78e409eb`

## `public static class LSchema`

Runs the schema build in order.
Each area owns a file of its own and creates the tables it is named for.
This file gives table creation, version validation, indexes and stamp creation one explicit sequence.

Creating a table is only half of it.
`IF NOT EXISTS` leaves an existing table untouched.
So a change to a table an earlier build created is carried by `LSchemaMigration`.
`LSchemaMigration` validates or creates the version row through `LSchemaVersion`.
The lookup indexes live in `LSchemaIndex` and are created after the migration.
Version validation precedes indexes, so an incompatible version cannot pass silently.

## `public static void LSchemaCreate(SqliteConnection connection)`

Creates missing tables, validates the version, then creates indexes and stamps.
An incompatible version throws rather than rebuilding here.
Callers must rebuild older databases before running this initializer.

## Inline notes

### `LSchemaReference.LSchemaReferenceCreate(connection);`

The Author and Reference tables come before the Example tables.
Reference creation precedes quotation creation, keeping their dependency visible in the initialization sequence.

### `LSchemaFold.LSchemaFoldCreate(connection);`

The fold marks come after entry, sense and collocation.
Creating the parent tables first keeps the fold tables' ownership dependencies visible.

### `LSchemaAnchor.LSchemaAnchorCreate(connection);`

The shengfu table comes after fanqie, since both are read for one character of a headword.
The anchor table comes after reflex, fanqie and diwei.
The sequence places both foreign-key parents before the anchor table, making their ownership dependencies visible.

### `LSchemaRevision.LSchemaRevisionCreate(connection);`

The operational and history tables close the schema.
They carry no lexical ownership, and the workspace row points at both entry and revision.
So they are created last.

### `LSchemaMigration.LSchemaMigrationApply(connection);`

Every table now exists.
The version is now stamped or checked, not rebuilt.
Indexes follow only after that validation succeeds.
