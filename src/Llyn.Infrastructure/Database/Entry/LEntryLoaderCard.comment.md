# LEntryLoaderCard.cs
Hash: `95e0b048c2120f0c`

## `public sealed class LEntryLoaderCard`

Reads an entry's Meaning and Collocation cards back into the values its draft carries.
Each card brings its sentences, Situations, Registers, Tags, Translations, Images and Videos, in stored order.
`LEntryLoader` builds it inside its own session, so these reads join one snapshot.

## `public LEntryLoaderCard(LDatabase database)`

Binds the loader to the workspace `database` it reads through.

## `public IReadOnlyList<LCardDraft> LEntryMeaningRead(long id)`

The Meaning cards of the entry `id`, as the tree the store keeps.
Each card holds the cards nested under it.

## `public IReadOnlyList<LCardDraft> LEntryCollocationRead(long id)`

The Collocation cards of the entry `id`, in stored order.
A Collocation never nests, so its children are read nowhere.

## Inline notes

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
The draft is built by [LExampleDraft](../../../Llyn.Core/Lexicon/Card/LExampleDraft.comment.md), so every loader reads a stored Example the same way.

### `private LCardDraft LEntryCardRead(`

The one read path for the independents a card references.
A Meaning card and a Collocation card hold sentences, Situations, Registers, Translations, Tags, Images and Videos on identical terms.
So which owner side is being read is the only thing that differs.
The collocation flag says which side ownerId names.
The card's own columns are handed in already read off its row.
The position is handed in the same way, raised by one before it arrives.
Both archives keep their positions contiguous from zero, so the one place that adds the one is here.

### `ownerId);`

The card says which stored row it is.
So a draft handed back to the engine updates that row rather than reading as a new card.
