# PRollItem.cs

## `internal sealed class PRollItem`

Presentation item for one roll row: the Author's name, its worded source count, and its citation count.
The mark is resolved once here, because the uncredited row wears a different one from every Author.
The same row serves the union list of the edit area, because both list Authors with their source counts.

## `internal PRollItem(LCatalogAuthor row, string work)`

Builds the row of one stored Author from its catalog row and the worded source count.

## `internal PRollItem(string name, string work, int usage)`

Builds the uncredited row, which stands for the Sources crediting nobody and carries the id zero.

## `public bool PRollItemChosen`

Whether the row is the chosen one, raised so the row restyles itself.
