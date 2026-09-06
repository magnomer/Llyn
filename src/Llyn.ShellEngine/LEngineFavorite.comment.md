# LEngineFavorite.cs

## `public IReadOnlyList<LFavorite> LEngineFavoriteFind(string query)`

Finds the marked entries whose headword carries the query.
An empty query returns every marked entry.
Each result carries the moment it was marked, so the caller can order by it.

## `public bool LEngineFavoriteCheck(string entryId)`

Reports whether the entry is marked.

## `public void LEngineFavoriteSave(string entryId)`

Marks the entry a favorite.
Marking changes no lexical data and keeps the entry's identity.

## `public void LEngineFavoriteDelete(string entryId)`

Unmarks the entry.
The entry stands, still reachable through the entry catalog.
