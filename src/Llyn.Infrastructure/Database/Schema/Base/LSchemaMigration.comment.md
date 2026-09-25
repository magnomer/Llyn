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
The rows then return into the old file, whose tables are replaced under one transaction.
The file itself is never moved or renamed.
So a crash at any point leaves it whole, at one version or the other.
A copy of the old file is kept beside it under its version, so nothing is lost to a rebuild.
The same rebuild serves a file written by a newer build, since what this build cannot read it cannot keep.

## `public const long LSchemaMigrationVersion = 75;`

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

Rebuilds `file` in the current shape and returns the path its copy was kept at.
The pools are cleared first, so no pooled connection holds a stale fresh file while it is deleted.
The fresh file is a scratch space and is deleted again whether the rebuild succeeded or failed.
A rebuild that fails leaves the old file untouched and no half-built file beside it.

## `private static string LSchemaFileApply(string file, string fresh)`

Builds the fresh file, copies the old file aside, and attaches the fresh file to the old one.
One transaction then carries each shared table across and settles old unknown citations and register marks.
It also dates the stored glyph pictures whose captions still carry their age in the source's own words.
It renumbers every Collocation whose id a Meaning also holds.
It then sweeps the orphans and brings the rows back.
Bringing them back drops every old table, runs the schema on the file, and copies the fresh tables in.
The transaction commits or the old file stays exactly as it was.
Foreign keys are off during the copy, so the order tables are read in decides nothing.

## `private static void LSchemaTableApply(SqliteConnection connection, string from, string into, string table)`

Copies one table from the `from` schema into the `into` schema over the columns both shapes name.
A row the target shape refuses, by a constraint or a check, is skipped rather than failing the copy.
The realm row replaces the one the schema minted, and the id counters replace the target's own.

## `private static void LSchemaTableClear(SqliteConnection connection)`

Drops every table and view of the old file, so the schema can create the current shape in their place.
Indexes and triggers go with their tables.
SQLite's own tables cannot be dropped and are left alone.

## `private static void LSchemaOrphanSweep(SqliteConnection connection)`

Deletes every row whose parent did not come across, until the foreign-key check reports nothing.
Deleting a parent can orphan its own children, which is why the check is run again.

## `private static List<string> LSchemaTableRead(SqliteConnection connection, string schema, string other)`

The tables `schema` holds that `other` also holds, in the order the schema created them.
SQLite's own tables are left out, except the id counters.

## `private static string LSchemaBackupSave(SqliteConnection connection, string file, long stored)`

Writes a consistent copy of the old file beside it and returns the copy's path.
SQLite makes the copy itself, so the file is never moved and no journal is left behind it.

## `private static List<string> LSchemaColumnRead(SqliteConnection connection, string schema, string table)`

The column names of one table in one attached schema.

## `private static string LSchemaBackupResolve(string file, long stored)`

The path the old file is kept at, named by the time and the version it was written at.
A name already taken is stepped past rather than overwritten.

## `private static void LSchemaMigrationDelete(string fresh)`

Removes the fresh file and its journal, whether half-built or spent, so a retried rebuild starts clean.
