# TAnchor.cs

## `public sealed class TAnchor`

Covers the anchors tying a reflex row to the fanqie rows it answers.
A refetch keeps the fanqie ids on the natural key, and an anchor survives it.
A placement the refetch drops takes its anchors by cascade.
Anchors round trip sorted through the archive and the loaded draft, and an unknown fanqie id is not stored.
The row scan leaves out an unstored row and marks a stored one held and estimated.
The anchor text of a headword longer than one glyph is empty, though its row is anchored.
The anchor request toggles one pair on one row alone, dirties the draft and commits with the entry.
