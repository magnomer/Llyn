# Base

This folder holds the schema runner and the schema areas that belong to no single lexical group.
It covers the build sequence, version, migration, indexes, stamps, and the Example, marker, anchor, citation and favorite tables.
It stands apart from its neighbours because these pieces run for the whole file rather than for one Entry area.

## `LSchema`

It runs the schema build in order.
Each area owns a file of its own and creates the tables it is named for.
This file gives table creation, version validation, indexes and stamp creation one explicit sequence.

Creating a table is only half of it, because `IF NOT EXISTS` leaves an existing table untouched.
So a change to a table an earlier build created is carried by `LSchemaMigration`.
`LSchemaMigration` validates or creates the version row through `LSchemaVersion`.
The lookup indexes live in `LSchemaIndex` and are created after the migration.
Version validation precedes indexes, so an incompatible version cannot pass silently.

`LSchemaCreate` creates missing tables, validates the version, then creates indexes and stamps.
An incompatible version throws rather than rebuilding here.
Callers must rebuild older databases before running this initializer.

## `LSchemaVersion`

It owns the `schema_version` table itself.
That means whether a database carries one, what it records, and the shape that permits exactly one row.
Every other migration step reads a table the workspace owns.
This one reads the table the migration keeps about itself.

`LSchemaVersionExist` says whether the database has ever carried a version row.
Absent means the file is new.
`LSchema` has just created every table at the current shape, so there is nothing to migrate.
`LSchemaVersionCreate` creates the table in its single-row shape and stamps this build's version.
`LSchemaVersionRead` gives the version recorded.

## `LSchemaMigration`

It reads the version an existing database was built at and brings the file to the version this build produces.
It exists because `CREATE TABLE IF NOT EXISTS` leaves an existing table exactly as it stands.
A column, constraint or foreign key added to an already created table never reaches such a file.
Stamping a version without acting on the one already stored would record a shape the file does not have.

No step climbs from one version to the next.
A file at another version is rebuilt instead.
A fresh database is created in the current shape and the old rows are carried into it.
Shared columns carry across.
Old-only columns are discarded, and new-only columns take the target defaults.
A row the current shape refuses, or one whose parent did not come across, is dropped.
Neither stops the launch.

The rows then return into the old file, whose tables are replaced under one transaction.
The file itself is never moved or renamed.
So a crash at any point leaves it whole, at one version or the other.
A copy of the old file is kept beside it under its version, so nothing is lost to a rebuild.
The same rebuild serves a file written by a newer build, since what this build cannot read it cannot keep.

`LSchemaMigrationVersion` is the schema version this build produces, and a change to any table raises it.

The private constants name the fixed pieces of the rebuild.
`LSchemaMigrationFresh` is the name the new database is attached under and the suffix of its file while built.
`LSchemaMigrationTable` is `schema_version`, the one table never carried across, because the fresh file already records its version.
`LSchemaMigrationRealm` is the table whose row replaces the one the fresh file minted.
So the workspace keeps the realm it was born under.
`LSchemaMigrationSequence` holds the id counters of the autoincrement tables.
They are carried across so a rebuilt workspace never reissues an id a tombstone remembers.
`LSchemaMigrationMain` is the name the original database keeps throughout the replacement.
`LSchemaMigrationCompanion` lists the fresh database, its WAL and its shared-memory file for cleanup.

`LSchemaMigrationApply` records the version on a database that has never carried one.
It throws when the stored version is not this build's.
The rebuild runs before the schema and leaves no such file behind.

`LSchemaMigrationCheck` says whether the database was written at another version and must be rebuilt first.
A file with no version table is new, and the schema stamps it rather than rebuilding it.

`LSchemaMigrationRun` rebuilds the file in the current shape and returns the path its copy was kept at.
The pools are cleared first, so no pooled connection holds a stale fresh file while it is deleted.
The fresh file is scratch space and is deleted again whether the rebuild succeeded or failed.
Failures before commit roll back the table replacement.
Scratch cleanup also runs after commit, so a later failure does not undo a committed rebuild.

