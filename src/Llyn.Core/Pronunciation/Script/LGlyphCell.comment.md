# LGlyphCell.cs

## `public sealed record LGlyphCell(string LGlyphCellText, string LGlyphCellLanguage)`

One rune of a glyph row, as [LGlyph](LGlyph.comment.md) divides it.
The language is the one the character's entry lives in.
It is blank for a rune that is not Han, so the cell opens nothing.
