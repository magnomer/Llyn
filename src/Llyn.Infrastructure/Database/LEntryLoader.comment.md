# LEntryLoader.cs

## `public sealed class LEntryLoader`

Reads a stored entry back into the `LEntryDraft` the input form saved. It owns no SQL of its own: it composes what the archives already read — the entry row, its meanings and collocations, its note and pronunciation, and every Example, Situation and Tag each card references, in stored order — into the one value shape `LEngineEntrySave` consumes, so a save and a load are inverses of each other.

The whole composition runs inside one `LDatabaseSession`. Every archive call it makes nests into that session, so the draft is a single consistent snapshot rather than a dozen independently-timed queries that could disagree with each other.

The audio path comes back exactly as stored, relative to the workspace folder. Resolving it against the folder in use is the engine's job, because the workspace root is what the engine owns.

## `public LEntryLoader(LDatabase database)`

Binds the loader to the workspace `database` it reads through.

## `public LEntryDraft? LEntryLoad(string id)`

Returns the draft for the entry identified by `id`, or `null` when no entry has that id.

A card's Examples, Situations and Tags are ordered sets: every row the card references fills the field, in the order the associations record, so a card referencing three tags loads back with three. Meanings arrive flat, as the draft has no nesting: a sub-meaning — which no save writes — would come back as a card of its own.

Each card carries the id of the row it was read from, so a draft loaded, edited and handed to `LEngineEntryUpdate` names the Meaning or Collocation each card belongs to; a card the user adds afterwards has no id and is created.

`LCardDraftSynonym` comes back empty on every card, because no card writes one: a synonym is a link to a stored Entry or Meaning, neither card offers a control for it any more, and the drafts carry no target to read back. A Meaning's relations and a Collocation's synonyms are read through `LRelationArchive` and `LSynonymArchive`, which is what the engine's relation seam exposes; they are links between rows rather than text belonging to the card.

## Inline notes

### `private string LEntrySpeechFormat(string language, IReadOnlyList<LSpeech> speeches)`

The part of speech as the field shows it: the text a custom row carries, the language's display name for a row naming a preset, and the bare id when the pack that declared it is no longer installed — an id is still what the entry was filed under, and showing it beats showing nothing. Only the first assignment is shown, because the field holds one; a second row, which no save writes, stays stored and is simply not displayed.

### `private LCardDraft LEntryCardRead(`

The one read path for the independents a card references. A Meaning card and a Collocation card reference Examples, Situations and Tags on identical terms, so which owner side is being read is the only thing that differs: the collocation flag says which side ownerId names, and the card's own columns are handed in already read off its row.

### `string.Empty,`

Nothing stored feeds a card synonym: the save writes none, so the field comes back empty.

### `ownerId);`

The card says which stored row it is, so a draft handed back to the engine updates that row rather than being read as a new card.

### `private static IReadOnlyList<string> LEntryTextRead<TRow>(`

A card field is an ordered set, so every referenced row fills it and none is discarded. The rows arrive ordered by the position each association carries, which is the order the save wrote them in, so the field reads back as it was typed. A row whose text is null reads as the empty string the draft uses for "nothing was typed".
