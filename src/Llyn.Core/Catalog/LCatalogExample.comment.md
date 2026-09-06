# LCatalogExample.cs

## `public sealed record LCatalogExample(`

One Example as a browsed row: the stored record, the name of the Source it cites, and its quotation count.
The cited name travels with the row because the ordering, the match and the row all show it.
An Example citing nothing carries an empty name rather than an id nobody would recognise.

**Parameters**

- `LCatalogExampleStored` — The stored Example the row stands for.
- `LCatalogExampleSource` — The name of the Source it cites, empty where it cites none.
- `LCatalogExampleUsage` — How many places quote it.

## `public static LCatalogExample LCatalogExampleCreate(LExample example, string? source, int usage)`

Builds the row from the stored Example and the name resolved for its citation.

## `public static IReadOnlyList<LCatalogExample> LCatalogExampleSort(IReadOnlyList<LCatalogExample> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the sentence where the ordering is not one an Example answers to.
Ordering by Source orders by the resolved name rather than the id, because the id is never shown.

## `public bool LCatalogExampleMatch(string query)`

Whether one Example answers the query, over its sentence, its translation and its Source name.
