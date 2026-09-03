# LEntryLoader.cs

## `public sealed class LEntryLoader`

Reads a stored entry back into the `LEntryDraft` the input form saved.
It owns no SQL of its own.
It composes what the archives already read into one value shape.
That shape is what `LEngineEntrySave` consumes, so a save and a load are inverses.
The parts are the entry row, its meanings and collocations, its note and pronunciation.
They also include every Example and Situation each card references and every Tag and Translation it carries.
All of them arrive in stored order.

The whole composition runs inside one `LDatabaseSession`.
Every archive call it makes nests into that session.
So the draft is a single consistent snapshot.
It is not a dozen independently-timed queries that could disagree with each other.

The audio path comes back exactly as stored, relative to the workspace folder.
Resolving it against the folder in use is the engine's job, because the workspace root is what the engine owns.

## `public LEntryLoader(LDatabase database)`

Binds the loader to the workspace `database` it reads through.

## `public LEntryDraft? LEntryLoad(string id)`

Returns the draft for the entry identified by `id`, or `null` when no entry has that id.

A card's Examples, Situations, Tags and Translations are ordered sets.
Every row the card references or carries fills the field, in the order the associations record.
So a card referencing three tags loads back with three.
Meanings arrive flat, as the draft has no nesting.
A sub-meaning, which no save writes, would come back as a card of its own.

Each card carries the id of the row it was read from.
So a draft loaded, edited and handed to `LEngineEntryUpdate` names the row each card belongs to.
A card the user adds afterwards has no id and is created.

`LCardDraftSynonym` comes back empty on every card, because no card writes one.
A synonym is a link to a stored Entry or Meaning.
Neither card offers a control for it any more.
The drafts carry no target to read back.
A Meaning's relations and a Collocation's synonyms are read through `LRelationArchive` and `LSynonymArchive`.
That is what the engine's relation seam exposes.
They are links between rows rather than text belonging to the card.

## Inline notes

### `private string LEntrySpeechFormat(string language, IReadOnlyList<LSpeech> speeches)`

The part of speech as the field shows it.
A custom row shows the text it carries.
A row naming a preset shows the language's display name.
The bare id is shown when the pack that declared it is no longer installed.
An id is still what the entry was filed under, and showing it beats showing nothing.
Only the first assignment is shown, because the field holds one.
A second row, which no save writes, stays stored and is simply not displayed.

### `private LCardDraft LEntryCardRead(`

The one read path for the independents a card references.
A Meaning card and a Collocation card hold Examples, Situations, Tags and Translations on identical terms.
So which owner side is being read is the only thing that differs.
The collocation flag says which side ownerId names.
The card's own columns are handed in already read off its row.

### `string.Empty,`

Nothing stored feeds a card synonym: the save writes none, so the field comes back empty.

### `ownerId);`

The card says which stored row it is.
So a draft handed back to the engine updates that row rather than reading as a new card.

### `private static IReadOnlyList<string> LEntryTextRead<TRow>(`

A card field is an ordered set, so every referenced row fills it and none is discarded.
The rows arrive ordered by the position each association carries.
That is the order the save wrote them in, so the field reads back as it was typed.
A row whose text is null reads as the empty string the draft uses for "nothing was typed".
