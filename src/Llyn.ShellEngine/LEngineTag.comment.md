# LEngineTag.cs

## `public sealed partial class LEngine`

The Tag half of the engine.
It covers the Tags a card carries, read and written as a line.
It also covers the two operations that reach across every card at once.
Those are renaming a Tag and deleting it.

A Tag is a shared row a card links by id.
A card still hands its Tags over as a whole line rather than as an attach and a detach.
The shell holds what the card should say, not a diff against what it said before.
A Tag carrying an id links that row.
A Tag carrying only text is resolved to the row reading the same, created if absent.
A row no card links any longer stays in the catalog until the taxonomy panel deletes it.

Which side an id names arrives as an `LOwner` rather than in the method's name.
A name carries three components after its prefix, and a method per side would need four.
A Tag hangs from a Meaning or a Collocation and from nothing else.
So any other side is refused rather than guessed.

## `internal IReadOnlyList<LTag> LEngineTagRead()`

Reads every Tag the workspace holds, once each, in alphabetical order — what a Tag-based lookup lists.

## `internal IReadOnlyList<LTag> LEngineTagRead(long ownerId, LOwner owner)`

Reads the Tags the Meaning or Collocation identified by `ownerId` carries, in the order that card holds them.

## `internal void LEngineTagSave(long ownerId, IReadOnlyList<LTag> written, LOwner owner)`

Writes that card's whole Tag line.
Each Tag is resolved to its row by id, or by text when it carries none.
Blank texts and repeats are dropped, and the surviving order is the order given.

## `public LTag LEngineTagCreate(string text)`

Makes a Tag reading `text` with no card carrying it yet, for the taxonomy panel's New.
A Tag already reading the same is returned rather than doubled.
The catalog is announced so every panel listing Tags shows the new row.

## `internal void LEngineTagChange(long tagId, string renamed)`

Renames the Tag `tagId` names, and every card linking it follows.
When another Tag already reads `renamed`, the two fold into one.
A card that carried both ends with one Tag rather than the same words twice.

## `internal void LEngineTagDelete(long tagId)`

Takes a Tag off every card that carries it and deletes its row.
Nothing else goes: the cards stay exactly as they were, one label shorter.

## `public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order)`

The tags answering `query`, in `order`.
The tag catalog is held whole rather than searched in the store.
The match is made over what was read.

## `public IReadOnlyList<LTag> LEngineTagFind(LVista vista)`

The tags the taxonomy panel's vista lists, with the query and order read off the vista.
The vista's filter hides languages from the entries of the chosen tag, not tags, so it is not applied here.
