# Sound

This folder holds the SQLite adapters for pronunciation and writing data.
It stores what an entry sounds like, how its characters were placed in rime books, and how they are drawn.
Some stores hang off an Entry, such as pronunciation, transcription, reflex and frequency.
The fanqie, script, shengfu, stem and diwei stores hang off a language and character instead.
A character is shared by every entry written with it, so these rows outlive any one entry.

## `LPronunciationArchive`

Persists the ordered pronunciations an entry owns, each with its ordered syllables.
The id is assigned here on creation.
Children carry pronunciation_parent and position from that id and list order, so reordering rewrites positions only.
A new pronunciation is appended after the entry's others, and `LPronunciationOrderSet` places the whole list.
Deleting removes the syllables and the recording through the foreign-key cascade.

A pronunciation also owns at most one downloaded recording, kept beside the aggregate rather than inside it.
`LPronunciationAudioSave` and `LPronunciationAudioRead` write and read that one row.
Its file path is stored relative to the workspace folder.

## `LPronunciationCreate`

Inserts with a fresh opaque id, placed after the entry's other pronunciations.
The position it carries is ignored, because `LPronunciationOrderSet` settles the entry's order.
A blank variety, IPA or respelling is stored as NULL.
Syllables are written as ordered child rows, and the whole write is one transaction.
It returns the stored pronunciation with its id, position and children filled in.

## `LPronunciationRead`

Reads the entry's pronunciations in position order, each with its ordered syllables, empty when none exist.

## `LPronunciationUpdate`

Replaces the variety, the IPA, the respelling and the syllables of one pronunciation.
Existing syllable rows are cleared and the supplied list written in order.
So the id, entry link, position and identity stay fixed.
The whole write is one transaction, and it throws when no pronunciation carries that id.

## `LPronunciationOrderSet`

Renumbers the entry's pronunciations so the given order is their position order.
The first id becomes the primary pronunciation.

## `LPronunciationDelete`

Deletes one pronunciation and closes the gap it leaves in its entry's order.

## `LPronunciationAudioSave`

Records a path relative to the workspace folder as the recording the pronunciation owns.
It also records the source label it was downloaded from and the moment it was stored.
A second save for the same pronunciation replaces the first rather than adding a row.

## `LPronunciationAudioRead`

Reads the recording, or null when there is none.
The path comes back exactly as stored.
So the caller that knows the workspace resolves it against the folder in use now.

## `LPronunciationAudioScan`

Returns every recording file the database names, as stored, in one statement.
The sweep that drops orphaned recordings reads the kept set through it.

## `LTranscriptionArchive`

Persists the ordered transcriptions an entry owns, one row per scheme.
The id is assigned on insertion and survives every later save.
So a draft and a shell may hold it across saves.
The whole list of an entry is written at once.
The rows are few, and their order and schemes are one fact.

## `LTranscriptionSet`

Makes the given list the whole list of the entry, in the order given.
A row with a positive id is rewritten in place under that id, so the id survives the save.
A row with no id is inserted and given one.
A stored row the list no longer names is deleted.
It returns the stored rows with ids and positions filled in.
The whole write is one transaction.
It throws when a positive id names no row of this entry, and when two rows share a scheme.

## `LTranscriptionRead`

Reads the entry's transcriptions in position order, empty when none exist.

## `LTranscriptionAsideMove`

Every surviving row is moved out of the way before the list is written back.
Its position is shifted far up and its scheme is prefixed with a character no scheme name carries.
Without this, rows swapping schemes or moving onto another's position would hit the unique constraints mid-write.
Every shelved row is rewritten by the loop that follows, so nothing stays shelved.

## `LReflexArchive`

Persists the ordered reflexes an entry owns, one row per reading.
The id is assigned on insertion and survives every later save.
So a draft and a shell may hold it across saves.
The whole list of an entry is written at once.
The rows are few, and their order is one fact.

## `LReflexRead`

Reads the entry's reflexes in position order, empty when none exist.
The eight anatomy columns come back as one `LAnatomy` on each row, and the anchors as their sorted ids.

## `LReflexAnchorScan`

Returns the reflex rows anchored to each fanqie row of one Diwei category, keyed by fanqie id.
A fanqie row nothing is anchored to has no key, so a tally over it counts nothing.
Each row carries its own entry id and its anchors, as a read would give it.

## `LReflexSet`

