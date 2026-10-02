# CGlyphCell.cs
Hash: `2f4dded0bc252b1c`

## `public sealed record CGlyphCell(`

One character cell of the glyph row, mapped from the engine cell.

**Parameters**

- `CGlyphCellText`: the character the cell shows.
- `CGlyphCellLanguage`: the language the character opens an entry in, blank for a cell that opens none.
- `CGlyphCellLinked`: Core's verdict that the cell opens an entry.
