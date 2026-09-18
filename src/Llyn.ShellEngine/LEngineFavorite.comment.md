# LEngineFavorite.cs

## `public IReadOnlyList<LFavorite> LEngineFavoriteFind(string query)`

Finds the marked entries whose headword carries the query.
An empty query returns every marked entry.
Each result carries the moment it was marked, so the caller can order by it.

## `public bool LEngineFavoriteCheck(long entryId)`

Reports whether the entry is marked.

## `public void LEngineFavoriteSave(long entryId)`

Marks the entry a favorite.
Marking changes no lexical data and keeps the entry's identity.

## `public void LEngineFavoriteDelete(long entryId)`

Unmarks the entry.
The entry stands, still reachable through the entry catalog.

## `public IReadOnlyList<LFavorite> LEngineFavoriteFind(string query, LCatalogOrder order)`

The marked entries answering `query`, in `order`.
The mark carries its own stamp, so ordering by the mark is not ordering by the entry.

## `public IReadOnlyList<LFavorite> LEngineFavoriteFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The same list with the marked entries in a hidden language left out.

## `public IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista)`

The rows the favorites panel's vista lists, with the query, order and filter read off the vista.
They come back as vista rows, twins numbered and epithets read in one scan, so the panel only copies them.
The row of the entry the vista stands on comes back marked chosen, so the panel keeps no choice.
