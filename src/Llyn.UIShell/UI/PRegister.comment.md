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

A chip for a Register whose language and origin are not known here.
It is what a chip the user types needs, because typed text belongs to no language pack.

## `internal PRegister(LStateValue text, string id, string language, bool builtin)`

A chip for a Register the card already marks, keeping the id the reference is written by.
The language and the built-in mark ride along unread.
No control on the card shows either, and a save that dropped them would erase what the pack declared.

## `internal LStateValue PRegisterTextRead()`

The wording as the engine takes it, with what is known about it put back.
