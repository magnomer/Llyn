# TAnchor.cs

## `public sealed class TAnchor`

Covers the anchors tying a reflex row to the fanqie rows it answers.
A refetch keeps the fanqie ids on the natural key, and an anchor survives it.
A placement the refetch drops takes its anchors by cascade.
Anchors round trip sorted through the archive and the loaded draft, and an unknown fanqie id is not stored.
The anchor request toggles one pair on one row alone, dirties the draft and commits with the entry.
