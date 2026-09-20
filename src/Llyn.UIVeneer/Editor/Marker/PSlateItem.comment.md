# PSlateItem.cs

## `internal sealed class PSlateItem`

One stored tag offered in the dropdown: the id of the stored Tag and the text it reads.
Choosing a row links that Tag by id, so a rename in the workspace follows the card.

## `public string PSlateItemLead`

The text is held in three pieces: what stands before what was typed, what was typed, and what follows.
The row draws the middle piece in weight, so a reader sees why the row is offered rather than guessing.
The comparison is the culture's own and reports how long the match ran.
A match under a culture's rules is not always as long as the text that found it.
A tag the typing does not appear in is held whole in the first piece.
A row is never lost to a search that cannot point at itself.
