# LCollocationArchive.cs

## `public sealed class LCollocationArchive`

Persists the collocations an entry owns.
A collocation is a stable-id row assigned an opaque id here on creation.
It is ordered within its entry.
Updating rewrites its title, expression and meaning, so ids survive reordering.
Deleting the entry removes its collocations.

Order within the entry is a unique index, so a position is never written one row at a time.
A new collocation is appended to the end.
`LCollocationMove` renumbers the whole set through `LDatabaseOrder`.

## `public LCollocationArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LCollocation LCollocationCreate(LCollocation collocation)`

Inserts `collocation` with a fresh opaque id at the end of its entry's order.
Returns the stored collocation with that id and its assigned position filled in.

## `public IReadOnlyList<LCollocation> LCollocationRead(long entryId)`

Reads the entry's collocations in card order.

## `public void LCollocationUpdate(LCollocation collocation)`

Updates the title, expression, and meaning of the collocation identified by `collocation`'s id.
The id, owning entry, and position are untouched.
Card order is changed by `LCollocationMove`, which has to renumber the whole set.
Throws when no collocation carries that id.

## `public void LCollocationMove(long id, int position)`

Moves the collocation identified by `id` to `position` in its entry's card order.
It renumbers the whole set so positions stay `0 … n-1`.
A position outside the set is clamped into it, and nothing moves when no collocation carries that id.

## `public void LCollocationDelete(long id)`

Deletes the collocation identified by `id`.
Its example, tag, and situation association rows go with it through the foreign-key cascade.
The independent Examples, Tags, and Situations they pointed at are left standing.
The collocations left under the same entry are renumbered so their positions stay contiguous.

## `public long? LCollocationHolderRead(long id)`

Which entry holds the collocation, or nothing when no row carries the id.
The engine uses it to stamp the entry when a collocation changes on its own.

## Inline notes

### `private static string? LCollocationHolderRead(SqliteConnection connection, long id)`

The entry a collocation belongs to, or null when no collocation carries the id.

### `private static IReadOnlyList<string> LCollocationSiblingRead(SqliteConnection connection, long? entryId)`

The collocations of one entry, in card order.
