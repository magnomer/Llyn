# LEntryLoader.cs

## `public sealed class LEntryLoader`

Reads a stored entry back into the `LEntryDraft` the input form saved.
It owns no SQL of its own.
It composes what the archives already read into one value shape.
That shape is what `LEngineEntrySave` consumes, so a save and a load are inverses.
The parts are the entry row, its meanings and collocations, its note and pronunciation.
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

## `public LEntryDraft? LEntryLoad(string id)`

Returns the draft for the entry identified by `id`, or `null` when no entry has that id.

A card's rows, Situations, Tags and Translations are ordered sets.
Every row the card references or carries fills the field, in the order the associations record.
So a card referencing three tags loads back with three.
Meanings arrive as the tree the store keeps, each card holding the cards nested under it.
A Collocation never nests, so its children are read nowhere.

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

### `private static LPronunciationDraft? LEntrySoundFormat(LPronunciation? pronunciation, LPronunciationAudio? audio)`

The stored pronunciation and its recording as the one value the draft carries.
An entry with no pronunciation row carries none, rather than an empty one.
The row's id travels with it, so a save updates that row instead of writing another.
The audio's added time stays behind, because it is workspace bookkeeping.

### `private IReadOnlyList<LSpeechDraft> LEntrySpeechFormat(string language, IReadOnlyList<LSpeech> speeches)`

Every part of speech the entry carries, in stored order.
The draft keeps the stored value id and the custom name apart, and carries the name to show beside them.
A custom row shows the text it carries.
A row naming a preset shows the language's display name.
The bare id is shown when the pack that declared it is no longer installed.
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

### `private LCardDraft LEntryCardRead(`

The one read path for the independents a card references.
A Meaning card and a Collocation card hold Examples, Situations, Tags and Translations on identical terms.
So which owner side is being read is the only thing that differs.
The collocation flag says which side ownerId names.
The card's own columns are handed in already read off its row.
The position is handed in the same way, raised by one before it arrives.
Both archives keep their positions contiguous from zero, so the one place that adds the one is here.

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

### `private IReadOnlyList<LRelationDraft> LEntryRelationRead(string meaningId)`

The lexical relations one Meaning holds, in stored order.
Each carries the stored id of the row it points at, an Entry or another Meaning.
The export turns that id into the key the file declares the row under.
Only a Meaning is asked, because the store hangs a relation off a sense.

### `private IReadOnlyList<LSynonymDraft> LEntrySynonymRead(string collocationId)`

The synonym links one Collocation holds, in stored order, by the same rule.
These are the stored links and never the card's free-text Synonym field.
