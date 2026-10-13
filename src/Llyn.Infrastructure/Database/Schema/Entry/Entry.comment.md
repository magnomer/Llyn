# Entry

This folder creates the tables an Entry owns, the records an Entry points at, and the realm and history tables.
Each file builds one family of tables, so one file holds one responsibility.
Table and column names are persisted keys.
They stay lowercase and independent of code member names.

## `LSchemaEntry`

Creates the Entry root, its written forms, its parts of speech and the part-of-speech vocabulary.
Owned child rows carry an (entry_parent, position) identity and cascade when their Entry is deleted.

speech_value is unique on (language, pack_code), and pack_code is the id the pack file gave.
So a re-seed upserts, and a rename keeps the row id.

A part_of_speech row names its part of speech by link or by text, never both.
speech_value_ref links a value the language declares.
custom_name carries text the user typed that no value names.
The CHECK stops a row from being half link and half name.

grasp is the user's half-step score of how well they know the word, zero until rated.
The CHECK keeps it within the ten half steps five stars hold.

epithet is the string lists print after the headword, derived from the reflex rows and stored.
It is written whenever the reflex rows are, so a list reads one column and runs no rule.
An older database arrives with it blank, and the engine fills it once.

unit and freedom hold the number of an `LUnit` or `LFreedom` member.
Zero means not chosen, and the CHECK keeps each within the members.
An older database arrives with every unit and freedom unchosen.

## `LSchemaEntrySettle`

Clears every unit number the current shape refuses, in the old file before its rows are carried.
Numbers 1 and 2 held the retired content and function units.
Without this step the CHECK would refuse those rows, and their entries would be lost.
A file from before the unit column has nothing to clear.

## `LSchemaMeaning`

Creates the Meaning tree an Entry owns.
Each meaning is a stable-id node that may nest under another meaning of the same entry.
Its id only counts upward and is never given to a later meaning once the row is gone.
Meanings and Collocations draw ids from one space, `LSchemaCollocationSequence`.
A revision names a deleted meaning by that id.
A reused number would point the history at a living row.
A meaning cascades when its parent meaning or its Entry is deleted.
Sibling ordering is unique within a parent.
The ifnull() expression index treats root meanings, which have a null parent, as one sibling group.

## `LSchemaCollocation`

Creates the collocations an Entry owns, the single Note it owns, and the links from collocations to Meanings.
A collocation is a stable-id row ordered within its entry.
It carries both an expression and the meaning that explains it, so the card has a field for each.
A meaning carries a definition instead.
Reordering rewrites position only.
Everything here cascades when its entry is deleted.

collocation_sense links a collocation to the Meanings it explains, ordered per collocation.
Its first link is the collocation's home.
Deleting either side removes the link by cascade.
A collocation with no link rows is real and shows as unlinked.

The note table has no id and no position.
entry_parent is its primary key, which makes at-most-one Note per entry a schema fact.

## `LSchemaCollocationSequence`

The next id a new Meaning or Collocation takes, one past the highest either table has ever used.
A draft and every card request name a card by its id alone, across both lists.
So no Collocation may share an id with a Meaning.
The two sequences are read too, so a deleted row's number is never handed out again.
Both inserts give this id explicitly, which also moves the inserting table's own sequence.
A reused collocation number would point a revision at a living row.

## `LSchemaCollocationSettle`

Gives every Collocation of an older workspace whose id a Meaning also holds a fresh id above both sequences.
It runs in the migration on the file being built, with foreign keys off.
Every table whose foreign key names a Collocation follows the new id, found from the schema rather than listed.
The revision rows naming that Collocation follow too, so the history still points at the same row.
The collocation sequence ends at the last id given, so a later insert starts above it.
A workspace with no shared id is left alone.

## `LSchemaEtymology`

Creates the etymology, etymology_mention and etymon tables.
An entry has at most one etymology row, so entry_parent is unique there.
Each etymology_mention row is one span of that prose and names the Entry the span means.
Each etymon row is one direct link, held in position order under the entry that declares it.
Every row goes when its entry goes, and a link goes when its target entry goes.
A workspace from before version 74 arrives with all three tables empty.

## `LSchemaInflection`

Creates the inflected forms an Entry owns and the morphology vocabulary they read.
morphology_feature belongs to a speech_value row, and morphology_value belongs to a feature.
Both are unique on (parent, pack_code), so a re-seed upserts and keeps ids.

An inflection is unique on (entry_parent, position) and cascades with its Entry.
An inflection_feature links its inflection by id and cascades with it.
It stores only the morphology_value link, because the value knows its feature.

regular, prediction, marks and stamp are derived on store by the pack's rule book.
regular is one when the form is what the paradigm's pattern predicts from the headword.
prediction is the predicted form.
marks are the departing ranges of text as offset:length pairs joined by commas.
An empty marks string means covered with nothing marked.
stamp names the book that made the analysis, so a changed book is noticed and the row analysed again.
Uncovered analysis leaves prediction and marks NULL but stores the examining book's stamp.
NULL analysis is allowed when no book has examined the row.
A row carried from an older version takes NULL in all three.

## `LSchemaLacuna`

Creates the lacuna store, one row per paradigm slot the web could not fill for one entry.
The key is entry, morphology value and cell, so a refetch replaces an entry's rows cleanly.
The cell is the key of a missed multi-value cell, and empty for a one-value slot.
Without it, cells sharing a first value would collide.
An older row takes the empty cell, which is the one-value row it was.
Nullable morphology links preserve older rows and allow identification by a cell key alone.
The fetch time is kept so a later build may age the answer out.
Rows go with their entry, and with their morphology value when a pack drops it.

