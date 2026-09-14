# LCatalogAuthor.cs

## `public sealed record LCatalogAuthor(`

One Author as a browsed row: the stored record, the Sources crediting it, and the places citing those.
Both counts travel with the row because the ordering reads them and the row draws them.
An Author credited nowhere is still a row, because the workspace still holds it.

**Parameters**

- `LCatalogAuthorStored` — The stored Author the row stands for.
- `LCatalogAuthorWork` — How many Sources credit the Author.
- `LCatalogAuthorUsage` — How many Entries and Examples cite the Sources crediting the Author.

## `public static LCatalogAuthor LCatalogAuthorCreate(LAuthor author, IReadOnlyList<LReference>? works, IReadOnlyDictionary<long, int>? usage)`

Builds the row and sums the citations of every Source crediting the Author.
A Source cited nowhere adds nothing, and an Author with no Sources counts zero on both.

## `public static IReadOnlyList<LCatalogAuthor> LCatalogAuthorSort(IReadOnlyList<LCatalogAuthor> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the name where the ordering is not one an Author answers to.
The two count orderings fall back to the name, so equal counts still read in a stable order.

## `public bool LCatalogAuthorMatch(string query)`

Whether the Author answers the query over its name alone, because a name is all it carries.
