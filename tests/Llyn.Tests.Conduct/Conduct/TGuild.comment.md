# TGuild.cs
Hash: `0a14cdf002b0278e`

## `public sealed class TGuild`

Covers how the authors panel lists, chooses and shows Authors, end to end on a real workspace.
The roll leads with the uncredited row, and a query narrows it.
A roll that drops the chosen Author closes the panel.
No order keeps the ordering, and a hidden kind marks the roll filtered.
With nothing chosen the vita is nobody's, and a chosen Author shows its vita, fellows and Sources.
An Author click records the station it leaves before the Author opens.
A Source click shows the colophon.
A confirmed delete removes the Author, a workspace notice empties the panel, and an Author notice refreshes the roll.
A print on the author side prints nothing.
The autograph desk lives in `TGuildScribe`, and the union of two Authors in `TGuildUnion`.

## `internal static CGuild TGuildPrepare(CAtelier atelier, CEnvoy envoy)`

Builds the panel over the atelier, and the panel restores both vistas itself.
Its seam answers that the tab is in front, and its marshal runs each answer at once.
Every `TGuild` sibling class builds its panel through it, except where a test needs its own marshal.
