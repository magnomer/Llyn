# LSchema.cs

## `public static class LSchema`

Creates the database schema idempotently.
Each job adds its own `CREATE TABLE IF NOT EXISTS` statements here.
It may add them in a per-area file it owns instead.
So the schema grows one job at a time without ever dropping what an earlier job built.

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

### `command.CommandText =`

Data-contract names (table and column identifiers) are persisted keys, so they stay lowercase and independent of code member names.

The Entry root, its owned written forms and parts of speech, and the language-controlled POS display vocabulary.
Owned child rows carry an (entry_id, position) identity and cascade when their Entry is deleted.

A part_of_speech row says its part of speech one of two ways and never both.
value_id names a preset the language declares, whose display name lives in part_of_speech_value.
custom_name carries text the user typed that no preset names.
The CHECK keeps the two from drifting into a row that is half id, half name.

### `command.CommandText =`

Entry-owned inflected forms and their ordered grammatical features, plus the language-controlled morphology display vocabulary.
A feature's identity is the two-level owned key (entry_id, inflection_position, position).
It cascades when its inflection is deleted.
An inflection cascades when its Entry is deleted.
Lexical rows store only stable ids — display names live in morphology_value.

### `command.CommandText =`

The Meaning tree an entry owns.
Each meaning is a stable-id node that may nest under another meaning in the same entry.
It carries a single inline definition field.
A meaning cascades when its parent meaning is deleted and when its Entry is deleted.
Sibling ordering is unique within a parent.
The ifnull() expression index treats root meanings (null parent) as one sibling group.

### `command.CommandText =`

Lexical relations originating from a Meaning.
Each relation hangs from its origin meaning and points at exactly one target.
The target is reached through a checked reference row.
It is an Entry (relation_entry) XOR another Meaning (relation_sense).
The relation_id primary key on each target table allows at most one target row per relation.
The store enforces the XOR across the two tables.
A relation cascades when its origin meaning is deleted, and its target row cascades with it.
The referenced Entry or Meaning is never touched.

### `command.CommandText =`

The single pronunciation an entry owns and its two owned child structures.
An entry carries at most one pronunciation, enforced by the unique entry_id.
The pronunciation itself carries no position.
It owns ordered syllables and representations keyed by (pronunciation_id, position).
A syllable requires only its nucleus.
Every other syllable field and a representation's local_tone are optional.
They store NULL when absent, which is distinct from empty.
The pronunciation also owns at most one downloaded recording.
pronunciation_audio has no id of its own, since pronunciation_id is its primary key.
Its file is stored relative to the workspace folder.
So a moved or copied workspace keeps its audio.
Children cascade when their pronunciation is deleted, and the pronunciation cascades when its entry is deleted.

### `command.CommandText =`

The collocations an entry owns and the single Note it owns.
A collocation is a stable-id row ordered within its entry.
It carries both an expression and the meaning that explains it, and the card has a field for each.
A meaning carries a definition instead.
Reordering rewrites position only.
A collocation's synonym is an interlink, not owned text.
It uses the same discriminated target model as a job05 relation, an Entry XOR a Meaning.
The XOR is enforced by a check constraint.
Each target column is a checked foreign key.
So the referenced row must exist and is never touched by the link.
The note table has no id and no position.
entry_id is its primary key.
That makes at-most-one Note per entry a schema fact.
Everything here cascades when its entry is deleted, and a synonym cascades when its collocation is deleted.

### `command.CommandText =`

The independent Author and the independent bibliographic Reference.
They are created before the Example block below.
So example.source_id can carry its foreign key.
Both are owned by nothing.
An Author is shared by any number of References.
A Reference is cited by any number of Entries and Examples without belonging to any.
Every Reference field is stored as a state column plus a value column.
The state says whether the field was never filled in, was recorded as unknown, or holds a value.
The value column is NULL unless the state is 'specified'.
The check constraints make that a schema fact rather than a store convention.
author_state has no value column of its own, because the authors themselves are the value.
They are attached in order through source_author.
That table cascades from its Reference.
So deleting a Reference drops its author links and never an Author.
entry_source cascades from its Entry only.
So the source_id and author_id foreign keys deliberately have no cascade.
The stores refuse to delete a Reference or an Author while anything still points at it.

### `command.CommandText =`

The independent Example and the references that reach it.
An Example is owned by nothing.
It carries its own opaque id, its language and display text, and at most one Source reference.
It is reached through the three association tables below, one per referrer kind.
Each association carries the position the Example takes *for that referrer*.
So one Example may be first under an Entry and third under a Meaning.
The unique index per referrer keeps those orderings free of duplicates.
sense_example is the one association with data of its own, so it carries its own opaque id.
It holds the frame the Meaning reads the Example under, a marker and a role, each with what is known about it.
It also holds the revision, which is a second Example row standing for the rewritten sentence.
Both live on the association and never on the Example.
So two Meanings citing one Example keep their own frame and their own rewrite.
Deleting a referrer removes only its own association rows, by ON DELETE CASCADE on that side.
The example_id foreign keys deliberately have no cascade.
So an Example survives every detach.
The store refuses to delete one while any reference still points at it.
The translation is a column on the Example row, so it goes when the row goes.

example.source_id points at the independent Reference the block above creates.
Its foreign key could only be declared once that source table existed.
SQLite refuses to prepare any statement writing to a child table whose parent is missing.
That holds even for NULL keys, which is why the source block runs first.
A database created before this version keeps the column without the constraint.
CREATE TABLE IF NOT EXISTS leaves the existing table as it stands.

### `command.CommandText =`

The Tags a card carries and the independent Situation the cards reach.
A Tag has no table of its own, because its text is its identity.
So the association row *is* the Tag.
It is one card, one text, and the position that text takes on that card.
The primary key on (card, text) stops a card carrying the same Tag twice.
The unique index on (card, position) keeps the order free of duplicates.
Both cascade with the card, since a Tag no card writes is not data left behind.
The translation tables beside them are the links a card carries to another Entry.
A link stores the target's id and never its text, so it is one card, one entry, one position.
The primary key on (card, entry) stops a card linking the same Entry twice.
The entry_id cascade is deliberate, since deleting a target Entry drops the links pointing at it.
Situation is the opposite and keeps the older shape.
It carries its own opaque id, and its visible data is never identity.
It is reached only through the association table below.
That table's situation_id foreign key deliberately has no cascade.
So the row survives every detach.

### `CREATE TABLE IF NOT EXISTS video (`

The independent Video and the two associations that reach it.
It is shaped exactly like the image block above, because a Video is kept exactly like an Image.
It carries its own opaque id and the location it plays from, and nothing about it is identity.
The association cascades with the card and never with the Video.
So a Video survives every detach, and the store refuses to delete one anything still points at.

### `CREATE TABLE IF NOT EXISTS favorite (`

The favorite marks, one row per marked Entry.
The mark is no lexical object, so it holds nothing but the entry it stands on and its stamp.
The cascade drops the mark with the entry it marks.
The stamp is indexed because the favorites panel orders by it.

### `LSchemaRevision.LSchemaRevisionCreate(connection);`

The operational and history tables close the schema.
They carry no lexical ownership, and the workspace row points at both entry and revision.
So they are created last, in their own file.

### `LSchemaMigration.LSchemaMigrationApply(connection);`

Every table now exists.
What an earlier build left in the wrong shape is corrected next.
Only then are the indexes created.
The unique position indexes cannot be built over rows that still hold duplicates.
The migration removes those duplicates.
