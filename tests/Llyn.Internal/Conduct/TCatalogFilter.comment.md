# TCatalogFilter.cs

## `public sealed class TCatalogFilter`

Covers the language filter a driver ticks in a catalog menu.
A hidden language fails the match whatever its case, and any other language passes.
A row with no language passes unless the empty name is hidden.
