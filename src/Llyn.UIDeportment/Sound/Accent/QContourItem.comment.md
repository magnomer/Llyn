# QContourItem.cs
Hash: `819e805c0669a089`

## `public sealed record QContourItem(string QContourItemText, IReadOnlyList<int> QContourItemLevels, IReadOnlyList<Brush> QContourItemBrushes, bool QContourItemToned)`

One syllable of the tone contour box, in Deportment's own shape.
`QContourInk` copies it from a ready `CContour`, so the box names no Conduct record.

**Parameters**

- `QContourItemText`: the syllable as written beneath its cell.
- `QContourItemLevels`: the pitch levels its line passes through, in order.
- `QContourItemBrushes`: the theme brush of each level, in the same order, looked up by `QContourInk` from `CContourKeys`.
- `QContourItemToned`: whether the cell draws a pitch line.
