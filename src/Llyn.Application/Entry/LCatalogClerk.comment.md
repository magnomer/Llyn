# LCatalogClerk.cs
Hash: `56685f63108c1e84`

## `public static class LCatalogClerk`

The catalog helpers the shell reaches without naming the catalog itself.

## `public static string LCatalogClerkFormat(LCatalogFilter? filter)`

The filter as its stored text, an absent filter formatted as the empty one.

## `public static LCatalogFilter LCatalogClerkCreate(IReadOnlyList<string> hidden)`

The filter hiding the given languages, built by the filter's own rule.
A list hiding nothing is the shared empty filter.

## `public static IReadOnlyList<LCatalogRow> LCatalogClerkFind<LCatalogRow>(IEnumerable<LCatalogRow> rows, string query, LCatalogOrder order, Func<LCatalogRow, string> key, Func<LCatalogRow, int> count)`

The one rule every catalog column narrows and sorts by, so series and diwei lists agree.
Rows whose key holds the trimmed query stay, ignoring case, and a blank query keeps all.
Reverse sorts keys descending, usage sorts by count descending then key, and any other order sorts keys ascending.
