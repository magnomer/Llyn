# PBylineItem.cs

## `internal sealed class PBylineItem`

One stored Author offered in the dropdown: the id of the stored Author and the name it reads.
Choosing a row credits that Author by id, so a rename in the workspace follows the credit.

## `public string PBylineItemLead`

The name is held in three pieces: what stands before what was typed, what was typed, and what follows.
The row draws the middle piece in weight, so a reader sees why the row is offered rather than guessing.
The comparison is the culture's own and reports how long the match ran.
A match under a culture's rules is not always as long as the text that found it.
A name the typing does not appear in is held whole in the first piece.
A row is never lost to a search that cannot point at itself.
