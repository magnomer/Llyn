# LCollocationVault.cs

## `public interface LCollocationVault`

The persistence port for the Collocation rows the engine reads and writes.
It lists exactly what the engine asks of collocation storage, and nothing about how rows are kept.
`LCollocationArchive` in Infrastructure is its adapter over the workspace database.

## `LCollocation LCollocationCreate(LCollocation collocation);`

Inserts `collocation` with a fresh opaque id at the end of its entry's order.
Returns the stored collocation with that id and its assigned position filled in.

## `IReadOnlyList<LCollocation> LCollocationRead(long entryId);`

Reads the entry's collocations in card order.

## `void LCollocationUpdate(LCollocation collocation);`

Updates the title, expression, and meaning of the collocation identified by `collocation`'s id.
The id, owning entry, and position are untouched.
Card order is changed by `LCollocationMove`, which has to renumber the whole set.
Throws when no collocation carries that id.

## `void LCollocationMove(long id, int position);`

Moves the collocation identified by `id` to `position` in its entry's card order.
It renumbers the whole set so positions stay `0 … n-1`.
A position outside the set is clamped into it, and nothing moves when no collocation carries that id.

## `void LCollocationDelete(long id);`

Deletes the collocation identified by `id`.
Its example, tag, and situation association rows go with it through the foreign-key cascade.
The independent Examples, Tags, and Situations they pointed at are left standing.
The collocations left under the same entry are renumbered so their positions stay contiguous.

## `long? LCollocationHolderRead(long id);`

Which entry holds the collocation, or nothing when no row carries the id.
The engine uses it to stamp the entry when a collocation changes on its own.

## `void LCollocationOrderSet(long entryId, IReadOnlyList<long> order);`

Rewrites the positions of the entry's collocations so they follow `order`.
A collocation left out of `order` keeps a position past the last.
