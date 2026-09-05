# LSchemaMigration.cs

## `public static class LSchemaMigration`

Reads the version an existing database was built at.
Brings it up to the version this build produces.
Records the result.
It exists because `CREATE TABLE IF NOT EXISTS` leaves an existing table exactly as it stands.
That is everything `LSchema` runs.
A column, a constraint, or a foreign key added to an already-created table never reaches such a file.
The file is one that has been opened before.
Stamping a version without acting on the one already stored records a shape the file does not have.
That is what this file fixes.

A step that has to change an existing table follows SQLite's documented table-rebuild procedure.
It creates the new shape beside the old one, copies, drops, and renames.
That requires foreign-key enforcement to be off.
Enforcement cannot be changed inside a transaction.
So the runner is called with a plain connection, before any session is open.

## `public const long LSchemaMigrationVersion = 21;`

The schema version this build produces.
A later change to an existing table raises it.

## `public static void LSchemaMigrationApply(SqliteConnection connection)`

Records the version on a database that has never carried one.
Otherwise it applies every step above the stored version before recording the new one.
Throws when the file was written by a newer build than this one, rather than writing it backwards.

## Inline notes

### `if (stored < 12)`

Version 12.
Three shapes an older database is missing, none of which a CREATE statement can deliver.
The first is the foreign key on example.source_id.
It was declared only once the source table existed, so an older database keeps the column bare.
The second is duplicate positions in the two ordered sets that had no unique index yet.
The third is a schema_version table that permits more than one row.

### `if (stored < 13)`

Version 13.
The collocation card carries a Meaning beside its Expression.
So the table needs the column to store it in.
Without this step the meaning of every collocation saved into an existing database would be dropped.

### `if (stored < 14)`

Version 14.
The pronunciation's downloaded recording gets a table of its own.
A database built before this version has no pronunciation_audio at all, so the step creates it.
The rows already stored are untouched.
A pronunciation with no recording simply has no row.

### `if (stored < 15)`

Version 15.
Both card templates have always carried a Title field that nothing stored.
The step gives meaning and collocation the same title column.
So the two cards end up the same shape for it.
meaning.gloss is left as it stands, unused by the input form.

### `if (stored < 16)`

Version 16.
The part-of-speech field is editable, so a row may now carry text no preset names.
The step rebuilds part_of_speech with value_id nullable beside a custom_name column.
A CHECK keeps a row to exactly one of the two.
The assignments already stored are all preset ids and copy across untouched.

### `if (stored < 20)`

Version 20.
The card-to-entry link tables arrive, so an upgraded workspace gains them too.

### `if (stored < 21)`

Version 21.
An Example carries one translation of its sentence, on the Example row itself.
The step stands last because the older steps still name the owned-text table it retires.

### `private static void LSchemaSpeechNormalize(SqliteConnection connection)`

part_of_speech rebuilt to hold a custom part of speech beside a declared one.
Nullability and a CHECK are table-shape facts.
So neither ADD COLUMN nor anything short of SQLite's documented rebuild can deliver them.
The table is one an earlier build created.

### `using (SqliteCommand off = connection.CreateCommand())`

Foreign-key enforcement has to be off across a table rebuild.
It cannot be switched inside a transaction.
Hence the explicit statements rather than a session.

### `private static bool LSchemaVersionExist(SqliteConnection connection)`

Whether the database has ever carried a version row.
Absent means the file is new.
LSchema has just created every table at the current shape, so there is nothing to migrate.

### `private static void LSchemaVersionCreate(SqliteConnection connection)`

Creates the version table in its current single-row shape and stamps this build's version.

### `private static long LSchemaVersionRead(SqliteConnection connection)`

The highest version recorded.
An older database may hold several rows, which is one of the things version 12 removes.
Until it does, the highest is the one that describes the file.

### `private static void LSchemaVersionNormalize(SqliteConnection connection)`

Rebuilds the version table so exactly one row can exist, and stamps this build's version.

### `private static void LSchemaVersionSave(SqliteConnection connection)`

Records this build's version on a table that already holds exactly one row.

### `private static void LSchemaAudioNormalize(SqliteConnection connection)`

Creates the pronunciation_audio table on a database that predates it.
LSchema's own CREATE statement usually gets there first on startup.
But the step is what makes the change explicit at the version that introduced it.
It is also what carries the change when the table is created by any other path.

### `private static void LSchemaTitleNormalize(SqliteConnection connection, string table)`

Gives a card table the title column its template has always had a field for.
Adding a column needs no table rebuild.
So existing rows keep everything they have and read back a NULL title, which is what they had.
Skipped when the column is already there.

### `private static void LSchemaCollocationNormalize(SqliteConnection connection)`

Gives the collocation table the meaning column its card has always had a field for.
Adding a column needs no table rebuild.
So the existing rows, and their expressions, are untouched.
The meaning of a collocation written before this version reads back as NULL, which is what it was.

### `private static void LSchemaExampleNormalize(SqliteConnection connection)`

Gives example.source_id the foreign key it was declared with only once the source table existed.
Skipped when the constraint is already there, so this costs one pragma read on every later start.

### `using (SqliteCommand off = connection.CreateCommand())`

Foreign-key enforcement has to be off across a table rebuild.
It cannot be switched inside a transaction.
Hence the explicit statements rather than a session.

### `private static void LSchemaPositionNormalize(SqliteConnection connection, string table, string ownerColumn)`

Renumbers an ordered set to 0…n-1 per owner so the unique position index can be created over it.
The new positions are computed into a temporary table first.
An UPDATE that read the very column it writes would depend on the order rows were visited.

### `private static void LSchemaTranslationCreate(SqliteConnection connection)`

Creates sense_translation and collocation_translation with their unique position indexes.
The tables hold nothing yet, so there is no data to move and no rebuild to run.
CREATE TABLE IF NOT EXISTS makes the step harmless on a workspace that already carries them.

### `private static void LSchemaTranslationNormalize(SqliteConnection connection)`

Retires the owned-text table an Example used to carry and puts one translation on the Example row.
An example row still holding a local column is the mark of the old shape, because LSchema cannot add one.
The rebuild is skipped when that column is gone, and the retired tables are dropped either way.

### `private static void LSchemaCarriedRead(SqliteConnection connection)`

The expression that decides what each Example's single translation becomes.
The hand-written local text wins, because a person wrote it for that sentence.
The first owned row stands in when no local text was written, so an upgraded workspace keeps one translation instead of none.
The table it reads is named example_rendition or example_translation depending on how old the file is.
