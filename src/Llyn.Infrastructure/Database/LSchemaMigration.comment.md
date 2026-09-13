# LSchemaMigration.cs

## `public static class LSchemaMigration`

Reads the version an existing database was built at and checks it against the version this build produces.
It exists because `CREATE TABLE IF NOT EXISTS` leaves an existing table exactly as it stands.
That is everything `LSchema` runs.
A column, a constraint, or a foreign key added to an already-created table never reaches such a file.
Stamping a version without acting on the one already stored would record a shape the file does not have.
So a file at another version is refused rather than stamped.

Until 1.0.0 no upgrade path is kept.
A schema change raises the version, and a workspace built before it is created anew and imported.

## `public const long LSchemaMigrationVersion = 46;`

The schema version this build produces.
A change to any table raises it.

## `public static void LSchemaMigrationApply(SqliteConnection connection)`

Records the version on a database that has never carried one.
Throws when the file was written by a newer build than this one, rather than writing it backwards.
Throws when the file was written by an older build, since no step climbs between versions.
