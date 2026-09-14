# PGlyphItem.cs

## `internal sealed record PGlyphItem(string PGlyphItemText)`

One character chip of the reading view's glyph row.
It carries only the character, because the display resolves the entry on click rather than ahead of time.
