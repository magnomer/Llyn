# PRegister.cs
Hash: `78f4581e696a5ffe`

## `internal sealed class PRegister`

One committed Register chip on a card's Register field.
It holds the wording shown and the id of the stored Register the chip marks.
A chip made from typed text carries a fresh id, because the row it will become does not exist yet.
The wording is kept as the engine's value, state and all, and the template's converter reads its mark.

## `internal PRegister(CStateWording text, long id)`

A chip for a Register, keeping the id the reference is written by.
A chip the user types carries no id yet.
Whether a pack names the Register is read from the stored row, so the chip carries no mark for it.

## `public CStateWording PRegisterText { get; }`

The wording as the draft holds it, which the template shows and a redraw compares against.
