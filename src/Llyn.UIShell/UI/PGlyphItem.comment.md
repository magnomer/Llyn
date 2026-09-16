# PGlyphItem.cs

## `internal sealed record PGlyphItem(string PGlyphItemText, string PGlyphItemLanguage)`

One character chip of the reading view's glyph row.
The language is the one the character's entry lives in.
It is blank for a rune that is not Han.
A blank language makes the chip inert, so punctuation is drawn like the editor field draws it but opens nothing.
