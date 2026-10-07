# TSpeech.cs
Hash: `cd1f43db2e32dd9c`

## `public sealed class TSpeech`

Covers the controlled vocabularies and the inflections that use them.
The engine writes parts of speech and morphology into a workspace from the packs on disk.
It covers a pack row rewritten in place, as a second import rewrites it.
It covers an Entry's inflected forms appended to, moved and deleted through the inflection save.

## `public void SpeechRead_ValuesTypedOutOfOrder_ListsPackFirstThenAlphabetical()`

Pins the declared order of a language's parts of speech.
The pack's values come first in pack order, and typed values follow alphabetically without case.
The typed values are added out of order, so arrival order cannot pass for the rule.

## `public void SpeechRead_PackRowStoredOutOfPackOrder_ListsInDeclaredOrder()`

A pack row whose stored position moves to the end still reads in its pack place.
So the stored position never decides what a view shows.

## `public void EntryDraftNames_UnnamedSpeech_LeavesItOut()`

An unnamed part of speech stays out of the names a reader reads, though the entry still counts as marked.

## Inline notes

### `Assert.Equal("Noun", engine.TEngineSpeechRead("English", noun.LSpeechValueId)?.LSpeechValueName);`

The tables are no longer empty on a fresh workspace.
What a language declares on disk is what the workspace holds.
So an entry can carry a part of speech from the first save.

### `Assert.Null(engine.TSpeechValueFind("English", "Verb, transitive"));`

The presets are bare roles, and a retired subtype is no value at all.
Valency belongs to the dependence field, not to the part of speech.

### `"SELECT value.name FROM morphology_value value " +`

Order is the order the pack lists the values in, and it is stored, not inferred.

### `Assert.NotNull(engine.TSpeechValueFind("Vietnamese", "Classifier"));`

A language declaring no morphology is an ordinary language, not a broken pack.

### `using LEngine reopened = workspace.TWorkspaceEngineStart();`

A second engine over the same workspace writes the packs again.
So the pack's wording is what the workspace ends up holding.

### `LSpeech speech = Assert.Single(`

Stored as the id, never as the word.
So renaming the preset renames it on this entry too.

### `Assert.Single(TInterface.TSpeechNameRead(engine.TEngineEntryLoad(entry.LEntryId)!)));`

The entry reads back the preset's name, resolved through the language's vocabulary.

### `LSpeech speech = Assert.Single(`

The pack has never heard of it.
So it is kept as the user wrote it, trimmed, and resolving against nothing.

### `engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Verb, ergative"));`

A part of speech no pack declares replaces a declared one on the same terms.

### `engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate(string.Empty));`

The field cleared is the entry left with no part of speech, not the last one still standing.

### `engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Noun"));`

Setting the field records a speech change.
The same update made again leaves the field as it stands and records nothing about it.
