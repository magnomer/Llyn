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

This file is the ladder alone.
Each step is a file of its own, named for the shape it changes, and this one only says at which version it runs.

A step that has to change an existing table follows SQLite's documented table-rebuild procedure.
It creates the new shape beside the old one, copies, drops, and renames.
That requires foreign-key enforcement to be off.
Enforcement cannot be changed inside a transaction.
So the runner is called with a plain connection, before any session is open.

## `public const long LSchemaMigrationVersion = 34;`

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

### `if (stored < 20)`

Version 20.
The card-to-entry link tables arrive, so an upgraded workspace gains them too.

### `if (stored < 21)`

Version 21.
An Example carries one translation of its sentence, on the Example row itself.
The step stands last because the older steps still name the owned-text table it retires.

### `if (stored < 22)`

Version 22.
A Situation carries no Source field, so the two columns holding one are dropped.

### `if (stored < 23)`

Version 23.
The favorite table arrives, so an upgraded workspace gains it too.

### `if (stored < 24)`

Version 24.
A Meaning's hold on an Example becomes a row with data of its own, so `sense_example` is rebuilt.
Nothing is dropped, because an older row said nothing that the new shape cannot hold.
The video tables arrive in the same version through `LSchema`, which creates what is missing.

### `if (stored < 25)`

Version 25.
A Collocation's hold on an Example takes the shape a Meaning's already has, so `collocation_example` is rebuilt.
The editor already drew the frame fields on a Collocation card, and the store dropped them until now.

### `if (stored < 26)`

Version 26.
The workspace row gains one ordering column per browse panel, so the shell's own view state outlives the process.
`LSchemaWorkspace` adds only the columns that are missing, and carries every row over untouched.

### `if (stored < 27)`

Version 27.
A frame is the owner's, not the Example's, so an owner may state one before any sentence is written.
Both hold tables are rebuilt with a nullable `example_id` for it.

### `if (stored < 28)`

Version 28.
A Video now carries the span of it worth watching.
The table gains a state column and a text column for it.

### `if (stored < 31)`

Retires the Reference program and channel columns and puts a kind and a note in their place.
Program name and channel name were one medium's metadata standing as columns every other medium left empty.

### `if (stored < 32)`

Version 32.
Drops entry_source and entry_example, the two tables that linked an Entry straight to a Source or an Example.
Both skipped the levels between, so a workspace could state a citation the chain of cards did not carry.
An Entry now reaches a Source only through the Examples its Meanings and Collocations cite.
