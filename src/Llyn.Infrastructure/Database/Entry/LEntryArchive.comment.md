# LEntryArchive.cs

## `public sealed partial class LEntryArchive : LEntryVault`

It is the adapter of `LEntryVault`, the port the engine holds.

Persists entries and their two owned child structures — written forms and parts of speech — in the workspace database.
An entry's id and timestamps are assigned here on creation.
Its forms and POS are written as ordered child rows.
So reordering rewrites `position` only and never touches the entry id.
Deleting an entry removes its forms and POS through the foreign-key cascade.

Every method runs inside a `LDatabaseSession`.
So a caller that opens one of its own around several stores gets one transaction.

## `public LEntryArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LEntry LEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)`

Inserts `entry` with a fresh opaque id and creation and modification timestamps.
It writes `forms` and `speeches` as ordered child rows.
Their entry id and position are assigned from list order.
Returns the stored entry with its id and timestamps filled in.
The whole write is one transaction.

## `public LEntry? LEntryRead(long id)`

Reads the entry row for `id`, or `null` when no entry has that id.

## `public LEntryDraft? LEntryLoad(long id)`

Assembles the whole editable content of the entry through `LEntryLoader`, which stays this store's helper.

## `public IReadOnlyList<LForm> LEntryFormRead(long id)`

Reads the entry's written forms, ordered by position.

## `public IReadOnlyList<LSpeech> LEntrySpeechRead(long id)`

Reads the entry's part-of-speech assignments, ordered by position.

## `public void LEntryUpdate(LEntry entry)`

Updates the headword and metadata of the entry identified by `entry`'s id and stamps a fresh modification timestamp.
The id, forms, and POS are untouched.
Throws when no entry carries that id, rather than reporting success for a write that reached nothing.

## `public void LEntryFormSet(long id, IReadOnlyList<LForm> forms)`

Replaces the entry's forms with `forms` in list order.
Existing form rows are cleared and the new set written.
So reordering rewrites positions while the entry id stays fixed.

## `public void LEntrySpeechSet(long id, IReadOnlyList<LSpeech> speeches)`

Replaces the entry's part-of-speech assignments with `speeches` in list order, clearing the existing rows first.
Reordering rewrites positions.
The entry id stays fixed.

## `public void LEntryUpdatedSet(long id)`

Moves `updated_utc` alone to now.
The engine calls it when a part of the entry changes through its own seam.
An id no entry carries is not an error here, since nothing was meant to be read back.

## `public string LEntryEpithetRead(long entryId)`

The stored epithet of the entry `entryId` names, or empty when it has none or no entry carries the id.
It is one scalar read, so every list row can ask without loading the entry or its reflex rows.

## `public void LEntryEpithetSave(long entryId, string epithet)`

Writes the derived epithet alone onto the entry `entryId` names.
A value equal to the one stored is not written, so nothing else notices.
`updated_utc` is left where it stands, because a derived string is not a user edit.
An id no entry carries is not an error here, since the derivation may outlive the entry.

## `public void LEntryGraspSet(long entryId, int grasp)`

Writes the user's grasp alone onto the entry `entryId` names.
`updated_utc` is left where it stands, because the word did not change, only the user's view of it.
The value must pass [LGrasp](../../Llyn.Core/Lexicon/LGrasp.comment.md) `LGraspCheck`, so nothing outside zero to ten reaches the row.
Throws when no entry carries that id, as `LEntryUpdate` does.
`LEntryCreate` and `LEntryUpdate` never mention the column, so an editor save can never clobber a rating.

## `public void LEntryDelete(long id)`

Deletes the entry identified by `id` and everything it owns.
The foreign-key cascade carries away its forms and parts of speech.
It carries away its inflections and their features.
It carries away its meanings, each with its inline definition field.
It carries away its pronunciations with their syllables and recordings, and its transcriptions.
It carries away its collocations and its single note.
It carries away every association row hanging from the entry, its meanings, or its collocations.
The independent Examples, Tags, Situations, References, and Authors those associations pointed at are left standing.
Only the rows linking them to this entry disappear.

## Inline notes

### `private static LEntry LEntryRowRead(SqliteDataReader reader)`

The entry row shape every read here selects, in one place.
A single read and a find would otherwise drift apart column by column.

### `bool declared = speech.LSpeechValueId is > 0;`

A row links a declared value or carries typed text, never both.
The table's CHECK says so.
A caller handing over both would otherwise write a row that resolves one way and displays another.
The id wins, because a name matching a value is that value.
