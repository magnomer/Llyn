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

The highest version recorded.
An older database may hold several rows, which is one of the things version 12 removes.
Until it does, the highest is the one that describes the file.

## `public static void LSchemaVersionNormalize(SqliteConnection connection)`

Rebuilds the version table so exactly one row can exist, and stamps this build's version.

## `public static void LSchemaVersionSave(SqliteConnection connection)`

Records this build's version on a table that already holds exactly one row.
