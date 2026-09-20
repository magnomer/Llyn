# LCatalogEntry.cs

## `public static class LCatalogEntry`

The orderings the entry catalog is listed in.
The store already answers which entries match, so only the ordering is decided here.

## `public static IReadOnlyList<LEntry> LCatalogEntrySort(IReadOnlyList<LEntry> entries, LCatalogOrder order)`

Orders the entries under one ordering, and under the headword where the ordering is not one an entry answers to.
A headword is read as a word, so the comparison is the reader culture and not an ordinal one.
A timestamp an entry does not carry sorts as empty.
That puts an entry of unknown age at the end of a newest-first list.
