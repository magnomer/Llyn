# LTagClerk.cs

## `public sealed class LTagClerk`

The clerk over the Tags a card carries and the two operations that reach across every card at once.
Those are renaming a Tag and deleting it.
It runs over the tag vault of one rig and holds no gate, no observer and no bulletin.
The engine calls it under its own gate and raises the tag bulletin afterwards.

A Tag is a shared row a card links by id.
A card still hands its Tags over as a whole line rather than as an attach and a detach.
The shell holds what the card should say, not a diff against what it said before.
A Tag carrying an id links that row.
A Tag carrying only text is resolved to the row reading the same, created if absent.
A row no card links any longer stays in the catalog until the taxonomy panel deletes it.

Which side an id names arrives as a `collocation` flag rather than in the method's name.
The engine turns its `LOwner` into that flag, since a Tag hangs from a Meaning or a Collocation only.

## `public LTagClerk(LRig rig)`

Reads the tag vault out of `rig`.

## `public IReadOnlyList<LTag> LTagClerkRead()`

Reads every Tag the workspace holds, once each, in alphabetical order.
That is what a Tag-based lookup lists.

## `public IReadOnlyList<LTag> LTagClerkFind(string query, LCatalogOrder order)`

The tags answering `query`, in `order`.
The tag catalog is held whole rather than searched in the store.
The match is made over what was read.

## `public LTagOffer LTagClerkFind(string text, LDraft? draft, long card)`

The stored Tags a card's tag field offers for the text it keeps, ready to show.
The text is trimmed, and a blank text offers nothing, since an empty query would list the whole catalog.
The Tags come in usage order, and a Tag whose trimmed text is blank is skipped.
A Tag is held when a chip reads its trimmed text, the rule `LDraftHeldCheck` owns, and is left out.
At most `LCatalog.LCatalogOfferLimit` rows are kept.
No draft or no such card holds nothing, so every match is offered.
Each row is split around the word by `LCatalog.LCatalogMarkFind`.
The offer shows only when some Tag is left.

## `public void LTagClerkSave(long ownerId, IReadOnlyList<LTagDraft> drafts, bool collocation, Dictionary<long, long> identity)`

The same line written from the draft rows a card commit carries.
The store resolves each Tag to its row, by id when it has one and by wording otherwise.
The wording is the Tag's identity in the store, so a written Tag can only ever name one row.
A negative Tag id is recorded in `identity` against the row the store answered with.

## `public LTag LTagClerkCreate(string text)`

Makes a Tag reading `text` with no card carrying it yet, for the taxonomy panel's New.
A Tag already reading the same is returned rather than doubled.