`LSchemaFileApply` builds the fresh file, copies the old file aside, and attaches the fresh one to the old one.
One transaction first clears the retired lexical units in the old file, so no entry is refused on the way.
It then carries each shared table across.
Along the way it settles old unknown citations and register marks.
It also settles the reflex and frequency rows an older version stored differently.
It dates the stored glyph pictures whose captions still carry their age in the source's own words.
It renumbers every Collocation whose id a Meaning also holds.
It then sweeps the orphans and brings the rows back.
Bringing them back drops every old table, runs the schema on the file, and copies the fresh tables in.
The transaction commits or the old file stays exactly as it was.
Foreign keys are off during the copy, so the order tables are read in decides nothing.

`LSchemaTableApply` copies one table over the columns both shapes name.
Ordinary tables use INSERT OR IGNORE, so conflicting rows can be skipped.
Realm rows replace conflicts, which swaps in the realm the schema minted.
Sequence rows use INSERT after clearing the target, so the id counters replace the target's own.

`LSchemaTableClear` drops every table and view of the old file.
So the schema can create the current shape in their place.
Indexes and triggers go with their tables, and SQLite's own tables cannot be dropped and are left alone.

`LSchemaOrphanSweep` deletes every row whose parent did not come across, until the foreign-key check reports nothing.
Deleting a parent can orphan its own children, which is why the check runs again.

`LSchemaTableRead` lists the tables both schemas hold, in the order the schema created them.
SQLite's own tables are left out, except the id counters.

`LSchemaBackupSave` writes a consistent copy beside the old file and returns its path.
SQLite makes the copy itself, so the file is never moved and no journal is left behind it.
`LSchemaBackupResolve` names the copy by the time and version it was written at.
It steps past a name already taken rather than overwriting it.

`LSchemaColumnRead` gives the column names of one table in one attached schema.
A settle step reads it to learn whether an old file has a column yet.

`LSchemaMigrationDelete` removes the fresh file and its WAL and shared-memory companions, so a retried rebuild starts clean.

## `LSchemaIndex`

It creates the lookup indexes the schema needs but does not get for free.
SQLite indexes a primary key and a unique constraint, and nothing else.
In particular it never indexes the child side of a foreign key.
With `PRAGMA foreign_keys = ON` every parent delete has to prove no child row points at it.
So an unindexed child column turns one delete into a full scan of that table.
The cascade of an Entry delete reaches a dozen of them.

Only the columns no existing key already covers are listed.
A composite primary key indexes its leading column.
So `form (entry_parent, position)` and `reference_author (reference_parent, author_ref)` need nothing for their first column.
The second column needs an index of its own, since an association is searched by it from the other direction.

`LSchemaIndexCreate` creates every missing index, on each startup through `LSchema.LSchemaCreate`.
Existing indexes are left as they are.

## `LSchemaStamp`

It makes stand-alone rows record which realm first created them.
Only a row that can exist on its own carries the mark.
An owned child knows its realm by following the reference to its parent.
Marking it again would store the same fact twice.
`LSchemaStampTable` lists the tables whose rows stand alone, meaning Entry and the shared pool every card points at.

`LSchemaStampCreate` creates for each table the trigger that fills the mark and the index that keeps it unique.
The trigger runs only when the inserted row left `origin_id` at zero, which is what a row made here does.
It then writes this workspace's realm value and the id the row was just given.
A row arriving from a merge already carries both.
So the trigger passes it over and its mark survives the trip.

Filling the mark in the database rather than in each store keeps every insert path honest.
A store cannot forget to stamp a row, because it never stamps one.

Each stamped table also checks that a row naming an origin id carries a sixteen-byte realm.
A row with the id and no realm would be a stamp with half its meaning gone.
So an import that forgets the realm is refused instead of quietly admitted.

The unique index is what makes importing the same file twice harmless.
The second pass finds the mark already present and updates that row instead of adding another.

## `LSchemaCitation`

It owns the one Reference every workspace starts with, the Source titled Unknown.
An Example whose source is not known cites this row like any other Source.
So the example table needs no unknown wording of its own, and the citation exchanges like any Reference.
The row is ordinary data.
The user may delete it, and typing its title in a citation field makes it again.

