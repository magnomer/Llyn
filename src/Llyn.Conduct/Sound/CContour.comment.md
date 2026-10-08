# CContour.cs
Hash: `92673f2280cc51c7`

## `public sealed record CContour(string CContourText, IReadOnlyList<int> CContourLevels, IReadOnlyList<string> CContourKeys, bool CContourToned)`

One syllable of a tone contour, ready to draw.
The engine parsed the reading, so the drawing only places what it is handed.
Conduct drops every level the scale lacks, since the drawing has no guide line for it.
A toned syllable always keeps at least one level, so its pitch line always has a point.

**Parameters**

- `CContourText`: the syllable as written beneath its cell.
- `CContourLevels`: the pitch levels the tone marks spell, in order, each one a level of `CDisplayAccent.CDisplayAccentScale`.
- `CContourKeys`: the theme brush key of each level, in the same order as `CContourLevels`, chosen in Conduct.
- `CContourToned`: whether the syllable carries a tone with at least one level, which draws its pitch line.

## `internal static IReadOnlyList<CContour> CContourRead(IReadOnlyList<LContour> syllables, IReadOnlyList<int> scale)`

Maps the contour syllables into ready ones over `scale`.
The editor's contour and the reading view's block share it, each passing the language port's scale.
It takes the scale as a parameter, so a test can feed levels the engine's scale never meets.
Each syllable is handed whole to `CContourInk.CContourInkBuild`, which keeps the levels and chooses their keys.
