# LCatalogExample.cs

## `public sealed record LCatalogExample(`

One Example as a browsed row: the stored record, the name of the Source it cites, and its quotation count.
The cited name travels with the row because the ordering, the match and the row all show it.
An Example citing nothing carries an empty name rather than an id nobody would recognise.

**Parameters**

- `LCatalogExampleStored` — The stored Example the row stands for.
- `LCatalogExampleSource` — The name of the Source it cites, empty where it cites none.
- `LCatalogExampleUsage` — How many places quote it.
- `LCatalogExampleCount` — The usage as shown, blank while unused, by `LCatalog.LCatalogUsageFormat`.
- `LCatalogExampleChosen` — True on the row of the Example the vista stands on, false until the vista find fills it.

## `public string LCatalogExampleText`

The sentence as the list shows it, worded by the vista find for an unknown or unwritten text.
It is the stored text until a find words it.

## `public static LCatalogExample LCatalogExampleCreate(LExample example, string? source, int usage)`

Builds the row from the stored Example and the name resolved for its citation.

## `public static IReadOnlyList<LCatalogExample> LCatalogExampleSort(IReadOnlyList<LCatalogExample> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the sentence where the ordering is not one an Example answers to.
Ordering by Source orders by the resolved name rather than the id, because the id is never shown.

## `public bool LCatalogExampleMatch(string query)`

Whether one Example answers the query, over its sentence, the text of every Gloss it carries, and its Source name.
