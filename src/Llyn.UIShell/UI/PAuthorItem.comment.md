# PAuthorItem.cs

## `internal sealed class PAuthorItem`

Presentation item for one credit row of the Source edit area.
A changed credit list is a rebuilt list rather than a mutated row.

## `internal PAuthorItem(LAuthor author, int position, int total)`

Builds the row from the credited Author and the place it takes in the list holding it.

## `internal PAuthorItem(int position)`

Builds the blank row that credits nobody yet, at the end of the list.
It carries the id no Author has, so the area can tell it apart.

## `public bool PAuthorItemEarlier { get; }`

Whether a row above this one exists, so the move control is offered only where it can act.

## `public bool PAuthorItemBlank => PAuthorItemId == 0;`

Whether the row credits nobody yet.
A stored Author has a positive id and a minted one a negative id.
So zero is free to mean neither.
