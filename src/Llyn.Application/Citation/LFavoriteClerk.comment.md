# LFavoriteClerk.cs

## `public sealed class LFavoriteClerk`

The clerk over the favorite marks.
A favorite is a marked citation of an entry.
The mark changes no lexical data and keeps the entry's identity.
The mark carries its own stamp, so ordering by the mark is not ordering by the entry.

## `public LFavoriteClerk(LRig rig)`

Reads the favorite port out of `rig`.

## `public IReadOnlyList<LCatalogFavorite> LFavoriteClerkFind(string query)`

Finds the marked entries whose headword carries the query.
An empty query returns every marked entry.

## `public IReadOnlyList<LCatalogFavorite> LFavoriteClerkFind(string query, LCatalogOrder order)`

The marked entries answering `query`, in `order`.

## `public IReadOnlyList<LCatalogFavorite> LFavoriteClerkFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The same list with the marked entries in a hidden language left out.

## `public bool LFavoriteClerkCheck(long entryId)`

Whether the entry is marked.

## `public void LFavoriteClerkSave(long entryId)`

Marks the entry a favorite.

## `public void LFavoriteClerkDelete(long entryId)`

Unmarks the entry.
The entry stands, still reachable through the entry catalog.
