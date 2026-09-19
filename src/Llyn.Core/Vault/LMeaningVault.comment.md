# LMeaningVault.cs

## `public interface LMeaningVault`

The persistence port for the Meaning rows the engine reads and writes.
It lists exactly what the engine asks of meaning storage, and nothing about how rows are kept.
`LMeaningArchive` in Infrastructure is its adapter over the workspace database.

## `LMeaning LMeaningCreate(LMeaning meaning);`

Inserts `meaning` with a fresh opaque id at the end of its sibling group.
Returns the stored meaning with that id and its assigned position filled in.
When it names a parent, the parent must be an existing meaning in the same entry.
Otherwise an `InvalidOperationException` is thrown before anything is written.

## `IReadOnlyList<LMeaning> LMeaningRead(long entryId);`

Reads the entry's whole Meaning tree as a flat list ordered by parent then position.
So a caller rebuilds the hierarchy from each meaning's `LMeaning.LMeaningParentId`.
Siblings arrive already in order.

## `LMeaning? LMeaningSingleRead(long id);`

Reads the single meaning identified by `id`, or `null` when no meaning carries that id.
That is the one-row read a caller needs when it holds a Meaning id alone.

## `void LMeaningUpdate(LMeaning meaning);`

Updates the title and definition of the meaning identified by `meaning`'s id.
The id, entry, parent link, and position are untouched.
Where a Meaning sits among its siblings is changed by `LMeaningMove`.
That method has to renumber the whole group.
Throws when no meaning carries that id.

## `void LMeaningParentUpdate(long id, long? parentId);`

Moves a Meaning under another Meaning of the same entry, or out to the entry's roots.
The row goes to the end of the group it joins, and the group it left is renumbered behind it.
The unique sibling index rejects two rows sharing a place, so neither group is left with a gap.
A Meaning cannot be moved inside its own subtree, because a Meaning is not its own ancestor.
Nothing happens when the row already sits under that parent.

## `void LMeaningMove(long id, int position);`

Moves the meaning identified by `id` to `position` among its siblings.
It renumbers the whole group so positions stay `0 … n-1`.
A position outside the group is clamped into it.
Nothing moves when no meaning carries that id.

## `void LMeaningDelete(long id);`

Deletes the meaning identified by `id` together with everything it owns.
That is its inline definition field, held in columns of the row itself.
It is also its subordinate meanings, with the same treatment applied down the tree.
It is also its example, tag, and situation association rows.
Sibling and ancestor meanings are untouched and are renumbered so positions stay contiguous.
The independent Examples, Tags, and Situations it referenced are left standing.
Only the links go.

## `long? LMeaningHolderRead(long id);`

Which entry holds the meaning, or nothing when no row carries the id.
The engine uses it to stamp the entry when a meaning changes on its own.

## `void LMeaningOrderSet(long entryId, long? parentId, IReadOnlyList<long> order);`

Rewrites the positions of one sibling group so they follow `order`.
The group is the entry's top-level meanings when `parentId` is `null`, else the children of that parent.
Every id in `order` is a meaning of that group.
A meaning left out keeps a position past the last.
