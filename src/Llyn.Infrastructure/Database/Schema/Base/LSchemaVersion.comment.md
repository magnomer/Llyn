# LSchemaVersion.cs

## `public static class LSchemaVersion`

The schema_version table itself.
Whether a database carries one, what it records, and the shape that permits exactly one row.
Every other migration step reads a table the workspace owns.
This one reads the table the migration keeps about itself.

## `public static bool LSchemaVersionExist(SqliteConnection connection)`

Whether the database has ever carried a version row.
Absent means the file is new.
LSchema has just created every table at the current shape, so there is nothing to migrate.

## `public static void LSchemaVersionCreate(SqliteConnection connection)`

Creates the version table in its current single-row shape and stamps this build's version.

## `public static long LSchemaVersionRead(SqliteConnection connection)`

The version recorded.

