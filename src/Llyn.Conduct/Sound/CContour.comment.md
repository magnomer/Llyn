# CContour.cs

## `public sealed record CContour(string CContourText, IReadOnlyList<int> CContourLevels, bool CContourToned)`

One syllable of a tone contour, ready to draw.
The engine parsed the reading, so the drawing only places what it is handed.

**Parameters**

- `CContourText`: the syllable as written beneath its cell.
- `CContourLevels`: the pitch levels the tone marks spell, in order.
- `CContourToned`: whether the syllable carries a tone, which draws its pitch line.

## `public const int CContourFloor`

The lowest pitch level of the scale, the bottom guide line.
It comes from the rule that parses the levels, so the scale has one owner.

## `public const int CContourCeiling`

The highest pitch level of the scale, the top guide line.
