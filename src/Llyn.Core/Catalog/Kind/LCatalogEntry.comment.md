# LCatalogEntry.cs
Hash: `40cd70240c65e6bc`

## `public static class LCatalogEntry`

The orderings the entry catalog is listed in.
The store already answers which entries match, so only the ordering is decided here.

## `public static IReadOnlyList<LEntry> LCatalogEntrySort(IReadOnlyList<LEntry> entries, LCatalogOrder order)`

Orders the entries under one ordering, and under the headword where the ordering is not one an entry answers to.
A headword is read as a word, so the comparison is the reader culture and not an ordinal one.
A timestamp an entry does not carry sorts as empty.
That puts an entry of unknown age at the end of a newest-first list.
Entries equal under the ordering fall to the shared tie rule, so storage order never decides a place.

## `public static IOrderedEnumerable<LCatalogEntryRow> LCatalogEntrySort<LCatalogEntryRow>(IOrderedEnumerable<LCatalogEntryRow> rows, Func<LCatalogEntryRow, LEntry> entry)`

The one tie rule for every list of rows that stand for entries.
Rows equal under the chosen ordering go by language, then headword, then entry id.
The id comes last and only keeps the result stable, because it records arrival and means nothing to a reader.
The store queries listing entries by headword add the same keys, so a list reads alike from either side.
Language and headword compare in the reader culture, ignoring case, as the headword ordering does.
