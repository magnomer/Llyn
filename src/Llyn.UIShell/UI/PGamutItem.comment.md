# PGamutItem.cs

## `internal sealed class PGamutItem`

One Register as a row of the tenor panel's catalog.
It carries the id the panel browses by, which is never the name, because a Register may be renamed.
It carries the language of the pack that ships it, and whether a pack ships it at all.
A row a pack ships shows its language, so a shelf holding several packs never reads as one flat list.
The count of cards marked with it is held as a number and shown as text.
A Register nothing is marked with shows nothing rather than a zero, because a zero reads as a value.
