# CContour.cs
Hash: `56e1f5c1decca68c`

## `public sealed record CContour(string CContourText, IReadOnlyList<int> CContourLevels, bool CContourToned)`

One syllable of a tone contour, ready to draw.
The engine parsed the reading, so the drawing only places what it is handed.

**Parameters**

- `CContourText`: the syllable as written beneath its cell.
- `CContourLevels`: the pitch levels the tone marks spell, in order.
- `CContourToned`: whether the syllable carries a tone, which draws its pitch line.

## `public static IReadOnlyList<int> CContourScale`

The pitch levels of the scale, highest first, one guide line each from the top of the box down.
The first level is the top line and each next one lies a step below.
So the drawing only scales those steps to pixels.
It comes from the rule that parses the levels, so the scale has one owner.
