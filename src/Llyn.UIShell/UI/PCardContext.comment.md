# PCardContext.cs

## `internal sealed partial class PCard`

The Situation rows a card carries, kept apart from the card's own fields.
It is for the same reason the Example rows are: one file, one responsibility.

## `internal void PCardContextShow(IReadOnlyList<LSituationDraft> drafts)`

Replaces the rows with the stored Situations of the card being loaded.
It leaves one empty row when the card references none.

## `internal IReadOnlyList<LSituationDraft> PCardContextRead()`

What the card says its Situations are.
A row nothing was written in is left out rather than read as an empty Situation.

## `internal void PCardContextInsert(PContext row)`

Opens a new row directly beneath the one the user asked from.

## `internal void PCardContextRemove(PContext row)`

Drops the row, except when it is the last one: the last row is emptied instead.

## `internal void PCardContextUpdate()`

Numbers the rows while there is more than one.
