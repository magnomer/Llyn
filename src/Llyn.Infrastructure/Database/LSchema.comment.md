# LSchema.cs

## `public static class LSchema`

Creates the database schema idempotently. Each job adds its own `CREATE TABLE IF NOT EXISTS` statements here (or in a per-area file it owns) so the schema grows one job at a time without ever dropping what an earlier job built.

Creating a table is only half of it. `IF NOT EXISTS` leaves an existing table untouched, so a change to a table an earlier build already created is carried by `LSchemaMigration`, which also owns the version row; the lookup indexes live in `LSchemaIndex` and are created after the migration, because one of them cannot exist until the migration has cleaned the rows it indexes.

## `public static void LSchemaCreate(SqliteConnection connection)`

Creates every table that does not yet exist, migrates a database built by an earlier version, and creates the lookup indexes. Safe to run on each startup: existing tables and an existing version row are left as they are.

## Inline notes

### `command.CommandText =`

Data-contract names (table and column identifiers) are persisted keys, so they stay lowercase and independent of code member names.

The Entry root, its owned written forms and parts of speech, and the language-controlled POS display vocabulary. Owned child rows carry an (entry_id, position) identity and cascade when their Entry is deleted.

A part_of_speech row says its part of speech one of two ways and never both: value_id names a preset the language declares, whose display name lives in part_of_speech_value, or custom_name carries text the user typed that no preset names. The CHECK is what keeps the two from drifting into a row that is half an id and half a name.

### `command.CommandText =`

Entry-owned inflected forms and their ordered grammatical features, plus the language-controlled morphology display vocabulary. A feature's identity is the two-level owned key (entry_id, inflection_position, position); it cascades when its inflection is deleted, and an inflection cascades when its Entry is deleted. Lexical rows store only stable ids — display names live in morphology_value.

### `command.CommandText =`

The Meaning tree an entry owns: each sense is a stable-id node that may nest under another sense in the same entry, carrying a single inline definition field. A sense cascades when its parent sense is deleted and when its Entry is deleted. Sibling ordering is unique within a parent — the ifnull() expression index treats root senses (null parent) as one sibling group.

### `command.CommandText =`

Lexical relations originating from a Meaning. Each relation hangs from its origin sense and points at exactly one target through a checked reference row: an Entry (relation_entry) XOR another Meaning (relation_sense). The relation_id primary key on each target table allows at most one target row per relation there; the store enforces the XOR across the two tables. A relation cascades when its origin sense is deleted, and its target row cascades with it — the referenced Entry/Meaning is never touched.

### `command.CommandText =`

The single pronunciation an entry owns and its two owned child structures. An entry carries at most one pronunciation — enforced by the unique entry_id, with no position on the pronunciation itself — and the pronunciation owns ordered syllables and representations keyed by (pronunciation_id, position). A syllable requires only its nucleus; every other syllable field and a representation's local_tone are optional and store NULL when absent (distinct from empty). The pronunciation also owns at most one downloaded recording: pronunciation_audio has no id of its own, since pronunciation_id is its primary key, and its file is stored relative to the workspace folder so a moved or copied workspace keeps its audio. Children cascade when their pronunciation is deleted, and the pronunciation cascades when its entry is deleted.

### `command.CommandText =`

The collocations an entry owns and the single Note it owns. A collocation is a stable-id row ordered within its entry, carrying both an expression and the meaning that explains it — the card has a field for each — where a sense carries a definition; reordering rewrites position only. A collocation's synonym is an interlink, not owned text: it uses the same discriminated target model as a job05 relation — an Entry XOR a Meaning — with the XOR enforced by a check constraint and each target column a checked foreign key, so the referenced row must exist and is never touched by the link. The note table has no id and no position: entry_id is its primary key, which makes at-most-one Note per entry a schema fact. Everything here cascades when its entry is deleted, and a synonym cascades when its collocation is deleted.

### `command.CommandText =`

The independent Author and the independent bibliographic Reference, created before the Example block below so example.source_id can carry its foreign key. Both are owned by nothing: an Author is shared by any number of References, and a Reference is cited by any number of Entries and Examples without belonging to any of them. Every Reference field is stored as a state column plus a value column: the state says whether the field was never filled in, was recorded as unknown, or holds a value, and the value column is NULL unless the state is 'specified' — the check constraints make that a schema fact rather than a store convention. author_state has no value column of its own because the authors themselves are the value, attached in order through source_author. That table cascades from its Reference, so deleting a Reference drops its author links and never an Author; entry_source cascades from its Entry only, so the source_id and author_id foreign keys deliberately have no cascade and the stores refuse to delete a Reference or an Author while anything still points at it.

### `command.CommandText =`

The independent Example and the references that reach it. An Example is owned by nothing: it carries its own opaque id, its language and display text, and at most one Source reference — and it is reached through the three association tables below, one per referrer kind. Each association carries the position the Example takes *for that referrer*, so one Example may be first under an Entry and third under a Meaning; the unique index per referrer keeps those orderings free of duplicates. Deleting a referrer removes only its own association rows (ON DELETE CASCADE on the referrer side); the example_id foreign keys deliberately have no cascade, so an Example survives every detach and the store refuses to delete one while any reference still points at it. Translations are owned text and cascade with their Example.

example.source_id points at the independent Reference the block above creates. Its foreign key could only be declared once that source table existed — SQLite refuses to prepare any statement writing to a child table whose parent is missing, even for NULL keys — which is why the source block runs first. A database created before this version keeps the column without the constraint: CREATE TABLE IF NOT EXISTS leaves the existing table as it stands.

### `command.CommandText =`

The independent Tag and Situation and the references that reach them. Like the Example above, both are owned by nothing: each carries its own opaque id, its visible data is never identity, and it is reached only through the association tables below — one per referrer kind. Each association carries the position the Tag or Situation takes *for that referrer*, so the same one may be first under a Meaning and third under a Collocation; the unique index per referrer keeps those orderings free of duplicates. Deleting a referrer removes only its own association rows (ON DELETE CASCADE on the referrer side); the tag_id and situation_id foreign keys deliberately have no cascade, so the entity survives every detach and the store refuses to delete one while any reference still points at it.

### `LSchemaRevision.LSchemaRevisionCreate(connection);`

The operational and history tables close the schema: they carry no lexical ownership and the workspace row points at both entry and revision, so they are created last, in their own file.

### `LSchemaMigration.LSchemaMigrationApply(connection);`

Every table now exists. What an earlier build left in the wrong shape is corrected next, and only then are the indexes created: the unique position indexes cannot be built over rows that still hold the duplicates the migration removes.
