# PGamutItem.cs

## `internal sealed class PGamutItem`

One Register as a row of the tenor panel's catalog.
It carries the id the panel browses by, which is never the name, because a Register may be renamed.
It carries no language, because a Register belongs to none.
The count of cards marked with it is held as a number and shown as text.
A Register nothing is marked with shows nothing rather than a zero, because a zero reads as a value.
