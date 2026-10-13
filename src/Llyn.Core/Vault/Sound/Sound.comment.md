# Sound

This folder holds the persistence ports for the phonetic data of an entry or a character.
Each interface lists only what the engine asks of its storage, and nothing about how rows are kept.
Every port has an adapter named `L<Name>Archive` in Infrastructure over the workspace database.
The ports stand apart from the Workspace ports because they serve the sound side of the lexicon.

## `LDiweiVault`

Persistence for Diwei categories, the tone, initial and rime classes derived from fanqie rows.

### `LDiweiApply`

Rebuilds the links of one character after its fanqie rows were stored.
Categories left without a link in the language are dropped afterwards.

### `LDiweiRebuild`

Drops every category of the language and derives them again from every stored character.
It runs when the engine binds a workspace.
The tone classes depend on the hypothesis file as it now reads.

### `LDiweiRead`

The category list of one kind comes with entry counts, in the order the categories were first made.
The read by id answers null when none is stored under that id.

### `LDiweiFind`

The category with this key, with its count, or null when no placement carries it.

### `LDiweiEntryScan`

Gives the entries owning a reflex row anchored to one fanqie row linked to every given category at once.
So an initial and a rime together name one cell of the rime table, not two separate sets.
Empty ids give nothing.

### `LDiweiFanqieRead`

Gives the fanqie rows linked to one category that some reflex row is anchored to, in storage order.
A placement nobody anchored is left out.
So the page lists no character without an anchored reading.

## `LFanqieVault`

Persistence for Fanqie rows, the spelling lines fetched per character.

### `LFanqieSave`

Makes the given rows the whole set of the character, in one transaction.
A row already stored under the same book, source and position is rewritten in place under its id.
A row not stored yet is inserted, and a stored row the fetch no longer gives is deleted.
Books arrive in pack order and positions in table order, and a first save's ids keep that order.

### `LFanqieRead`

Gives every stored row of the character in insertion order, with the parts the line was built from.
The reading and tone class come back as the placement stored them, so no hypothesis runs on read.
An empty list means the character was never fetched, or nothing was found and nothing stored.

## `LFrequencyVault`

Persistence for Frequency rows, the per-source usage figures of an entry.

### `LFrequencyRead`

Reads the entry's rows by source name, only so the result is stable.
A refetch rewrites the rows, so write order carries no meaning.
The engine reorders them by the pack's declared sources.
The word interval is not stored, so the rows come back without it for the engine to derive.

### `LFrequencySet`

Replaces the entry's rows with the given set in one transaction.
Every row is stamped with the same fetch time.

### `LFrequencyClear`

Drops the entry's rows, so a renamed entry is fetched afresh.

### `LFrequencyBandSet`

Stores the band of one row after the engine resolved it.
A migrated row needs this once.

## `LPronunciationVault`

Persistence for Pronunciation rows, their syllables and their single recording.

### `LPronunciationCreate`

Inserts the pronunciation with a fresh opaque id, placed after the entry's other pronunciations.
The position it carries is ignored, because `LPronunciationOrderSet` settles the entry's order.
A blank variety, IPA or respelling is stored as NULL.
Syllables are written as ordered child rows.
Their pronunciation id and position are assigned from the new id and list order.
It returns the stored pronunciation with id, position and children filled in.
The whole write is one transaction.

### `LPronunciationRead`

Reads the entry's pronunciations in position order, each with its ordered syllables.
The list is empty when the entry has none.

### `LPronunciationUpdate`

Replaces the variety, the IPA, the respelling and the syllables of the pronunciation with the given id.
Existing syllable rows are cleared and the supplied list written in order.
So the id, entry link, position and identity stay fixed.
The whole write is one transaction, and it throws when no pronunciation carries that id.

### `LPronunciationOrderSet`

Renumbers the entry's pronunciations so that the given order is their position order.
The first id becomes the primary pronunciation.

### `LPronunciationDelete`

Deletes the pronunciation and closes the gap it leaves in its entry's order.
Its syllables and its recording are removed by the foreign-key cascade.

### `LPronunciationAudioSave`

Records a path relative to the workspace folder as the recording the pronunciation owns.
It also records the source label it was downloaded from and the moment it was stored.
A pronunciation carries at most one recording.
So a second save for the same pronunciation replaces the first rather than adding a row.

### `LPronunciationAudioRead`

Reads the recording the pronunciation owns, or null when it has none.
The file path comes back exactly as stored, relative to the workspace folder.
So the caller resolves it against the folder in use now.

### `LPronunciationAudioScan`

Gives every recording file the database names, as stored, in one statement.
The sweep that drops orphaned recordings reads the kept set through it.

## `LReflexVault`

Persistence for Reflex rows, the historical readings of an entry anchored to fanqie rows.

### `LReflexRead`

Reads the entry's reflexes in position order, empty when it has none.
The eight anatomy columns come back as one `LAnatomy` on each row.
The anchors come back as their sorted ids.

### `LReflexAnchorScan`

