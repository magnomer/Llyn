# PRegister.cs

## `internal sealed class PRegister`

One committed Register chip on a card's Register field.
It holds the wording shown and the id of the stored Register the chip marks.
A chip made from typed text carries a fresh id, because the row it will become does not exist yet.
The wording is kept as shown text plus whether it is unreadable, so the template binds two plain values.
The state it was read from is rebuilt on the way out rather than carried through the view.

## `internal PRegister(string text)`

A chip for a wording the user typed, with an id nothing is stored under yet.

## `internal PRegister(LStateValue text, string id)`

A chip for a Register the card already marks, keeping the id the reference is written by.

## `internal LStateValue PRegisterTextRead()`

The wording as the engine takes it, with what is known about it put back.
