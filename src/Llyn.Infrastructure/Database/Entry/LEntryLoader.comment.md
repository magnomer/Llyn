# LEntryLoader.cs
Hash: `f0da859a542d6c13`

## `public sealed class LEntryLoader`

Reads a stored entry back into the `LEntryDraft` the input form saved.
It owns no SQL of its own.
The cards come from `LEntryLoaderCard` and the pronunciations, transcriptions and reflexes from `LEntryLoaderReading`.
It composes what the archives already read into one value shape.
That shape is what a draft commit consumes, so a save and a load are inverses.
The parts are the entry row, its meanings and collocations, its note, its pronunciations and its transcriptions.
They include the entry's forms, its parts of speech and its inflections with their features.
They include its reflexes and its etymology.
They include its lexical unit, read off the entry row.
They also include every sentence, Situation, Register, Tag, Translation, Image and Video each card carries.
All of them arrive in stored order.

The whole composition runs inside one `LDatabaseSession`.
Both section loaders are built inside it, so every archive call they make nests into that session.
So the draft is a single consistent snapshot.
It is not a dozen independently-timed queries that could disagree with each other.

The audio path comes back exactly as stored, relative to the workspace folder.
Resolving it against the folder in use is the engine's job, because the workspace root is what the engine owns.

## `public LEntryLoader(LDatabase database)`

Binds the loader to the workspace `database` it reads through.

## `public LEntryDraft? LEntryLoad(long id)`

Returns the draft for the entry identified by `id`, or `null` when no entry has that id.

A card's sentences, Situations, Registers, Tags, Translations, Images and Videos are ordered sets.
Every row the card references or carries fills the field, in the order the associations record.
So a card referencing three tags loads back with three.
Meanings arrive as the tree the store keeps, each card holding the cards nested under it.
A Collocation never nests, so its children are read nowhere.

Each card carries the id of the row it was read from.
So a draft loaded, edited and committed names the row each card belongs to.
A card the user adds afterwards has no id and is created.

## Inline notes

### `private LEtymologyDraft LEntryEtymologyRead(long id)`

The stored etymology of the entry, both shapes read together into one draft.
An entry that declares no origin gives an empty draft rather than null.

### `private IReadOnlyList<LSpeechDraft> LEntrySpeechFormat(IReadOnlyList<LSpeech> speeches)`

Every part of speech the entry carries, in stored order.
The draft keeps the value link and the custom name apart, and carries the name to show beside them.
A custom row shows the text it carries.
A row linking a value shows that value's name, or nothing when the value is gone.