## `LSchemaFold`

Creates the fold marks, one table per folding surface.
A row's presence is the flag, as with the favorite marks.
For an opening mark, no row means folded, which is the default of both boxes.

- sense_fold and collocation_fold mark a folded Meaning or Collocation card.
- reflex_fold, fanqie_fold and script_fold mark the opened fold or box of that entry.
- stem_fold marks the opened "More readings" fold on one series key's page.

stem_fold is keyed by series key, not series id, since a series rebuild mints new ids.
The marks sit in tables of their own, so the card save path and the draft never see them.
Folding therefore never dirties an entry, never makes a revision and never enters undo.
Each cascade drops the mark with the card or entry it stands on.
Saving updates cards in place, so a mark follows its card across saves.
`LSchemaCollocationSettle` finds collocation_fold through its foreign key and renumbers it with its card.

## `LSchemaReference`

Creates the independent Author and the independent bibliographic Reference.
They are created before the Example tables, so example.reference_ref can carry its foreign key.
Both are owned by nothing.
An Author is shared by any number of References.
A Reference is cited by any number of Entries and Examples without belonging to any.

Every Reference text field is a state column plus a value column.
The state says the field was never filled in, was recorded as unknown, or holds a value.
The value column is NULL unless the state is 'specified'.
The check constraints make that a schema fact rather than a store convention.

kind is one column, because its closed set carries the three states as members of its own.
It stores the word rather than a number, so the file never depends on member declaration order.
author_state has no value column, because the authors themselves are the value.
They are attached in order through reference_author, which cascades from its Reference.
So deleting a Reference drops its author links and never an Author.
The example.reference_ref and author_ref foreign keys deliberately have no cascade.
The stores refuse to delete a Reference or an Author while anything still points at it.

## `LSchemaRealm`

Creates the realm table and seeds the row standing for this workspace.
A realm is the identity of one workspace's database.
Every row a workspace creates is stamped with the realm that first created it.
Identifiers are sequential within a database, so they say nothing alone once two databases meet.
The realm tells them apart, so a merge never mistakes one workspace's row for another's.

The table holds one row.
value is sixteen bytes no other workspace mints.
Rows stamp that value itself, so a stamp means the same in every workspace it reaches.
A local number in its place would be true only inside the database that assigned it.

## `LSchemaRealmRow`

The id the single realm row always carries.
The table checks that no other id exists.
A second row would give one workspace two identities.

## `LSchemaRealmCreate`

The seeded value is minted once and never again.
The insert names the one row and yields to the row already there.
Minting it twice would give one workspace two identities and break every stamp already written.

## `LSchemaRevision`

Creates the operational and history block: workspace row, revisions with ordered changes, and entry tombstones.
These tables carry no lexical ownership.
Nothing here owns an Entry or is owned by one.

Ordering matters.
workspace points at entry and at revision, so this block runs after `LSchema` has created the lexical tables.
revision is created before workspace.
SQLite refuses to prepare a statement writing to a child table whose parent is missing.
That holds even for NULL keys.
A revision id only counts upward and is never given to a later revision once the row is gone.

## Inline notes

### LSchemaRevision.cs: command.CommandText = (revision, change, tombstone)

A revision is a stamped point in history.
Its changes are ordered rows identified by (revision_parent, position).
So a change has no id of its own and cascades when its revision is dropped.
target_ref and target_type name what the change touched without a foreign key.
A change often records a row that no longer exists, which is the point of keeping history.

A tombstone is the same idea for a deleted Entry.
entry_ref deliberately carries no foreign key to entry, because that row is gone when the tombstone is written.
entry_ref is the primary key, so one deleted Entry leaves exactly one tombstone.
The revision it was deleted under is a real reference that must exist.

### LSchemaRevision.cs: command.CommandText = (workspace)

The workspace row is operational state and owns nothing lexical.
It holds which Entry each duplex side shows, the session revision and the identity floor.
The identity floor is the lowest temporary id ever issued, kept at or below zero.
The open tab, its split, each panel's ordering and its hidden languages live in the settings file instead.
Its entry columns are ON DELETE SET NULL, so deleting an Entry empties the duplex side that showed it.
That neither blocks the delete nor drags the workspace row down with it.
Its revision column behaves the same way.

### LSchemaCollocation.cs: LSchemaLinkRead

It lists every table and column in the schema whose foreign key points at the collocation table.

### LSchemaCollocation.cs: LSchemaLinkUpdate

It rewrites one id in one column of a table.
A kind limits the rewrite to revision rows of that target type.

## Stamp

Commit: `c2f335c331fe`
- `LSchemaCollocation.cs` `e90e598319da5101`
- `LSchemaEntry.cs` `869d40d6d41025eb`
- `LSchemaEtymology.cs` `8a11e48f9823be89`
- `LSchemaFold.cs` `8f54d54779c68955`
- `LSchemaInflection.cs` `39ce71c62dea519e`
- `LSchemaLacuna.cs` `1ff0cd476f3a833c`
- `LSchemaMeaning.cs` `be3ae1bf1fd02d8a`
- `LSchemaRealm.cs` `5408c0396c86b1d0`
- `LSchemaReference.cs` `479eaa8f5a7ece2f`
- `LSchemaRevision.cs` `d178b1555440da88`