`LSchemaCitationTitle` is `"Unknown"`.
Migration finds an existing Unknown Source by this exact title.
So changing it would make migration add a second row beside a workspace's first.

`LSchemaCitationCreate` inserts the Unknown Source into a schema and returns its id, stating only the title.
It is called once when a workspace is first created.
Migration calls it when older rows need a row that is gone.

`LSchemaCitationSettle` points every Example still carrying the old `unknown` source wording at the Unknown Source.
Older workspaces wrote that wording before the row existed.
The reading side treats the wording as unreadable now, so migration rewrites it before any read sees it.
The Unknown Source is looked up by title first, so a workspace already holding one gains no second.
Nothing is touched when no Example carries the wording.

## `LSchemaQuotation`

It creates the independent Example and the association tables that reach it.
`LSchemaQuotationCreate` creates the Example, its own Gloss and Mention lists, and the two association tables that cite it.

An Example is owned by nothing.
It carries its own opaque id, its language and display text, and at most one Source reference.
It is reached through two association tables, one per card kind.
Each carries the position the Example takes for that referrer.
So one Example may be first under a Meaning and third under a Collocation.
The unique index per referrer keeps those orderings free of duplicates.
An association id counts strictly upward and is never given to a later row once the row is gone.
A draft holds it while editing.

`sense_example` and `collocation_example` carry data of their own, so each row carries its own opaque id.
A row holds the frame its owner reads the Example under, a particle and a dependence.
Each carries what is known about it.
The frame lives on the association and never on the Example.
So two owners citing one Example keep their own frame.
Its `example_ref` is nullable, so a row may state a frame and cite no Example at all.
The frame is the owner's, so writing one before any sentence exists must not wait on a sentence.
A row citing nothing and stating no frame says nothing, and the CHECK refuses it.
Deleting a referrer removes only its own association rows, by ON DELETE CASCADE on that side.
The `example_ref` foreign keys deliberately have no cascade, so an Example survives every detach.
The store refuses to delete one while any reference still points at it.

`example_translation` is one Gloss, the sentence rendered as text in one language.
A row holds the language, the text with its state, and the position among the Example's Glosses.
It goes with the Example by ON DELETE CASCADE, because a rendering says nothing without the sentence.
The unique index per Example keeps positions free of duplicates, as the association tables do.
One Example may carry several rows in one language, since two renderings of one sentence are both worth keeping.
The table is named after the Translation the schema already speaks of, while the code names the row a Gloss.

`example_mention` is one word of the sentence that stands for an Entry.
A row holds the span as a start and a length counted in code points.
It holds the Entry the word points at, and at most one Meaning of that Entry.
The row lives on the Example and never on the card that quotes it.
So every card citing one sentence reads the same words the same way.
A NULL `entry_ref` is the explicit mark that the word stands for nothing.
The CHECK refuses a Meaning without its Entry.
Two of its foreign keys act although they are references, as `sense_translation` does.
A Mention without its Entry says nothing, so it goes with the Entry.
A Mention whose Meaning went still says the Entry, so it degrades to the Entry by ON DELETE SET NULL.
The unique index refuses two words starting at one place.
Overlap between two spans is not a SQL rule, and the store refuses it instead.
The table is created before the associations because it is the Example's own list.

`example.reference_ref` points at the independent Reference that `LSchemaReference` creates.
Its foreign key could only be declared once that source table existed.
SQLite refuses to prepare any statement writing to a child table whose parent is missing.
That holds even for NULL keys, which is why the reference tables are created first.

## `LSchemaMarker`

It creates the sets a card attaches to itself.
They are its Tags, its translation links, and the independent Situations it reaches.

A Tag is one shared row whose text is unique.
The association row links a card to it by `tag_ref` and holds the position.
The primary key on (card, tag) stops a card carrying the same Tag twice.
The unique index on (card, position) keeps the order free of duplicates.
The association rows cascade with the card.
The `tag_ref` foreign key has no cascade, so the Tag row survives every detach.

