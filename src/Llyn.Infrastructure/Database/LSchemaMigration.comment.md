# LSchemaMigration.cs

## `public static class LSchemaMigration`

Reads the version an existing database was built at, brings it up to the version this build produces, and records the result. It exists because `CREATE TABLE IF NOT EXISTS` — everything `LSchema` runs — leaves an existing table exactly as it stands: a column, a constraint, or a foreign key added to a table a previous build already created never reaches a database that has been opened before. Stamping a version without acting on the one already stored therefore records a shape the file does not have, which is what this file fixes.

A step that has to change an existing table follows SQLite's documented table-rebuild procedure — create the new shape beside the old one, copy, drop, rename — and that requires foreign-key enforcement to be off, which cannot be changed inside a transaction. So the runner is called with a plain connection, before any session is open.

## `public const long LSchemaMigrationVersion = 16;`

The schema version this build produces. A later change to an existing table raises it.

## `public static void LSchemaMigrationApply(SqliteConnection connection)`

Records the version on a database that has never carried one, and otherwise applies every step above the stored version before recording the new one. Throws when the file was written by a newer build than this one, rather than writing it backwards.

## Inline notes

### `if (stored < 12)`

Version 12. Three shapes an older database is missing, none of which a CREATE statement can deliver: the foreign key on example.source_id (declared only once the source table existed, so a database created before that keeps the column bare), duplicate positions in the two ordered sets that had no unique index yet, and a schema_version table that permits more than one row.

### `if (stored < 13)`

Version 13. The collocation card carries a Meaning beside its Expression, so the table needs the column to store it in; without this step the meaning of every collocation saved into an existing database would be dropped on the way to disk.

### `if (stored < 14)`

Version 14. The pronunciation's downloaded recording gets a table of its own. A database built before this version has no pronunciation_audio at all, so the step creates it; the rows already stored are untouched, and a pronunciation with no recording simply has no row.

### `if (stored < 15)`

Version 15. Both card templates have always carried a Title field that nothing stored. The step gives sense and collocation the same title column, so the two cards end up the same shape for it; sense.gloss is left as it stands, unused by the input form.

### `if (stored < 16)`

Version 16. The part-of-speech field is editable, so a row may now carry text no preset names. The step rebuilds part_of_speech with value_id nullable beside a custom_name column and the CHECK that keeps a row to exactly one of the two; the assignments already stored are all preset ids and copy across untouched.

### `private static void LSchemaSpeechNormalize(SqliteConnection connection)`

part_of_speech rebuilt to hold a custom part of speech beside a declared one. Nullability and a CHECK are table-shape facts, so neither ADD COLUMN nor anything short of SQLite's documented rebuild can deliver them to a table an earlier build created.

### `using (SqliteCommand off = connection.CreateCommand())`

Foreign-key enforcement has to be off across a table rebuild, and it cannot be switched inside a transaction — hence the explicit statements rather than a session.

### `private static bool LSchemaVersionExist(SqliteConnection connection)`

Whether the database has ever carried a version row. Absent means the file is new: LSchema has just created every table at the current shape, so there is nothing to migrate.

### `private static void LSchemaVersionCreate(SqliteConnection connection)`

Creates the version table in its current single-row shape and stamps this build's version.

### `private static long LSchemaVersionRead(SqliteConnection connection)`

The highest version recorded. An older database may hold several rows, which is one of the things version 12 removes; until it does, the highest is the one that describes the file.

### `private static void LSchemaVersionNormalize(SqliteConnection connection)`

Rebuilds the version table so exactly one row can exist, and stamps this build's version.

### `private static void LSchemaVersionSave(SqliteConnection connection)`

Records this build's version on a table that already holds exactly one row.

### `private static void LSchemaAudioNormalize(SqliteConnection connection)`

Creates the pronunciation_audio table on a database that predates it. LSchema's own CREATE statement usually gets there first on startup, but the step is what makes the change explicit at the version that introduced it — and what carries it when the table is created by any other path.

### `private static void LSchemaTitleNormalize(SqliteConnection connection, string table)`

Gives a card table the title column its template has always had a field for. Adding a column needs no table rebuild, so existing rows keep everything they have and read back a NULL title, which is what they had. Skipped when the column is already there.

### `private static void LSchemaCollocationNormalize(SqliteConnection connection)`

Gives the collocation table the meaning column its card has always had a field for. Adding a column needs no table rebuild, so the existing rows — and their expressions — are untouched; the meaning of a collocation written before this version reads back as NULL, which is what it was.

### `private static void LSchemaExampleNormalize(SqliteConnection connection)`

Gives example.source_id the foreign key it was declared with only once the source table existed. Skipped when the constraint is already there, so this costs one pragma read on every later start.

### `using (SqliteCommand off = connection.CreateCommand())`

Foreign-key enforcement has to be off across a table rebuild, and it cannot be switched inside a transaction — hence the explicit statements rather than a session.

### `private static void LSchemaPositionNormalize(SqliteConnection connection, string table, string ownerColumn)`

Renumbers an ordered set to 0…n-1 per owner so the unique position index can be created over it. The new positions are computed into a temporary table first: an UPDATE that read the very column it writes would depend on the order the rows happened to be visited in.
