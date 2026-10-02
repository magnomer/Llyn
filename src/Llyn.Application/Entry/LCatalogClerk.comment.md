# LCatalogClerk.cs
Hash: `c4680b58860e3aa1`

## `public static class LCatalogClerk`

The catalog helpers the shell reaches without naming the catalog itself.

## `public static string LCatalogClerkFormat(LCatalogFilter? filter)`

The filter as its stored text, an absent filter formatted as the empty one.

## `public static LCatalogFilter LCatalogClerkCreate(IReadOnlyList<string> hidden)`

The filter hiding the given languages, built by the filter's own rule.
A list hiding nothing is the shared empty filter.
