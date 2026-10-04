# CContour.cs
Hash: `56e1f5c1decca68c`

## `public sealed record CContour(string CContourText, IReadOnlyList<int> CContourLevels, bool CContourToned)`

One syllable of a tone contour, ready to draw.
The engine parsed the reading, so the drawing only places what it is handed.
Conduct drops every level the scale lacks, since the drawing has no guide line for it.
A toned syllable always keeps at least one level, so its pitch line always has a point.

**Parameters**

- `CContourText`: the syllable as written beneath its cell.
- `CContourLevels`: the pitch levels the tone marks spell, in order, each one a level of `CContourScale`.
- `CContourToned`: whether the syllable carries a tone with at least one level, which draws its pitch line.

## `public static IReadOnlyList<int> CContourScale`

The pitch levels of the scale, highest first, one guide line each from the top of the box down.
The first level is the top line and each next one lies a step below.
So the drawing only scales those steps to pixels.
It comes from the rule that parses the levels, so the scale has one owner.
