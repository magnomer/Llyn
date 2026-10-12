# LCollocationArchive.cs
Hash: `7d6be511d2061c2d`

## `public sealed class LCollocationArchive`

Persists the collocations an entry owns.
A collocation is a stable-id row assigned an opaque id here on creation.
It is ordered within its entry.
Updating rewrites its title, expression and meaning in place.
Reordering rewrites positions only, so ids survive it.
Deleting the entry removes its collocations.
A collocation also links to zero, one or many Meanings of its entry, in link order.
The first link is its home.

Order within the entry is a unique index, so a position is never written one row at a time.
A new collocation is appended to the end.
`LCollocationOrderSet` renumbers the whole set through `LDatabaseOrder`.

## `public LCollocationArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.
It also builds the link over `collocation_sense` that the Meaning links are written through.

## `public LCollocation LCollocationCreate(LCollocation collocation)`

Inserts `collocation` with a fresh opaque id at the end of its entry's order.
The id comes from the space Collocations share with Meanings, `LSchemaCollocationSequence`.
Returns the stored collocation with that id and its assigned position filled in.

## `public IReadOnlyList<LCollocation> LCollocationRead(long entryId)`

Reads the entry's collocations in card order.

## `public void LCollocationUpdate(LCollocation collocation)`

Updates the title, expression, and meaning of the collocation identified by `collocation`'s id.
The id, owning entry, and position are untouched.
Card order is changed by `LCollocationOrderSet`, which renumbers the whole set.
Throws when no collocation carries that id.

## `public void LCollocationOrderSet(long entryId, IReadOnlyList<long> order)`

Rewrites the positions of the entry's collocations so they follow `order`.
The card update names the whole order because it has already applied every card and knows the ids.

## `public void LCollocationDelete(long id)`

Deletes the collocation identified by `id`.
Every row that links to it, Meaning links included, goes with it through the foreign-key cascade.
The independent rows those links pointed at, such as Examples and Meanings, are left standing.
The collocations left under the same entry are renumbered so their positions stay contiguous.

## `public IReadOnlyList<long> LCollocationSenseRead(long collocationId)`

The ids of the Meanings the collocation links to, in link order.
The first id is the home.
Throws when `collocationId` is not positive.

## `public void LCollocationSenseAttach(long collocationId, long senseId, int position)`

Links the collocation to a Meaning at `position` in its link order.
The set is renumbered around it, and a Meaning already linked moves to `position`.
No check is made here that the Meaning belongs to the same entry.

## `public void LCollocationSenseDetach(long collocationId, long senseId)`

Removes one link and closes the gap it leaves.
The Meaning and the collocation both survive.
Deleting either one removes its links through the foreign-key cascade.

## Inline notes

### `private static long? LCollocationHolderRead(SqliteConnection connection, long id)`

The entry a collocation belongs to, or null when no collocation carries the id.

### `private static IReadOnlyList<long> LCollocationSiblingRead(SqliteConnection connection, long? entryId)`

The collocations of one entry, in card order.
