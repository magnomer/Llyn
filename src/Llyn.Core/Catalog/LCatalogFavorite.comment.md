# LCatalogFavorite.cs

## `public static class LCatalogFavorite`

The orderings the favorite catalog is listed in.
The store already answers which marked entries match, so only the ordering is decided here.

## `public static IReadOnlyList<LFavorite> LCatalogFavoriteSort(IReadOnlyList<LFavorite> favorites, LCatalogOrder order)`

Orders the marked entries under one ordering, and under the headword otherwise.
Ordering by the mark reads when the mark was made, never when the entry was written.
Ordering by grasp puts the best known entries first and breaks ties by headword.