Makes the given list the whole list of the entry, in the order given.
A row with a positive id is rewritten in place under that id, so the id survives the save.
A row with no id is inserted and given one.
A stored row the list no longer names is deleted.
The anchors of each row are rewritten after the row, under the id the row now has.
The whole write is one transaction.
It throws when a positive id names no row of this entry.
No column is unique, so no row needs shelving before the write.

## `LReflexAnchorSave`

Rewrites the anchors of one stored row by dropping every old pair and writing each id of the row.
An id no fanqie row carries is skipped, so a stale draft never breaks the save.

## `LReflexRowSave`

Updates one existing row and refuses an id that belongs to no row of the entry.

## `LReflexLeftoverDelete`

Deletes each stored row of the entry whose id the replacement list did not keep.

## `LReflexRowRead`

Reads one reflex row from the twenty columns every reflex select lists in the same order.

## `LReflexAnchorLoad`

Attaches the anchors to the rows, with one small query per row.

## `LReflexParameterApply`

Adds the column values of one row to a write, shared by the update and the insert.
`LReflexAnatomyApply` adds the eight anatomy values after the other columns.

## `LFanqieArchive`

Stores and reads the rime-book placements of a character, per language.
A character's rows are written as one set and read back in the order they were first written.
A row keeps its id across refetches, so the anchors reflex rows hold on it stand.

## `LFanqieSave`

Makes the given rows the whole set of the character, in one transaction.
A row already stored under the same book, source and position is rewritten in place under its id.
A row not stored yet is inserted, and a stored row the fetch no longer gives is deleted.
The books arrive in pack order and the positions in table order, and a first save's ids keep that order.
A representative rank survives the rewrite, and the ranks are closed up when a ranked row is deleted.

## `LFanqieLeftoverDelete`

Deletes the character's rows the save did not write, and their diwei links and anchors by cascade.

## `LFanqieRead`

Returns every stored row of the character in insertion order, with the parts the line was built from.
The reading and tone class come back as stored, so no hypothesis runs on read.
An empty list means the character was never fetched, or nothing was found and nothing stored.

## `LFanqieRepresentativeSet`

Places the row at the asked rank among the character's representative readings, zero unmarking it.
Every other ranked row shifts, so the ranks always run from one without a gap.
A rank past the end lands the row last, which is how a fresh mark is asked for.
The ranked ids are read in rank order, the id breaking a tie that two equal ranks leave.
`LFanqieRankApply` numbers the given ids from one in arrival order, and `LFanqieRankSave` writes one rank.

## `LDiweiArchive`

Derives and reads the 音韻地位 categories of a language from its stored fanqie rows.
The categories follow the fanqie store and the hypothesis, never the user.
The entries a category counts follow the anchors the user drew from reflex rows to fanqie rows.

## `LDiweiArchiveTally`

The count of entries in a category.
An entry counts when it owns a reflex row anchored to a linked fanqie row.
An entry with no anchored row is not counted, whatever its headword.

## `LDiweiApply`

Rebuilds the links of one character after its fanqie rows were stored.
Categories left without a link in the language are dropped afterwards.

## `LDiweiRebuild`

Drops every category of the language and derives them again from every stored character.
It runs when the engine binds a workspace.
The tone classes depend on the hypothesis file as it now reads.

## `LDiweiRead`

Reads the categories of one kind with their entry counts, in the order they were first made.
The id overload reads one category with its count, or null when none is stored under it.

## `LDiweiFind`

Returns the category with this key and its count, or null when no placement carries it.

## `LDiweiEntryScan`

Returns the entries owning a reflex row anchored to one fanqie row linked to every given category at once.
So an initial and a rime together name one cell of the rime table, not two separate sets.
Empty ids give nothing.

## `LDiweiFanqieRead`

Returns the fanqie rows linked to one category that some reflex row is anchored to, in storage order.
Each carries its character and id.
A placement nobody anchored is left out, so the page lists no character without an anchored reading.

## `LDiweiCharacterApply`

Reads the character's fanqie rows with their ids, clears each row's links and writes them afresh.
The hypothesis is run once per row here.
Its result is stored on the row and keyed into the links.
So the reading view prints a stored reading and never runs the hypothesis itself.

## `LDiweiReadingSave`

Writes the derived reading and tone class onto the fanqie row, or blanks both when the hypothesis gave none.
A row already holding the same pair is not written, so a rebuild at every open dirties nothing.