The translation tables beside them are the links a card carries to another Entry.
A link stores the target's id and never its text, so it is one card, one entry, one position.
The primary key on (card, entry) stops a card linking the same Entry twice.
The `entry_ref` cascade is deliberate, since deleting a target Entry drops the links pointing at it.

A Situation is shared in the same way.
It carries its own opaque id, and its visible data is never identity.
It is reached only through the association tables below it.
Their `situation_ref` foreign keys deliberately have no cascade, so the row survives every detach.

## `LSchemaAnchor`

It creates the anchor store, which holds the ties from reflex rows to the fanqie rows they answer.
An anchor belongs to a reflex row and points to a fanqie row, one row per pair.
A reflex row deleted takes its anchors by cascade.
A fanqie row deleted takes them too, so a placement the refetch no longer finds unties itself.
The fanqie save upserts on the natural key, so a refetch keeps the ids and the anchors stand.

## `LSchemaFavorite`

It creates the favorite marks, one row per marked Entry.
The mark is no lexical object, so it holds nothing but the entry it stands on and its stamp.
The cascade drops the mark with the entry it marks.
The stamp is indexed because the favorites panel orders by it.

## Inline notes

### LSchema.cs: LSchemaReference.LSchemaReferenceCreate(connection)

The Author and Reference tables come before the Example tables.
Reference creation precedes quotation creation, keeping their dependency visible in the initialization sequence.

### LSchema.cs: LSchemaFold.LSchemaFoldCreate(connection)

The fold marks come after entry, sense and collocation.
Creating the parent tables first keeps the fold tables' ownership dependencies visible.

### LSchema.cs: LSchemaAnchor.LSchemaAnchorCreate(connection)

The shengfu table comes after fanqie, since both are read for one character of a headword.
The anchor table comes after reflex, fanqie and diwei.
The sequence places both foreign-key parents before the anchor table, making their ownership dependencies visible.

### LSchema.cs: LSchemaRevision.LSchemaRevisionCreate(connection)

The operational and history tables close the schema.
They carry no lexical ownership, and the workspace row points at both entry and revision.
So they are created last.

### LSchema.cs: LSchemaMigration.LSchemaMigrationApply(connection)

Every table now exists.
The version is now stamped or checked, not rebuilt.
Indexes follow only after that validation succeeds.

### LSchemaIndex.cs: command.CommandText = (first statement)

It indexes lexical rows owned by an Entry or a Meaning whose owning column carries no key of its own.
An Example's Reference is here too.
So is the Meaning side of a collocation link, so deleting a Meaning does not scan the links.

### LSchemaIndex.cs: command.CommandText = (second statement)

It indexes the member side of every association table, and the shared entities those members name.
The two references a Mention carries are listed here as well, since both cascade from their parent.
The target of an etymology span and the target of an Etymon are here for the same reason.

### LSchemaIndex.cs: command.CommandText = (third statement)

It indexes operational rows pointing at lexical rows and at history.

### LSchemaIndex.cs: command.CommandText = (fourth statement)

It gives a unique position to the ordered sets that were left without one.
They are a collocation, a pronunciation, a transcription and an Etymon, each within its Entry.
The translation links a card carries are ordered the same way and take the same index.
Every other ordered set already has one.
Without it a duplicate position is silently possible and the read order becomes arbitrary.
Any duplicate an older database still holds is renumbered by the migration that precedes this call.

## Stamp

Commit: `c2f335c331fe`
- `LSchema.cs` `b30f64ac78e409eb`
- `LSchemaAnchor.cs` `269b59734b8ecf68`
- `LSchemaCitation.cs` `8f67c1c2b7d5273d`
- `LSchemaFavorite.cs` `e20098526745e8aa`
- `LSchemaIndex.cs` `642f88e749df6a26`
- `LSchemaMarker.cs` `7f186069510a11a3`
- `LSchemaMigration.cs` `19e387f890aa14a8`
- `LSchemaQuotation.cs` `6e6e272cb61c61fa`
- `LSchemaStamp.cs` `53e8d23fe6a0faee`
- `LSchemaVersion.cs` `331b3131521e536f`
