# CCatalogFilter.cs

## `public sealed record CCatalogFilter(IReadOnlyList<string> CCatalogFilterHidden)`

The languages a catalog hides, as a driver ticks them in a menu.
The controller maps it to and from the engine's filter.

**Parameters**

- `CCatalogFilterHidden`: the language names hidden from the rows, empty when nothing is hidden.

## `public bool CCatalogFilterMatch(string? language)`

Whether a row in this language stays shown, so a driver can tick its box.
Names compare without case, as the engine's filter compares them.
