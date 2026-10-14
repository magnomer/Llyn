# Entry

This folder holds the stores for an Entry and everything it owns.
It covers entries with their forms and speeches, the etymology map, lacunae, folds, the note, queries and the draft loaders.
It stands apart from Context because these rows belong to one Entry and go with it when it is deleted.
Every method runs inside a `LDatabaseSession`, so a caller's session around several stores gives one transaction.
Each store binds to the workspace `database` it opens sessions through.

## `LAncestorArchive`

It persists the etymology map of an entry, and the relation and tongue lists the boxes draw from.
`LAncestorSave` replaces the whole map of the entry inside one session.
Its boxes are written breadth-first from the headword, so a child is stored before its sources.
Each box's position is renumbered from zero among its siblings, in the order of the given positions.
A box with no key, or a key another box holds, is refused with an exception, and nothing is written.
So is a box that feeds a box the map lacks, or boxes that feed one another in a ring.
The ids the store gives replace the draft ids, and the child links are rewritten to match.
A target, tongue or relation id that no longer exists is stored as none.
So a stale pick never fails a save.
The table also keeps `language` empty whenever a tongue is set, by a check.
Deleting the entry removes its boxes by cascade.
Deleting an entry that a box links to leaves the box as text.
The link is set to none, and the text holds the old headword.
Deleting a tongue or a custom relation clears it on every box that holds it.
Builtin relations are seeded by the schema and cannot be deleted.
Creating a name already in the list changes nothing and returns the held row.
The name match here is exact, and the clerk above it matches ignoring case.

## `LEntryArchive`

It is the adapter of `LEntryVault`, the port the engine holds.
It persists entries and their two owned child structures, the written forms and the parts of speech.
An entry's id and timestamps are assigned here on creation.
Forms and speeches are ordered child rows, so reordering rewrites `position` only and never touches the entry id.
Deleting an entry removes its forms and speeches through the foreign-key cascade.

`LEntryCreate` inserts the entry with a fresh opaque id and both timestamps, and writes forms and speeches in list order.
Its lexical unit and freedom are stored with the row.
The whole write is one transaction.

`LEntryRead` answers `null` when no entry has the id.
`LEntryLoad` assembles the whole editable content through `LEntryLoader`, which stays this store's helper.

`LEntryUpdate` changes headword, language, lexical unit and freedom, and stamps a fresh modification time.
The id, forms and speeches are untouched.
It throws when no entry carries the id, rather than reporting success for a write that reached nothing.

`LEntryFormSet` and `LEntrySpeechSet` clear the existing rows and write the new set in list order.
So reordering rewrites positions while the entry id stays fixed.

`LEntryEpithetRead` is one scalar read, so every list row can ask without loading the entry.
It gives empty when the entry has none or does not exist.

`LEntryEpithetSave` writes the derived epithet alone.
A value equal to the stored one is not written, so nothing else notices.
`updated_utc` stays where it is, because a derived string is not a user edit.
An id no entry carries is not an error, since the derivation may outlive the entry.

`LEntryGraspSet` writes the user's grasp alone.
`updated_utc` stays where it is, because the word did not change, only the user's view of it.
The value must pass `LGrasp.LGraspCheck`, so nothing outside zero to ten reaches the row.
It throws when no entry carries the id, as `LEntryUpdate` does.
`LEntryCreate` and `LEntryUpdate` never mention the column, so an editor save can never clobber a rating.

`LEntryDelete` removes the entry and everything it owns through the foreign-key cascade.
That covers forms, speeches, inflections with their features, and meanings with their inline definition field.
It covers pronunciations with syllables and recordings, transcriptions, collocations and the note.
It also covers every association row hanging from the entry, its meanings or its collocations.
The independent Examples, Tags, Situations, References and Authors those associations pointed at are left standing.
Only the rows linking them to this entry disappear.

`LEntryRowRead` is the entry row shape every read selects, in one place.
`LEntryQueryArchive` reads through it too, so a single read and a find never drift apart column by column.

## `LEntryLoader`

It reads a stored entry back into the `LEntryDraft` the input form saved.
It owns no SQL of its own.
The cards come from `LEntryLoaderCard` and the pronunciations, transcriptions and reflexes from `LEntryLoaderReading`.
The ancestor boxes come from `LAncestorArchive`, as drafts with their stored ids.
The draft is the shape a draft commit consumes, so a save and a load are inverses.
Everything arrives in stored order, and the entry's lexical unit and freedom are read off the entry row.

The whole composition runs inside one `LDatabaseSession`, with both section loaders built inside it.
So every archive call nests into that session and the draft is a single consistent snapshot.
It is not a dozen independently timed queries that could disagree.

