# LCatalogFavorite.cs

## `public sealed record LCatalogFavorite(LEntry LCatalogFavoriteEntry, string LCatalogFavoriteMarked)`

One row of the favorite catalog: an entry the user has marked, carried with the moment it was marked.
Favorite status is no lexical object, so this row adds nothing to the entry itself.
The entry keeps its identity whether it is marked or not.
The stamp exists so the catalog can order by when the mark was made.
It also hosts the orderings the catalog is listed in, as the tag and register rows host theirs.

**Parameters**

- `LCatalogFavoriteEntry` — The marked entry, unchanged by the mark.
- `LCatalogFavoriteMarked` — Moment the mark was made, ISO 8601 UTC.

## `public static IReadOnlyList<LCatalogFavorite> LCatalogFavoriteSort(IReadOnlyList<LCatalogFavorite> favorites, LCatalogOrder order)`

Orders the marked entries under one ordering, and under the headword otherwise.
The store already answers which marked entries match, so only the ordering is decided here.
Ordering by the mark reads when the mark was made, never when the entry was written.
Ordering by grasp puts the best known entries first and breaks ties by headword.
