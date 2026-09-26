# TFavorite.cs

## `public sealed class TFavorite`

Covers the engine's favorite seam.
An entry is marked, read back, searched, and unmarked through `LEngine`.
It covers the mark carrying a stamp, which is what the favorites panel orders by.
It covers marking twice, which must leave one mark on the earlier stamp.
It covers what marking must never do: create an entry, change one, or change its identity.
It also covers the mark going down with the entry it stands on.
