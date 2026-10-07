# CContourInk.cs
Hash: `3998d9c0c905ba2f`

## `internal static class CContourInk`

Turns one Core contour syllable into the ready `CContour` with a theme brush key per level.
`CSounding.LSoundingContourRead` hands it each syllable, so the drawing only looks the key up.

## `internal static CContour CContourInkBuild(LContour syllable, IReadOnlyList<int> scale)`

Pairs each level with its Core colour role and drops every level outside `scale`.
The drawing has no guide line for a dropped level.
Each kept role becomes its key through `CContourInkRead`, in level order.
A toned syllable left with no level is marked untoned, so a drawn pitch line always has a point.

## `private static string CContourInkRead(LContourRole role)`

Maps one Core colour role to its `Theme.Contour` brush key by name.
An unknown role throws, so a new Core role cannot pass undrawn.
