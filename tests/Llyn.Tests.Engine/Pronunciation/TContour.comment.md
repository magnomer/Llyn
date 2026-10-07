# TContour.cs
Hash: `a56cfee488b20e3b`

## `public sealed class TContour`

Covers the tone contour parse and the engine's contour read.
Superscript digits, Chao letters and plain digits each read as levels, one syllable per mark run.
A sandhi joiner keeps only the surface tone after it.
A syllable without marks, or with a mark outside the five levels, reads with no level.
The tone check reports true as soon as one syllable carries a level.
The role pick maps levels 5 to 1 onto the five colour roles, top first.
A level outside the scale takes the bottom role, and a parsed syllable carries one role per level.
The engine draws a Mandarin contour, and draws none for English or a blank language.
