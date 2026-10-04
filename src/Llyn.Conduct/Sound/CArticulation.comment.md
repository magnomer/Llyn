# CArticulation.cs
Hash: `8a6ec94f7906876a`

## `public sealed record CArticulation(IReadOnlyList<string> CArticulationHeaders, IReadOnlyList<string> CArticulationSides, IReadOnlyList<IReadOnlyList<IReadOnlyList<string>>> CArticulationCells)`

One IPA chart of the input aid, ready to build.
Conduct gives one cell row per side, and no row is wider than the headers.
A row may be shorter than the headers, and its missing columns are blank.
So the chart builds a cell only where a side and a header meet.

**Parameters**

- `CArticulationHeaders`: the localization key of each column header, in chart order.
- `CArticulationSides`: the localization key of each row header, in chart order.
- `CArticulationCells`: per side, per column, the symbols the cell offers, none for an empty cell.
