# LCatalogAuthor.cs
Hash: `4ce4794a49e22664`

## `public sealed record LCatalogAuthor(`

One Author as a browsed row: the stored record, the Sources crediting it, and the places citing those.
Both counts travel with the row because the ordering reads them and the row draws them.
An Author credited nowhere is still a row, because the workspace still holds it.

**Parameters**

- `LCatalogAuthorStored` — The stored Author the row stands for.
- `LCatalogAuthorWork` — How many Sources credit the Author.
- `LCatalogAuthorUsage` — How many Entries and Examples cite the Sources crediting the Author.
- `LCatalogAuthorChosen` — True on the row of the Author the vista stands on, false until the vista find fills it.

## `public string LCatalogAuthorName { get; init; }`

The name as the row shows it.
It is the stored name until the vista find numbers twins apart, while sort and match read the stored name.

## `public string LCatalogAuthorCount`

The citation count as shown, blank while uncited, by `LCatalog.LCatalogUsageFormat`.
It is read off the count on every access, so a copied row never shows a stale count.

## `public static LCatalogAuthor LCatalogAuthorCreate(LAuthor author, IReadOnlyList<LReference>? works, IReadOnlyDictionary<long, int>? usage)`

Builds the row and sums the citations of every Source crediting the Author.
A Source cited nowhere adds nothing, and an Author with no Sources counts zero on both.

## `public static IReadOnlyList<LCatalogAuthor> LCatalogAuthorSort(IReadOnlyList<LCatalogAuthor> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the name where the ordering is not one an Author answers to.
The two count orderings fall back to the name, so equal counts still read in a stable order.

## `public bool LCatalogAuthorMatch(string query)`

Whether the Author answers the query over its name alone, because a name is all it carries.
