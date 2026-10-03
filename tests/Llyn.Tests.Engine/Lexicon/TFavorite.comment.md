# TFavorite.cs
Hash: `00998332c4948308`

## `public sealed class TFavorite`

Covers the engine's favorite seam.
An entry is marked, read back, searched, and unmarked through `LEngine`.
It covers the mark carrying a stamp, which is what the favorites panel orders by.
It covers marking twice, which must leave one mark on the earlier stamp.
It covers a typed query returning only the marked entries that carry it.
It covers unmarking, which must leave the entry itself in place.
It also covers the mark going down with the entry it stands on.
