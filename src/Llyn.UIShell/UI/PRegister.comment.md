# PRegister.cs

## `internal sealed class PRegister`

One committed Register chip on a card's Register field.
It holds the wording shown and the id of the stored Register the chip marks.
A chip made from typed text carries a fresh id, because the row it will become does not exist yet.
The wording is kept as shown text plus whether it is unknown, so the template binds two plain values.
The state it was read from is rebuilt on the way out rather than carried through the view.

## `internal PRegister(LStateValue text, long id)`

A chip for a Register, keeping the id the reference is written by.
A chip the user types carries no id yet.
Whether a pack names the Register is read from the stored row, so the chip carries no mark for it.

## `internal LStateWritten PRegisterTextRead()`

The wording as written, with its mark, for the engine to read.