The audio path comes back exactly as stored, relative to the workspace folder.
Resolving it is the engine's job, because the workspace root is what the engine owns.

`LEntryLoad` answers `null` when no entry has the id.
A card's sentences, Situations, Registers, Tags, Translations, Images and Videos are ordered sets in the order the associations record.
So a card referencing three tags loads back with three.
Meanings arrive as the tree the store keeps.
A Collocation never nests, so its children are read nowhere.
Each card carries the id of the row it came from.
So a loaded, edited and committed draft updates that row.
A card the user adds afterwards has no id and is created.

An entry with no etymology gives an empty box list rather than null.

`LEntrySpeechFormat` keeps the value link and the custom name apart, and carries the name to show beside them.
A custom row shows its text.
A row linking a value shows that value's name, or nothing when the value is gone.

## `LEntryLoaderCard`

It reads an entry's Meaning and Collocation cards into the values its draft carries.
`LEntryLoader` builds it inside its own session, so these reads join one snapshot.

`LEntryMeaningRead` gives the Meaning cards as the tree the store keeps.
`LEntryCollocationRead` gives the Collocation cards in stored order, each with the ids of the Meanings it links, in link order.

`LEntryMeaningRead` groups the senses by parent once, and `LEntryChildRead` walks the tree from the roots.
A root is grouped under key 0, because the store writes no parent for one.
Each group is already in stored position order, so none is sorted again.
Position is per sibling group, which is what the store's unique index counts.

`LEntryExampleRead` gives the Example a card's row quotes, or `null` when it quotes none.
A row may state a frame and no sentence when the store holds no Example for it.
That row is real data and must load as itself rather than as an empty sentence.
The stored Glosses and Mentions travel as drafts under their positive ids.
A commit that dropped them would delete every rendering and link the sentence carried.
The draft is built by `LExampleDraft`, so every loader reads a stored Example the same way.

`LEntryCardRead` is the one read path for the independents a card references.
A Meaning card and a Collocation card hold them on identical terms, so only the owner side differs.
The collocation flag says which side the owner id names.
The card's own columns and position arrive already read, with the position raised by one.
Both archives keep positions contiguous from zero, so each caller adds the one.

## `LEntryLoaderReading`

It reads how an entry is said and written: pronunciations with recordings, transcriptions and reflexes.
`LEntryLoader` builds it inside its own session, so these reads join one snapshot.
Every list is in stored order, and each row's id travels with it.
So a save updates that row instead of writing another.
An entry with no pronunciation row carries an empty list.
The audio's added time stays behind, because it is workspace bookkeeping.

## `LEntryQueryArchive`

It is the adapter of `LEntryQueryVault`, the read-only entry port.
It holds every reader that answers with a list of Entries.
That covers the headword match, the text scan and the owner finds over Tag, Register, Situation, Example and Reference.
Each find comes back ordered by headword.
Equal headwords go by language, then entry id, the keys `LCatalogEntry` breaks ties by.
The id comes last, so storage order only parts rows equal on every declared key.
SQLite compares headwords by code point, while the catalog sort folds case by culture.
Every reader selects the lexical unit and freedom, and rows go through `LEntryArchive.LEntryRowRead`.

`LEntryFind` returns every entry when the query is empty or whitespace.
Matching is `lmatch()`, the catalog text match registered on every connection (`LDatabase.LDatabaseConnectionRead`).
It is a case-insensitive contains, and a query with `*` or `?` is read against the whole headword instead.
Prefix weighting, forms and accent folding wait for a stated requirement rather than being guessed at.
Case is folded with the invariant .NET lowercase, not SQLite's `lower()`, which folds ASCII only.
So `Ä` finds `ä`, and Greek or Cyrillic headwords match in either case.
Scripts without case, such as Korean, Japanese and Chinese, are unaffected.
The query is trimmed first, so trailing space left by typing does not narrow the result.

`LEntryHeadwordFind` gives every Entry whose language and headword equal the given ones, letter case folded.
The fold is `lfold()`, the invariant lowercase registered beside `lmatch()`.
It is the candidate list for a clicked word, and zero rows is a legitimate answer.

`LEntryHeadwordScan` gives every Entry of one language whose headword appears inside the text, longest first.
One query serves a language without word separators, where the engine tries each hit against the click.

`LEntryScan` gives the entries among the ids whose headword matches, in id order, in one session.
The cell of a rhyme table reads its entries this way instead of one read per id.

`LEntryEpithetScan` gives the epithet of every entry among the ids that has one, keyed by id, in one session.
An entry with no epithet has no key, so a caller falls back to nothing without a lookup per row.

