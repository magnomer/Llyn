# TGlyph.cs

## `public sealed class TGlyph`

Covers the glyph section: the split of a text into its Han characters and the entry one character opens.
The split keeps Han characters only, each once, in first-seen order, and keeps an ideograph beyond the basic plane whole.
Resolving a character makes its entry in the glyph language once and finds that same entry afterwards.
An entry of the same character in another language is never taken, so the glyph language gets its own.
The engine hands the section back for a pack that declares one and null for a pack that does not.
