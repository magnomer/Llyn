# TSpeech.cs

## `public sealed class TSpeech`

Covers the controlled vocabularies and the inflections that use them.
The engine writes parts of speech and morphology into a workspace from the packs on disk.
It covers a pack row rewritten in place, as a second import rewrites it.
It covers an Entry's inflected forms appended to, moved and deleted through the entry update.

## Inline notes

### `Assert.Equal("Noun", engine.LEngineSpeechRead("English", "noun"));`

The tables are no longer empty on a fresh workspace.
What a language declares on disk is what the workspace holds.
So an entry can carry a part of speech from the first save.

### `Assert.Equal("Verb, transitive", engine.LEngineSpeechRead("English", "verb_transitive"));`

The presets go finer than the bare word classes, and a subtype is a value of its own.

### `Assert.Equal(0, engine.LEngineMorphologyRead("English", "noun", "number", "singular")?.LMorphologyPosition);`

Order is the order the pack lists the values in, and it is stored, not inferred.

### `Assert.Equal("Classifier", engine.LEngineSpeechRead("Vietnamese", "classifier"));`

A language declaring no morphology is an ordinary language, not a broken pack.

### `using LEngine reopened = new(workspace.TWorkspaceFolder);`

A second engine over the same workspace writes the packs again.
So the pack's wording is what the workspace ends up holding.

### `LSpeech speech = Assert.Single(`

Stored as the id, never as the word: renaming the preset renames it on this entry too.

### `Assert.Equal("Verb, transitive", engine.LEngineEntryLoad(entry.LEntryId)?.LEntryDraftSpeech);`

And the field gets its own text back, resolved through the language's vocabulary.

### `LSpeech speech = Assert.Single(`

The pack has never heard of it.
So it is kept as the user wrote it, trimmed, and resolving against nothing.

### `engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Verb, ergative"));`

A part of speech no pack declares replaces a declared one on the same terms.

### `engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate(string.Empty));`

The field cleared is the entry left with no part of speech, not the last one still standing.

### `engine.TEngineEntryUpdate(entry.LEntryId, TSpeechDraftCreate("Noun"));`

An update that leaves the field as it stands records nothing about it.
