# TContour.cs

## `public sealed class TContour`

Covers the tone contour parse and the engine's tonal check.
Superscript digits, Chao letters and plain digits each read as levels, one syllable per mark run.
A sandhi joiner keeps only the surface tone after it.
A syllable without marks, or with a mark outside the five levels, reads with no level.
The tone check reports true as soon as one syllable carries a level.
The Mandarin pack reads as tonal through the engine and the English pack does not.
