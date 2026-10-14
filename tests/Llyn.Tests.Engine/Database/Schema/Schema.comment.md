# Schema

This folder holds the tests of the schema runner and its migrations and version steps.
They also cover the realm a fresh schema seeds.

## `TSchemaMigration.cs`

Covers the schema runner and the migration that rebuilds a database another build wrote.
Creating twice leaves one version row, and it holds the current version.
A database at any other version, older or newer, is rebuilt in the current shape rather than refused.
Every column both shapes know carries across, and a column only the old shape knew is dropped.
A copy of the old file stays beside it under its version.
The file itself is never replaced, and no scratch file is left once the rebuild is done.
A rebuild that fails, here because its scratch file cannot be created, leaves the old file untouched.
It keeps its version and every row.
The realm the workspace was minted under survives the rebuild, so rows made here keep their stamp.
A row the current schema cannot hold, such as a child whose parent is gone, is dropped.
The launch goes on without it.
The id counters carry across, so a rebuilt workspace never hands out an id a tombstone remembers.
Other migration facts live in `TSchemaRealm`, `TSchemaMigrationReflex`, `TSchemaMigrationReference` and `TSchemaMigrationVersion`.

## `TSchemaMigrationReference.cs`

Covers the Unknown Reference the schema seeds and the old wording that points at it.
A new workspace holds exactly one Reference, titled Unknown.
An example that wrote `unknown` as its source state is pointed at that row after the rebuild.
The rebuild mints no second row when the workspace already holds one.
The row is minted again only for old wording, since the user may delete it.
Opening a workspace again never re-seeds a deleted row, so deleting it holds.

## `TSchemaMigrationReflex.cs`

Covers how the rebuild carries the reflex rows an older build wrote.
A build older than the note drops every reflex row and keeps its entries.
A build older than the region keeps each row, its old note becoming the romanization.
A version-71 reflex carries its old note into romanization and its old remark into the new note.
Its new meaning is empty and its ownership flag is clear.

## `TSchemaMigrationVersion.cs`

Covers the rebuild steps that belong to one older version of the schema.
Each fact writes a workspace as that version left it, then rebuilds it.
A build without a table gets the table added empty, and every other row carries across.
This holds for the Situation media tables and the collocation link table.
A version-81 build holding the old `etymology`, `etymology_mention` and `etymon` tables drops all three.
Its entries stay, and the version row reads the current version.
The Situation media rebuild leaves exactly one `*.v47.db` copy in the workspace folder.
An older workspace numbered Collocations apart from Meanings, so the first of each was number 1.
The migration moves the Collocation above both sequences, and its links and history follow it.
The Meaning and a Collocation with a number of its own keep theirs.
A register named twice per language keeps one row, and every sense still carries its mark in order.
An entry frequency moves into the frequency table, and the old column is dropped.
A value naming a source is split into source and raw reading.
A bare value with no source gains no row, and neither does an empty one.
The retired content and function units, numbered 1 and 2, are cleared to unchosen.
Their entries carry across, while morpheme and word keep their numbers.
Every freedom comes across unchosen, since the column is new.

The version-82 rebuild keeps a cue whose Meaning is gone, with its reference set to none.
A cue whose deck is gone is swept, and its cells go with it.
A cue bound to a renumbered Collocation follows the new number.
A cue bound to a Meaning that the sweep itself deletes stays too, orphaned.
That last fact fails with one settle before the sweep, so it pins the settle in every sweep round.

## `TSchemaRealm.cs`

Covers the realm row, the origin stamp and the id counters of a fresh workspace.
The realm row is single, minted once, and kept when the schema runs again.
A row made here carries the realm and its own id as its stamp.
A row from elsewhere keeps the stamp it arrived with, and a half-written stamp is refused.
Tables that carry history never reuse an id, and a temporary id is never handed out twice across restarts.

## Inline notes

### TSchemaMigration.cs: Assert.Equal(1, TSchemaIndexRead(workspace, "collocation", "entry_parent"))

A child column with no index turns each parent delete into a full scan of the child table.
So the index set is part of the schema rather than an optimization applied later.

### TSchemaMigration.cs: TSchemaIndexRead

It asks whether some index on the table has the named column first.
That is what the foreign-key check and the cascade actually use.

### TSchemaRealm.cs: TSchemaAutoincrementRead

It asks whether the table was declared with AUTOINCREMENT.
That is what stops SQLite reusing a deleted id.

## Stamp

Commit: `aae5349dbb94`
- `TSchemaMigration.cs` `59dc0c10918ea99c`
- `TSchemaMigrationReference.cs` `720f13444005e50b`
- `TSchemaMigrationReflex.cs` `afa08e2720b02717`
- `TSchemaMigrationVersion.cs` `4657fd7cc52fb9f6`
- `TSchemaRealm.cs` `c8fcae2f1ea45a3b`
