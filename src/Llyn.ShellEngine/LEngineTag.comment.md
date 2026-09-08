# LEngineTag.cs

## `public sealed partial class LEngine`

The Tag half of the engine.
It covers the Tags a card carries, read and written as a line.
It also covers the two operations that reach across every card at once.
Those are renaming a Tag and deleting it.

A Tag is its own text.
There is nothing to create and nothing to attach.
A card carries a Tag by writing it, and stops carrying it by not writing it.
That is why the card seam is a whole-line write rather than an attach and a detach.
The shell holds what the card should say, not a diff against what it said before.
No row is left over to leak when a Tag drops off the last card that mentioned it.

Which side an id names arrives as an `LOwner` rather than in the method's name.
A name carries three components after its prefix, and a method per side would need four.
A Tag hangs from a Meaning or a Collocation and from nothing else.
So any other side is refused rather than guessed.

## `public IReadOnlyList<LTag> LEngineTagRead()`

Reads every Tag the workspace holds, once each, in alphabetical order — what a Tag-based lookup lists.

## `public IReadOnlyList<LTag> LEngineTagRead(string ownerId, LOwner owner)`

Reads the Tags the Meaning or Collocation identified by `ownerId` carries, in the order that card holds them.

## `public void LEngineTagSave(string ownerId, IReadOnlyList<LTag> written, LOwner owner)`

Writes that card's whole Tag line.
Blank texts and repeats are dropped, and the surviving order is the order given.

## `public void LEngineTagChange(string text, string renamed)`

Renames a Tag on every card that carries it.
Since the text is the Tag, this is the only way a Tag can be renamed at all.
A card that already carries `renamed` ends with one Tag rather than the same words twice.

## `public void LEngineTagDelete(string text)`

Takes a Tag off every card that carries it.
Nothing but the Tag goes: the cards stay exactly as they were, one label shorter.

## `public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order)`

The tags answering `query`, in `order`.
The tag catalog is held whole rather than searched in the store.
The match is made over what was read.
