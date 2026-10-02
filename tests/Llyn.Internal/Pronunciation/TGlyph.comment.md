# TGlyph.cs
Hash: `7e364e23da41c029`

## `public sealed class TGlyph`

Covers the glyph section.
It splits a text into its Han characters and opens the entry of one character.
The split keeps Han characters only, each once, in first-seen order, and keeps an ideograph beyond the basic plane whole.
The divide reads the transcription of the glyph scheme over the headword.
Each Han cell of the divide carries the glyph language and any other cell carries none.
So only a Han cell is linked and opens an entry.
Resolving a character makes its entry in the glyph language once and finds that same entry afterwards.
An entry of the same character in another language is never taken, so the glyph language gets its own.
The engine hands the section back for a pack that declares one and null for a pack that does not.
The reading view lists only the filled rows outside the glyph scheme, and every filled row without a section.
