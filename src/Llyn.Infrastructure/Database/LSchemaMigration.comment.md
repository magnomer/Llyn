# LSchemaMigration.cs

## `public static class LSchemaMigration`

Reads the version an existing database was built at and brings the file to the version this build produces.
It exists because `CREATE TABLE IF NOT EXISTS` leaves an existing table exactly as it stands.
That is everything `LSchema` runs.
A column, a constraint, or a foreign key added to an already-created table never reaches such a file.
Stamping a version without acting on the one already stored would record a shape the file does not have.

No step climbs from one version to the next.
A file at another version is rebuilt instead.
A fresh database is created in the current shape and the old rows are carried into it.
Every column both shapes know carries across, and a column only one of them knows is left behind.
A row the current shape refuses, or one whose parent did not come across, is dropped.
Neither stops the launch.
The old file is kept beside the new one under its version, so nothing is lost to a rebuild.
The same rebuild serves a file written by a newer build, since what this build cannot read it cannot keep.

## `public const long LSchemaMigrationVersion = 47;`

The schema version this build produces.
A change to any table raises it.

## `private const string LSchemaMigrationFresh = "fresh";`

The name the database being built is attached under, and the suffix of its file while it is built.

## `private const string LSchemaMigrationTable = "schema_version";`

The one table never carried across, because the fresh file already records the version it was built at.

## `private const string LSchemaMigrationRealm = "realm";`

The table whose row replaces the one the fresh file minted.
So the workspace keeps the realm it was born under.

## `private const string LSchemaMigrationSequence = "sqlite_sequence";`

The id counters of the autoincrement tables.
They are carried across so a rebuilt workspace never reissues an id a tombstone remembers.

## `public static void LSchemaMigrationApply(SqliteConnection connection)`

Records the version on a database that has never carried one.
Throws when the stored version is not this build's.
The rebuild runs before the schema and leaves no such file behind.

## `public static bool LSchemaMigrationCheck(SqliteConnection connection)`

Whether the database was written at another version and must be rebuilt before the schema runs.
A file with no version table is new, and the schema stamps it rather than rebuilding it.

## `public static string LSchemaMigrationRun(string file)`

Rebuilds `file` in the current shape and returns the path the old file was kept at.
The pools are cleared first, so no pooled connection holds the file while it is moved.
A rebuild that fails leaves the old file untouched and no half-built file beside it.

## `private static string LSchemaFileApply(string file, string fresh)`

Builds the fresh file, attaches it to the old one, carries each shared table across, and swaps the files.
Foreign keys are off during the copy, so the order tables are read in decides nothing.
The orphans that leaves are swept afterwards, under the same transaction.

## `private static void LSchemaTableApply(SqliteConnection connection, string table)`

Copies one table over the columns both shapes name.
A row the fresh shape refuses, by a constraint or a check, is skipped rather than failing the copy.
The realm row replaces the one the fresh file minted, and the id counters replace the fresh file's own.

## `private static void LSchemaOrphanSweep(SqliteConnection connection)`

Deletes every row whose parent did not come across, until the foreign-key check reports nothing.
Deleting a parent can orphan its own children, which is why the check is run again.

## `private static List<string> LSchemaTableRead(SqliteConnection connection, string schema)`

The tables the fresh shape holds that the old file also holds, in the order the schema created them.
SQLite's own tables are left out, except the id counters.

## `private static List<string> LSchemaColumnRead(SqliteConnection connection, string schema, string table)`

The column names of one table in one attached schema.

## `private static string LSchemaBackupResolve(string file, long stored)`

The path the old file is kept at, named by the time and the version it was written at.
A name already taken is stepped past rather than overwritten.

## `private static void LSchemaMigrationDelete(string fresh)`

Removes a half-built fresh file and its journal, so a retried rebuild starts clean.
