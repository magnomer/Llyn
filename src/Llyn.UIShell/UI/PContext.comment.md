# PContext.cs

## `internal sealed class PContext`

One committed Situation inside a card's Situation field — the boxed wording the user sees.
It carries the id the Situation is stored under, so a card keeps naming the same Situation across saves.
A Situation whose wording should change is closed and written again rather than edited in place.

A Situation the store could not read back is marked rather than shown.
The mark stands until the user closes the chip and writes the wording again.
So nothing unreadable is quietly turned into nothing at all.

## `internal PContext(string text)`

The Situation the user has just closed out of the entry.
It is given the id it will be stored under, because a written Situation is a Situation.

## `internal PContext(LStateValue text, string id)`

The chip for a stored Situation.
It holds the wording as the store knows it, under the id that names it.
An id the store never gave it stays empty, so loading a card does not read as editing one.

## `internal LStateValue PContextTextRead()`

What the chip says its wording is: unreadable, or the text it shows.
