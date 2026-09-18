# LCatalogReference.cs

## `public sealed record LCatalogReference(`

One Source as a browsed row: the stored record, its shown name, its byline, its credits, and its citation count.
The name and the byline are derived once here rather than in whatever is drawing the row.
The credits and the count travel with the row because the ordering and the match both read them.

**Parameters**

- `LCatalogReferenceStored` — The stored Source the row stands for.
- `LCatalogReferenceName` — The name the Source is shown and ordered under.
- `LCatalogReferenceByline` — The `Author (Year)` line a citation of it is written as.
- `LCatalogReferenceCredit` — The Authors credited on it, in the order the Source holds.
- `LCatalogReferenceUsage` — How many Entries and Examples cite it.
- `LCatalogReferenceChosen` — True on the row of the Source the vista stands on, false until the vista find fills it.

## `public static LCatalogReference LCatalogReferenceCreate(LReference reference, IReadOnlyList<LAuthor>? credits, int usage)`

Builds the row and derives the shown name and the byline from the Source and its credits.
A Source with no credits carries an empty list rather than nothing, so no reader tests for null.

## `public static IReadOnlyList<LCatalogReference> LCatalogReferenceSort(IReadOnlyList<LCatalogReference> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the name where the ordering is not one a Source answers to.
Ordering by year puts the states in order first, so an unstated year is never read as an early one.
Ordering by author reads the first credit, because the credit order belongs to the Source.
A Source with no credits is ordered by the authorship state it states.
That is not the same as being credited.

## `public bool LCatalogReferenceMatch(string query)`

Whether one Source answers the query, over its four texts, its year and its credited names.
The query is broken into terms at blanks, commas and parentheses, and every term must hit some field.
So a citation typed as it is shown, `Darwin (1859)`, finds the work of that author in that year.
Only a field standing specified carries text, so an unknown field matches nothing.
