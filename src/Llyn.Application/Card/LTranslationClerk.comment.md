# LTranslationClerk.cs

## `public sealed class LTranslationClerk`

The clerk over Translations.
A Translation is a link from one card to another Entry, held as that Entry's id.
It crosses languages, so `break` in English links to `부수다` in Korean.
The link is stored one way, from the card that wrote it.
A target finds what points at it by looking back rather than by holding a list.

A card writes its whole link line at once, the way it writes its Tag line.
So no caller works out which links were added and which were taken away.
Which side an id names arrives as a `collocation` flag, which the engine derives from its `LOwner`.
It runs over the ports of one rig and raises no bulletin and starts no fetch, which the engine keeps.

## `public LTranslationClerk(LRig rig)`

Reads the root, entry, revision, workspace and translation ports out of `rig`.

## `public IReadOnlyList<LTranslation> LTranslationClerkRead(long ownerId, bool collocation)`

Reads the links the Meaning or Collocation identified by `ownerId` carries, in the order that card holds them.

## `public void LTranslationClerkSave(long ownerId, IReadOnlyList<long> ids, bool collocation)`

Writes that card's whole link line from the Entry ids given.
Blank ids, repeats and ids no Entry answers are dropped, and the surviving order is the order given.
An import points a card at an Entry the same file declares, and that Entry may not exist yet.

## `public IReadOnlyList<LEntry> LTranslationClerkFind(string query, long? entryId)`

Every Entry whose headword contains `query`, across every language the workspace holds.
A link crosses languages, so the search cannot be narrowed to the card's own one.
The Entries whose whole headword reads as `query` come first, then the rest in search order.
An empty query names the whole workspace, which is what a just-opened dropdown shows.
The Entry named by `entryId` is left out, because a card may not translate its own Entry.
A null `entryId` leaves nothing out, which is what an unsaved Entry needs.

## `public LEntry? LTranslationClerkResolve(string word, long? entryId)`

The one Entry whose whole headword reads as `word`, or nothing when none or several do.
The search behind it matches on containment, so a hit is not proof the user named it.
Resolving asks for the headword itself, so `brea` never silently becomes `breakfast`.
Both sides are folded the way `LCatalog.LCatalogTextNormalize` folds.
Several Entries sharing a headword are a question, so this answers nothing and the caller asks.

## `public LEntry LTranslationClerkCreate(string headword, string language)`

Creates the bare Entry a typed word no Entry answers needs, and returns it.
The row carries a headword and a language and nothing else.
A stub is a real Entry and is opened and filled in like any other later.
So its making is recorded as a revision, exactly as any other new Entry is.
The workspace row is moved onto that revision in the same session.

## `public IReadOnlyList<LTranslationTarget> LTranslationTargetRead(IReadOnlyList<long> ids)`

The headword and language behind each link id, in the order asked for.
An id no Entry answers is passed over rather than raised.

## `public IReadOnlyList<LTranslationTarget> LTranslationTargetRead(long ownerId, IReadOnlyList<long> ids, IReadOnlyList<LCourt> links)`

The targets of `ids` as the draft `ownerId` sees them.
An id no stored Entry answers is looked for among `links`, the draft's court links the engine reads.
Such a target shows the word and language the court recorded, until the target commits and the id settles.

## `public IReadOnlyList<LUsage> LTranslationIncomingRead(long entryId, bool epithet)`

Every card of either kind that links to the Entry identified by `entryId`.
Links are stored one way, so an Entry's incoming list is read rather than held.
The rows are twinned and given their epithet when `epithet` asks for it.
