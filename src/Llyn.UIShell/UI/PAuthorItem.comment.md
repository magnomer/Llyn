# PAuthorItem.cs

## `internal sealed class PAuthorItem`

Presentation item for one Author, in the credit list and in the crediting menu alike.
It is one Author under one base, because the row offered and the row credited name the same thing.
A changed credit list is a rebuilt list rather than a mutated row.

## `internal PAuthorItem(LAuthor author, int position, int total)`

Builds the row from the stored Author and the place it takes in the list holding it.

## `public bool PAuthorItemEarlier { get; }`

Whether a row above this one exists, so the move control is offered only where it can act.
