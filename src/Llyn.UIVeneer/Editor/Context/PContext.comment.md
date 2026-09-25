# PContext.cs

## `internal sealed class PContext`

One committed Situation inside a card's Situation field — the boxed wording the user sees.
It carries the id the Situation is stored under, so a card keeps naming the same Situation across saves.
A Situation whose wording should change is closed and written again rather than edited in place.

A Situation the store could not read back is marked rather than shown.
The mark stands until the user closes the chip and writes the wording again.
So nothing unknown is quietly turned into nothing at all.

## `internal PContext(LStateValue text, long id)`

The chip for a stored Situation.
It holds the wording as the store knows it, under the id that names it.
An id the store never gave it stays empty, so loading a card does not read as editing one.

## `public LStateValue PContextText { get; }`

The wording as the draft holds it, which the template shows and a redraw compares against.
