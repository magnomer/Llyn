# TGlyph.cs
Hash: `744fa63fe86f45ed`

## `public sealed class TGlyph`

Covers the glyph section.
It splits a text into its Han characters and opens the entry of one character.
The split keeps Han characters only, each once, in first-seen order, and keeps an ideograph beyond the basic plane whole.
The divide reads the transcription of the glyph scheme over the headword.
A headword without a Han character and an empty glyph row divide into no cells.
So a native Korean word shows no Hanja row.
Each Han cell of the divide carries the glyph language and any other cell carries none.
So only a Han cell is linked and opens an entry.
Resolving a character makes its entry in the glyph language once and finds that same entry afterwards.
An entry of the same character in another language is never taken, so the glyph language gets its own.
The engine hands the section back for a pack that declares one and null for a pack that does not.
The reading view lists only the filled rows outside the glyph scheme, and every filled row without a section.
The character order sorts by code point, so an ideograph beyond the basic plane follows the compatibility block.
UTF-16 ordinal order would put it first, as the test shows.
A text sorts before a longer text it begins.
A blank language answers the empty font without loading a pack, so the theme's own font stands.
The fact reads the font's members rather than building a font, so it reaches product code only through the relay.
