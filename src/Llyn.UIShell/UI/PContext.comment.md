# PContext.cs

## `internal sealed class PContext`

One Situation row on a card.
The row carries the id of the Situation it edits.
So a wording the user rewrites stays the same Situation.
A row that has never been written carries an empty id.
It is given one the moment a wording is typed into it.
That is the id it is then stored under, and the id the row shows while writing.

A row standing empty is not one thing.
The wording may never have been written.
Or it may have been written and be unreadable now.
The second is marked rather than shown.
The mark stands until the user writes over it or clears the row.
So nothing unreadable is quietly turned into nothing at all.
Writing in the row is the user saying what the wording is, which is why any edit ends the mark.

## `internal PContext()`

An empty row nothing has been written in.

## `internal PContext(LStateValue text, string id)`

The row for a stored Situation.
It holds the wording as the store knows it, under the id that names it.
A Situation cites nothing, because the user writes it rather than quoting it.

## `internal LStateValue PContextTextRead()`

What the row says its wording is: nothing written, unreadable, or the text it shows.

## `internal void PContextIdentityApply()`

Gives the row the id its Situation will be stored under, once the row holds a wording.
An id, once given, stays with the row.

## `internal void PContextClear()`

Empties the row without dropping it, which is what removing the last row on a card leaves.
An emptied row says nothing was written, because the user said so.
