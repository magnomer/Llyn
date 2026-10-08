# CContour.cs
Hash: `8c9840e0cfeed9e7`

## `public sealed record CContour(string CContourText, IReadOnlyList<int> CContourLevels, IReadOnlyList<string> CContourKeys, bool CContourToned)`

One syllable of a tone contour, ready to draw.
The engine parsed the reading, so the drawing only places what it is handed.
Conduct drops every level the scale lacks, since the drawing has no guide line for it.
A toned syllable always keeps at least one level, so its pitch line always has a point.

**Parameters**

- `CContourText`: the syllable as written beneath its cell.
- `CContourLevels`: the pitch levels the tone marks spell, in order, each one a level of `CDisplaySound.CDisplaySoundScale`.
- `CContourKeys`: the theme brush key of each level, in the same order as `CContourLevels`, chosen in Conduct.
- `CContourToned`: whether the syllable carries a tone with at least one level, which draws its pitch line.
