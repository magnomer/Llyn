# CLecternGlyph.cs
Hash: `00744b0b4ab090a8`

## `public sealed record CLecternGlyph(`

The glyph row of the reading view for the shown entry, ready to show.
The display reads it whenever an entry opens, so the driver only draws it.

**Parameters**

- `CLecternGlyphShown`: whether the row has any cell, which shows its section and its shared label column.
- `CLecternGlyphKey`: the heading's localization key, chosen by `CScheme.CSchemeKeyRead`.
- `CLecternGlyphName`: the glyph scheme's own name, the heading when the key has no text.
- `CLecternGlyphCells`: the cells of the row, one per character, in headword order.
- `CLecternGlyphFont`: the pack's glyph typography for the shown language.
