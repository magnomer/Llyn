# PWindowGlyph.cs

## `public partial class PWindow`

The window's answer to a glyph chip: the character's entry in the glyph language, shown in the library.

## `internal void PWindowGlyphShow(string character, string language)`

Resolves the entry through the engine, which makes one when none exists, then shows it as a mention link would.
A failure is reported and nothing is shown, so a broken workspace never leaves the reader on a blank page.
