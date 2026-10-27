# Pronunciation

These tests cover the pronunciation side of the engine.
This folder holds the tone contour tests, and its subfolders hold the rest by topic.
Nothing here reaches the web.
The contour read runs over the shipped packs, which are copied beside the test binary.

## `TContour`

Covers the tone contour parse and the engine's contour read.
Superscript digits, Chao letters and plain digits each read as levels, one syllable per mark run.
A sandhi joiner keeps only the surface tone after it.
A syllable without marks, or with a mark outside the five levels, reads with no level.
The role pick maps levels 5 to 1 onto the five colour roles, top first.
A level outside the scale takes the bottom role.
The engine draws a Mandarin contour, and draws none for English or a blank language.

## Stamp

Commit: `9377e657b395`
- `TContour.cs` `275c58ccfc06dc54`