Gives the reflex rows anchored to each fanqie row of one Diwei category, keyed by the fanqie id.
A fanqie row nothing is anchored to has no key, so a tally over it counts nothing.
Each row carries its own entry id and its anchors, as a read would give it.

### `LReflexSet`

Makes the given reflexes the whole list of the entry, in the order given.
A row with a positive id is rewritten in place, so the id survives the save.
A row with no id is inserted and given one.
A stored row the list no longer names is deleted.
It returns the stored rows with ids and positions filled in.
The anchors of each row are rewritten after the row, under the id the row now has.
The whole write is one transaction.
It throws when a positive id names no row of this entry.
No column is unique, so no row needs shelving before the write.

## `LScriptVault`

Persistence for Script rows, the pictures of a character in historical styles.

### `LScriptSave`

Replaces whatever the character held with the given pictures, in one transaction.
Styles arrive in pack order and positions in source order, and the row ids keep that order.

### `LScriptRead`

Gives every stored picture of the character in insertion order, with its bytes.
An empty list means the character was never fetched, or nothing was found and nothing stored.

## `LShengfuVault`

Persistence for the phonetic series of a character.

### `LShengfuSave`

Makes the row the stored series of its character, rewriting whatever was kept before.

### `LShengfuRead`

Gives the stored series of the character, or null while it was never fetched.

### `LShengfuScan`

Gives the stored series of the given characters, skipping every character that has none.

## `LSpeechVault`

Persistence for the parts of speech a language declares.

### `LSpeechValueCreate`

Adds or replaces one vocabulary row keyed by language and pack code, and returns it with its row id.
A pack that renames a value keeps the row id, so every entry that links it follows the rename.
A value with code 0 is user-added and is given a negative code no pack can collide with.

### `LSpeechValueRead`

The read by row id answers null when no row has it.
The read by language gives every part of speech the language declares, in stored order.
Stored order carries no meaning.
`LVocabularyClerk.LSpeechRead` orders them by the pack's declared rule before any view sees them.
Those are the presets the part-of-speech field offers.
A language with no pack on disk declares none, which is an empty list rather than a failure.

### `LSpeechValueFind`

Resolves a part-of-speech name back to its row.
It answers null when the language declares none by that name.
Matching is a trimmed, case-insensitive comparison of the display name.
The name arrives as the user typed it.
So picking "Verb, transitive" from the dropdown and typing "verb, transitive" mean the same thing.
Only text that names no value at all is stored as typed.

### `LSpeechLoad`

Reads the parts of speech and their features the language pack declares.
A missing pack yields an empty pack.

### `LSpeechRetirementApply`

Moves every link off a retired part and deletes its row.
Part-of-speech links, inflections and features follow the target.
An entry left holding the target twice keeps one.
Linking entries without a unit take the row's unit.
A part already gone leaves nothing to do, so the call is safe on every start.

## `LStemVault`

Persistence for the phonetic series the engine browses by.
It is handed the series keys already cut, so it holds no language fact of its own.

### `LStemApply`

Links the character to every series named, dropping the links it held before.
Series rows are made as they are first named and cleared once nothing links to them.

### `LStemRebuild`

Builds the series of the language again from every Shengfu row it holds.
The separator is the one the language rule prints joined series with.

### `LStemRead`

The read by language gives the stored series, each with the count of entries it reaches.
The read by id answers null when no series carries that identity.

### `LStemFind`

Gives the series row of that language and key, or null while the series was never stored.

### `LStemCharacterRead`

Gives the characters linked to the series, ordered as their Shengfu rows were stored.

### `LStemEntryScan`

Gives the entries of the language whose headword carries a character of every series named.

## `LTranscriptionVault`

Persistence for Transcription rows, the scheme-keyed spellings of an entry.

### `LTranscriptionRead`

Reads the entry's transcriptions in position order, empty when it has none.

### `LTranscriptionSet`

Makes the given transcriptions the whole list of the entry, in the order given.
A row with a positive id is rewritten in place, so the id survives the save.
A row with no id is inserted and given one.
A stored row the list no longer names is deleted.
It returns the stored rows with ids and positions filled in.
The whole write is one transaction.
It throws when a positive id names no row of this entry, and when two rows share a scheme.

## Stamp

Commit: `c2f335c331fe`
- `LDiweiVault.cs` `017f3ac58a9ed64e`
- `LFanqieVault.cs` `176f3b4d17dea2a9`
- `LFrequencyVault.cs` `aa27b0733a2c4164`
- `LPronunciationVault.cs` `842da86c3b10b1ce`
- `LReflexVault.cs` `19a0dda293df5730`
- `LScriptVault.cs` `8da6632d5f0af93e`
- `LShengfuVault.cs` `31d0f74800cc8916`
- `LSpeechVault.cs` `c61536abe2ffd869`
- `LStemVault.cs` `49ce6d685358e1f7`
- `LTranscriptionVault.cs` `fc91c3ef3bcb3e4e`