`LEntryListDivide` cuts the ids into batches of five hundred, well under what one statement may bind.
`LEntryListFormat` binds one parameter per id and answers the list to put inside the `IN`.
`LEntryListRead` gives the rows a prepared query yields, in the order the query states.

`LEntrySituationFind`, `LEntryExampleFind` and `LEntryReferenceFind` return every entry when the id is zero.
A situation or an example is held on a meaning or a collocation.
So both sides are unioned into one row per entry.
A Source is cited by a card holding an Example that cites it.
So that walk goes through the example row.
`LEntryOwnerFind` runs these three, binding the owner id as `$owner`, since they differ only in the link tables walked.

`LEntryTagFind` and `LEntryRegisterFind` also return every entry when the id is zero.
A tag or a register is held on a meaning or a collocation, never on the entry itself.
So both sides are asked and the entries they name are unioned.
An entry tagged on several cards is still one row.

`LEntryCountRead` counts entries in one statement.
The status bar prints it, so nothing is loaded that the count alone answers.

## `LLacunaArchive`

It reads and writes the lacuna rows one entry carries, one per paradigm slot the web could not fill.
`LLacunaRead` gives the rows in written order.
An older workspace may hold a row without a morphology value.
The lacuna keeps its nullable id and cell key for the reader to resolve.
A row stored before cells existed carries an empty cell.

`LLacunaSave` replaces the entry's rows in one transaction, each keeping its morphology id and cell key.
An empty list leaves no rows, and every row is stamped with the same fetch time.
`LLacunaDelete` drops the rows, so a hand-edited entry is asked afresh.
`LLacunaClear` empties them inside the caller's session.

## `LFoldArchive`

Fold tables keep view-state marks separate from lexical rows.
Each card or entry has at most one mark per table, except one per series key in `stem_fold`.
Foreign-key cascades remove marks with their owning card or entry.
Meaning folds use `sense_fold` and Collocation folds use `collocation_fold`.
Reflex openings use `reflex_fold`, and box openings use `fanqie_fold` or `script_fold`.
Series member openings use `stem_fold`, keyed by entry and series key.
The clerk owns nonpositive-id filtering, so this adapter takes ids as supplied.

`LFoldSave` selects each parent card before insertion, which avoids missing-parent foreign-key failures.
Conflict handling makes repeated folding idempotent.
Shared card identity lets only the owning card table contribute a mark.

`LFoldDelete` works on both card tables with the same id, so callers need not supply the card kind.
Lexical card rows remain untouched.

`LFoldRead` combines both card kinds into an entry-scoped set in one query.
The Meaning join also includes sub-meanings carrying that entry parent.

The reflex, box and stem spread methods follow one rule: a row means opened, and no row means closed.
Parent selection prevents missing entries from acquiring marks.
Conflict handling makes repeated opening idempotent.
Each box owns its table, keeping its state independent of other boxes.
Each check answers true only for a stored mark in the selected table, or for the entry and series key.

`LFoldTableSelect` is a fixed enum-to-table mapping, which keeps caller-supplied text out of SQL identifiers.
Unknown boxes are rejected instead of naming an arbitrary table.

## `LNoteArchive`

It persists the single Note an entry owns.
The table is keyed by `entry_parent`, so at most one Note exists per entry.
Saving replaces an existing Note rather than adding a second.
Deleting the entry removes its Note through the foreign-key cascade.
The text is stored exactly as given, and the engine hands it over as normalized Markdown.
`LNoteRead` answers `null` when there is no Note.

## Inline notes

### LEntryArchive.cs: bool declared = speech.LSpeechValueId is > 0

A row links a declared value or carries typed text, never both.
The table's CHECK says so.
A caller handing over both would otherwise write a row that resolves one way and displays another.
The id wins, because a name matching a value is that value.

### LEntryLoaderCard.cs: ownerId)

The card says which stored row it is.
So a draft handed back to the engine updates that row rather than reading as a new card.

## Stamp

Commit: `c6e60141a172`
- `LAncestorArchive.cs` `190e9703f2a0033d`
- `LEntryArchive.cs` `b6b08d8105ccbbba`
- `LEntryLoader.cs` `b31b67e2a6b3587e`
- `LEntryLoaderCard.cs` `8f7544d1b4dd3a83`
- `LEntryLoaderReading.cs` `d637332d8f11d2c5`
- `LEntryQueryArchive.cs` `ed0a6c5e048971e1`
- `LFoldArchive.cs` `f066ca03fb72b2a4`
- `LLacunaArchive.cs` `a55a2ef1f0c5bbbf`
- `LNoteArchive.cs` `74e55e205494b7b9`
