# COeuvre.cs

## `internal static class COeuvre`

The reference row map, ahead of the oeuvre gates that will join it.

## `internal static IReadOnlyList<CCatalogReference> COeuvreReferenceRead(IReadOnlyList<LCatalogReference> rows)`

The one map for reference rows, shared with the card, the anthology and the shelf.

## `private static CStateValue COeuvreCreditRead(string? credit, bool uncertain)`

The authors as a written value, uncertain when the engine marks them unknown.
