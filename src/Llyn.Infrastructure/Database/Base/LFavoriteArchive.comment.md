# LFavoriteArchive.cs
Hash: `710ded0bebc2ea1f`

## `public sealed class LFavoriteArchive : LFavoriteVault`

Reads and writes the favorite marks of the workspace.
A mark is one row keyed by the entry it stands on, so an entry is marked at most once.
The row is deleted with its entry, so no mark outlives what it marks.

## `public void LFavoriteSave(long entryId)`

Marks the entry a favorite, stamping the moment.
Marking an entry that is already marked keeps the earlier stamp.

## `public void LFavoriteDelete(long entryId)`

Unmarks the entry.
The entry itself stands untouched.

## `public bool LFavoriteCheck(long entryId)`

Reports whether the entry is marked.

## `public IReadOnlyList<LCatalogFavorite> LFavoriteFind(string query)`

Finds the marked entries whose headword carries the query, folded for case and accent.
An empty query returns every marked entry.
Rows come back by headword, then language, then entry id, and the caller reorders them.
Those tie keys are the ones `LCatalogEntry` uses, so both sides agree on equal headwords.
