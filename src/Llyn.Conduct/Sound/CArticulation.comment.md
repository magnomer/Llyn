# CArticulation.cs

## `public sealed record CArticulation(`

One IPA chart of the input aid, ready to build.

**Parameters**

- `CArticulationHeaders`: the localization key of each column header, in chart order.
- `CArticulationSides`: the localization key of each row header, in chart order.
- `CArticulationCells`: per row, per column, the symbols the cell offers, none for an empty cell.