## `LDiweiKeyScan`

A fanqie row carries up to three category keys.
They are its initial, its rime keyed with division and 開合, and its tone class.
The tone class comes from the sound the hypothesis derived.
A row it could not resolve links to no tone.
An empty part is skipped.

## `LDiweiRowCreate`

Returns the category id, making the category when it is not there yet.
`LDiweiLinkCreate` writes one link from a fanqie row to a category and ignores one that already stands.

## `LDiweiOrphanClear`

Drops the categories of the language that no link points to any more.

## `LFrequencyArchive`

Reads and writes the frequency rows one entry carries, one row per pack source that answered.

## `LFrequencyRead`

Reads the entry's rows by source name, only so the result is stable.
A refetch rewrites the rows, so write order carries no meaning.
The engine reorders them by the pack's declared sources.
The word interval is not stored, so the rows come back without it for the engine to derive.

## `LFrequencySet`

Replaces the entry's rows with the given set in one transaction.
Every row is stamped with the same fetch time.

## `LFrequencyClear`

Drops the entry's rows, so a renamed entry is fetched afresh.

## `LFrequencyBandSet`

Stores the band of one row after the engine resolved it, which a migrated row needs once.

## `LFrequencyDelete`

Deletes the entry's rows inside the caller's session.

## `LScriptArchive`

Stores and reads the glyph pictures of a character, per language.
A character's pictures are written as one set and read back in the order they were written.
That order is storage order, so `LScriptImage.LScriptImageSort` decides what a view shows.

## `LScriptSave`

Replaces whatever the character held with the given pictures, in one transaction.
The styles arrive in pack order and the positions in source order, and the row ids keep that order.

## `LScriptRead`

Returns every stored picture of the character in insertion order, with its bytes, age code and row id.
The row id is carried only as the last tie-break of `LScriptImage.LScriptImageSort`.
An empty list means the character was never fetched, or nothing was found and nothing stored.

## `LShengfuArchive`

The workspace-database adapter of the phonetic-series store, one row per language and character.
`LShengfuSave` replaces the text and source already kept under the key.
`LShengfuRead` returns null while nothing was stored for the character.
`LShengfuScan` reads the characters in the order given and drops the ones with nothing stored.

## `LStemArchive`

The workspace-database adapter for the phonetic series the engine browses by.
A series row is shared, so it is made as a character first names it and dropped once none does.
Membership runs through the links, and entries are reached by the characters those links carry.

## `LStemArchiveTally`

The count of entries of the series language whose headword carries one of its characters.
It is spliced into every read, so a series is never listed without its count.

## `LStemApply`

Relinks the character to the series named and clears whatever series then holds nothing.
A character with no stored Shengfu row is left alone, since there is nothing to link from.

## `LStemRebuild`

Drops the series of the language and links every stored Shengfu row again.
It is the path an older workspace takes, where series were fetched before they were linked.

## `LStemRead`

Reads the series of the language in the order they were first made, each with its count.
The id overload returns one series or null when none carries it.
`LStemFind` returns the series of a language and key, or null while nothing links to it.

## `LStemCharacterRead`

Returns the characters linked to the series, in the order their Shengfu rows were stored.

## `LStemEntryScan`

Returns the entries of the language whose headword carries a character of every series named.
Counting the distinct series matched keeps the answer an intersection, as a cell of the rime table is.

## `LStemCharacterApply`

Clears the links of one character's Shengfu row and writes one link per series key.
Helpers read the Shengfu identity and create a series row when it was not stored.
Another helper creates a link and leaves one already held as it is.

## `LStemOrphanClear`

Drops every series of the language nothing links to any more.

## Stamp

Commit: `c2f335c331fe`
- `LDiweiArchive.cs` `264d319e5ce5bf75`
- `LFanqieArchive.cs` `3c92e1d4ec68c1d6`
- `LFrequencyArchive.cs` `6a2ae58bf5ab16a1`
- `LPronunciationArchive.cs` `c8c4cca122d11a5a`
- `LReflexArchive.cs` `2a0a0f41e781f344`
- `LScriptArchive.cs` `db0edbbcbeffce1d`
- `LShengfuArchive.cs` `02f2401ab220ff22`
- `LStemArchive.cs` `e99e8535eb5c71de`
- `LTranscriptionArchive.cs` `a9085d9ec3611895`
