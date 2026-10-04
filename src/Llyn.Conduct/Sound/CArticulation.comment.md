# CArticulation.cs
Hash: `8a6ec94f7906876a`

## `public sealed record CArticulation(IReadOnlyList<string> CArticulationHeaders, IReadOnlyList<string> CArticulationSides, IReadOnlyList<IReadOnlyList<IReadOnlyList<string>>> CArticulationCells)`

One IPA chart of the input aid, ready to build.

**Parameters**

- `CArticulationHeaders`: the localization key of each column header, in chart order.
- `CArticulationSides`: the localization key of each row header, in chart order.
- `CArticulationCells`: per row, per column, the symbols the cell offers, none for an empty cell.
