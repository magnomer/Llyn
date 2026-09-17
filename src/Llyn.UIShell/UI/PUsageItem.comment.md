# PUsageItem.cs

## `internal sealed class PUsageItem`

Presentation item for one referring side in `PUsage`.
Carries the Entry the referring side belongs to, what names that side, and which of the two kinds it is.
The relationship is kept rather than flattened.
The row says a Meaning or a Collocation carries the Situation, never the whole Entry.
The Entry id is the way from this row to the panel holding that Entry.
The flag is resolved once for the language and handed to the row, as an index row is given one.

## `internal PUsageItem(LUsage usage, string owner, string unknown, string unnamed)`

Builds the row from one read referring side.
The owner text is the word for that kind in the user's language, handed in rather than decided here.
A side that names itself nowhere shows the unnamed text, and one marked as not known shows the mark.

## `public string PUsageItemName`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
`LTwin` writes it once the list is filled, because a repeat is only visible across rows.
`PUsageItemHeadword` keeps the plain headword for everything that is not display.

## `public LOwner PUsageItemKind`

The kind of side the row stands for, kept as the stored value beside the worded owner text.
A click reads it to decide whether the row leads to an Entry or to an Example.

## `public string PUsageItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
