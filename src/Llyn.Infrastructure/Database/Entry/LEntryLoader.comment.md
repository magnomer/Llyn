# LEntryLoader.cs

## `public sealed class LEntryLoader`

Reads a stored entry back into the `LEntryDraft` the input form saved.
It owns no SQL of its own.
It composes what the archives already read into one value shape.
That shape is what a draft commit consumes, so a save and a load are inverses.
The parts are the entry row, its meanings and collocations, its note, its pronunciations and its transcriptions.
They include the entry's forms, its parts of speech and its inflections with their features.
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

## `public LEntryDraft? LEntryLoad(long id)`

Returns the draft for the entry identified by `id`, or `null` when no entry has that id.

A card's rows, Situations, Tags and Translations are ordered sets.
Every row the card references or carries fills the field, in the order the associations record.
So a card referencing three tags loads back with three.
Meanings arrive as the tree the store keeps, each card holding the cards nested under it.
A Collocation never nests, so its children are read nowhere.

Each card carries the id of the row it was read from.
So a draft loaded, edited and committed names the row each card belongs to.
A card the user adds afterwards has no id and is created.

## Inline notes

### `private IReadOnlyList<LPronunciationDraft> LEntrySoundRead(long id)`

Every stored pronunciation of the entry and its recording, in stored order, as the values the draft carries.
An entry with no pronunciation row carries an empty list.
Each row's id travels with it, so a save updates that row instead of writing another.
The audio's added time stays behind, because it is workspace bookkeeping.

### `private IReadOnlyList<LTranscriptionDraft> LEntrySpellingRead(long id)`

Every stored transcription of the entry, in stored order, each with its scheme and its id.

### `private LEtymologyDraft LEntryEtymologyRead(long id)`

The stored etymology of the entry, both shapes read together into one draft.
An entry that declares no origin gives an empty draft rather than null.

### `private IReadOnlyList<LReflexDraft> LEntryReflexRead(long id)`

Every stored reflex of the entry, in stored order, each with its language, kind, mark and id.

### `private IReadOnlyList<LSpeechDraft> LEntrySpeechFormat(IReadOnlyList<LSpeech> speeches)`

Every part of speech the entry carries, in stored order.
The draft keeps the value link and the custom name apart, and carries the name to show beside them.
A custom row shows the text it carries.
A row linking a value shows that row's name.
An id is still what the entry was filed under, and showing it beats showing nothing.

### `private IReadOnlyList<LCardDraft> LEntryChildRead(`

The Meanings under one parent, each already holding the Meanings under it.
The senses are grouped by parent once and the tree is walked from the roots.
A root is grouped under the empty key, because the store writes no parent for one.
Each group is already in stored position order, so no group is sorted again here.
Position is per sibling group, which is what the store's unique index counts.

### `private static LExampleDraft? LEntryExampleRead(LExample? example)`

The Example a card's row quotes, or `null` when the row quotes none.
A row states a frame and no sentence when the store holds no Example for it.
That row is real data and must load as itself rather than as an empty sentence.
The stored Glosses and Mentions travel with the Example as drafts under their positive ids.
A commit that dropped them would otherwise delete every rendering and link the sentence carried.
The draft is built by [LExampleDraft](../../Llyn.Core/Lexicon/LExampleDraft.comment.md), so every loader reads a stored Example the same way.

### `private LCardDraft LEntryCardRead(`

The one read path for the independents a card references.
A Meaning card and a Collocation card hold Examples, Situations, Tags and Translations on identical terms.
So which owner side is being read is the only thing that differs.
The collocation flag says which side ownerId names.
The card's own columns are handed in already read off its row.
The position is handed in the same way, raised by one before it arrives.
Both archives keep their positions contiguous from zero, so the one place that adds the one is here.

### `ownerId);`

The card says which stored row it is.
So a draft handed back to the engine updates that row rather than reading as a new card.

### `private static IReadOnlyList<string> LEntryTextRead<TRow>(`

A card field is an ordered set, so every referenced row fills it and none is discarded.
The rows arrive ordered by the position each association carries.
That is the order the save wrote them in, so the field reads back as it was typed.
A row whose text is null reads as the empty string the draft uses for "nothing was typed".
