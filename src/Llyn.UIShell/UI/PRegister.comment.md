# PRegister.cs

## `internal sealed class PRegister`

One committed Register chip on a card's Register field.
It holds the wording shown and the id of the stored Register the chip marks.
A chip made from typed text carries a fresh id, because the row it will become does not exist yet.
The wording is kept as shown text plus whether it is unknown, so the template binds two plain values.
The state it was read from is rebuilt on the way out rather than carried through the view.

## `internal PRegister(LStateValue text, long id)`

A chip for a Register whose language and origin are not known here.
It is what a chip the user types needs, because typed text belongs to no language pack.

## `internal PRegister(LStateValue text, long id, string language)`

A chip for a Register the card already marks, keeping the id the reference is written by.
The language rides along unread.
No control on the card shows it, and a save that dropped it would erase what the pack declared.
Whether a pack ships the Register is read from the stored row, so the chip carries no mark for it.

## `internal LStateWritten PRegisterTextRead()`

The wording as written, with its mark, for the engine to read.
