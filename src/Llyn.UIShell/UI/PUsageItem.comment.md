# PUsageItem.cs

## `internal sealed class PUsageItem`

Presentation item for one referring side in `PUsage`.
Carries the Entry the referring side belongs to, what names that side, and which of the two kinds it is.
The relationship is kept rather than flattened: the row says a Meaning or a Collocation carries the Situation, never the whole Entry.
The Entry id is the way from this row to the panel holding that Entry.
The flag is resolved once for the language and handed to the row, as an index row is given one.

## `internal PUsageItem(LUsage usage, string owner, string unreadable, string unnamed)`

Builds the row from one read referring side.
The owner text is the word for that kind in the user's language, handed in rather than decided here.
A side that names itself nowhere shows the unnamed text, and one that cannot be read back shows the mark.
