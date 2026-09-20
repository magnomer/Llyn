# LEntryVault.cs

## `public interface LEntryVault`

The persistence port for entries and their two owned child structures, written forms and parts of speech.
It lists exactly what the engine asks of entry storage, and nothing about how rows are kept.
`LEntryArchive` in Infrastructure is its adapter over the workspace database.
A test may answer from a dictionary, which is how the engine is proved to need no database.

## `LEntry LEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches);`

Stores `entry` with a fresh id and timestamps and its ordered `forms` and `speeches`.
Returns the stored entry with its id and timestamps filled in.

## `LEntry? LEntryRead(long id);`

Reads the entry with `id`, or `null` when no entry has that id.

## `LEntryDraft? LEntryLoad(long id);`

Assembles the whole editable content of the entry with `id`, or `null` when no entry has that id.

## `IReadOnlyList<LForm> LEntryFormRead(long id);`

Reads the entry's written forms, ordered by position.

## `IReadOnlyList<LSpeech> LEntrySpeechRead(long id);`

Reads the entry's part-of-speech assignments, ordered by position.

## `void LEntryUpdate(LEntry entry);`

Updates the headword and metadata of the entry identified by `entry`'s id.

## `void LEntryFormSet(long id, IReadOnlyList<LForm> forms);`

Replaces the entry's written forms with `forms` in list order.

## `void LEntrySpeechSet(long id, IReadOnlyList<LSpeech> speeches);`

Replaces the entry's part-of-speech assignments with `speeches` in list order.

## `void LEntryUpdatedSet(long id);`

Stamps a fresh modification time on the entry.

## `string LEntryEpithetRead(long entryId);`

Reads the entry's epithet, or an empty string when none is stored.

## `void LEntryEpithetSave(long entryId, string epithet);`

Stores `epithet` as the entry's epithet.

## `void LEntryGraspSet(long entryId, int grasp);`

Stores the entry's grasp level.

## `void LEntryDelete(long id);`

Removes the entry and everything it owns.

## `IReadOnlyList<LEntry> LEntryFind(string query);`

Finds the entries whose headword matches `query`, every entry for an empty query.

## `IReadOnlyList<LEntry> LEntryHeadwordFind(string language, string headword);`

Finds the entries of `language` whose headword equals `headword`.

## `IReadOnlyList<LEntry> LEntryHeadwordScan(string language, string text);`

Finds the entries of `language` whose headword occurs in `text`.

## `IReadOnlyList<LEntry> LEntryScan(IReadOnlyList<long> ids, string query);`

Reads the entries among `ids` whose headword matches `query`.

## `IReadOnlyDictionary<long, string> LEntryEpithetScan(IReadOnlyList<long> ids);`

Reads the epithet of every entry among `ids` that has one.

## `IReadOnlyList<LEntry> LEntryTagFind(long tagId);`

Finds the entries a card of which carries the tag.

## `IReadOnlyList<LEntry> LEntryRegisterFind(long registerId);`

Finds the entries a card of which carries the register.

## `IReadOnlyList<LEntry> LEntrySituationFind(long situationId);`

Finds the entries a card of which carries the situation.

## `IReadOnlyList<LEntry> LEntryExampleFind(long exampleId);`

Finds the entries a sentence of which cites the example.

## `IReadOnlyList<LEntry> LEntryReferenceFind(long referenceId);`

Finds the entries that cite the reference.

## `long LEntryCountRead();`

Counts every entry in the workspace.
