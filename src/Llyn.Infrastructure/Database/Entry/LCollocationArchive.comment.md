# LCollocationArchive.cs

## `public sealed class LCollocationArchive`

Persists the collocations an entry owns.
A collocation is a stable-id row assigned an opaque id here on creation.
It is ordered within its entry.
Updating rewrites its title, expression and meaning, so ids survive reordering.
Deleting the entry removes its collocations.

Order within the entry is a unique index, so a position is never written one row at a time.
A new collocation is appended to the end.
`LCollocationOrderSet` renumbers the whole set through `LDatabaseOrder`.

## `public LCollocationArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

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
Its example, tag, and situation association rows go with it through the foreign-key cascade.
The independent Examples, Tags, and Situations they pointed at are left standing.
The collocations left under the same entry are renumbered so their positions stay contiguous.

## Inline notes

### `private static string? LCollocationHolderRead(SqliteConnection connection, long id)`

The entry a collocation belongs to, or null when no collocation carries the id.

### `private static IReadOnlyList<string> LCollocationSiblingRead(SqliteConnection connection, long? entryId)`

The collocations of one entry, in card order.
