# LCatalogReference.cs

## `public sealed record LCatalogReference(`

One Source as a browsed row: the stored record, its shown name, its credits, and its citation count.
The name is derived once here rather than in whatever is drawing the row.
The credits and the count travel with the row because the ordering and the match both read them.

**Parameters**

- `LCatalogReferenceStored` — The stored Source the row stands for.
- `LCatalogReferenceName` — The name the Source is shown and ordered under.
- `LCatalogReferenceCredit` — The Authors credited on it, in the order the Source holds.
- `LCatalogReferenceUsage` — How many Entries and Examples cite it.

## `public static LCatalogReference LCatalogReferenceCreate(LReference reference, IReadOnlyList<LAuthor>? credits, int usage)`

Builds the row and derives the shown name from the Source itself.
A Source with no credits carries an empty list rather than nothing, so no reader tests for null.

## `public static IReadOnlyList<LCatalogReference> LCatalogReferenceSort(IReadOnlyList<LCatalogReference> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the name where the ordering is not one a Source answers to.
Ordering by year puts the states in order first, so an unstated year is never read as an early one.
Ordering by author reads the first credit, because the credit order belongs to the Source.
A Source with no credits is ordered by the authorship state it states.
That is not the same as being credited.

## `public bool LCatalogReferenceMatch(string query)`

Whether one Source answers the query, over its four texts and its credited names.
Only a field standing specified carries text, so an unknown field matches nothing.
