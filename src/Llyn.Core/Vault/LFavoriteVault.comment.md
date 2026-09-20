# LFavoriteVault.cs

## `public interface LFavoriteVault`

The persistence port for the Favorite rows the engine reads and writes.
It lists exactly what the engine asks of favorite storage, and nothing about how rows are kept.
`LFavoriteArchive` in Infrastructure is its adapter over the workspace database.

## `void LFavoriteSave(long entryId);`

Marks the entry a favorite, stamping the moment.
Marking an entry that is already marked keeps the earlier stamp.

## `void LFavoriteDelete(long entryId);`

Unmarks the entry.
The entry itself stands untouched.

## `bool LFavoriteCheck(long entryId);`

Reports whether the entry is marked.

## `IReadOnlyList<LCatalogFavorite> LFavoriteFind(string query);`

Finds the marked entries whose headword carries the query, folded for case and accent.
An empty query returns every marked entry.
Rows come back by headword, and the caller reorders them.
