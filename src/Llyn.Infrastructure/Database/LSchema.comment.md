# LSchema.cs

## `public static class LSchema`

Runs the schema build in order.
Each area owns a file of its own and creates the tables it is named for.
This file names those areas in the only order their foreign keys allow.
So the schema grows one area at a time without ever dropping what an earlier build made.

Creating a table is only half of it.
`IF NOT EXISTS` leaves an existing table untouched.
So a change to a table an earlier build created is carried by `LSchemaMigration`.
That file also owns the version row.
The lookup indexes live in `LSchemaIndex` and are created after the migration.
One of them cannot exist until the migration has cleaned the rows it indexes.

## `public static void LSchemaCreate(SqliteConnection connection)`

Creates every table that does not yet exist.
Migrates a database built by an earlier version.
Creates the lookup indexes.
Safe to run on each startup: existing tables and an existing version row are left as they are.

## Inline notes

### `LSchemaReference.LSchemaReferenceCreate(connection);`

The Author and Reference tables come before the Example tables.
An example row carries a foreign key into source, and SQLite refuses a child table whose parent is missing.

### `LSchemaRevision.LSchemaRevisionCreate(connection);`

The operational and history tables close the schema.
They carry no lexical ownership, and the workspace row points at both entry and revision.
So they are created last.

### `LSchemaMigration.LSchemaMigrationApply(connection);`

Every table now exists.
What an earlier build left in the wrong shape is corrected next.
Only then are the indexes created.
The unique position indexes cannot be built over rows that still hold duplicates.
The migration removes those duplicates.
