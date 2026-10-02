# CCatalogAuthor.cs
Hash: `b46f14e4512ea3a0`

## `public sealed record CCatalogAuthor(`

One author a search found, as the guild's roll and union list read it.

**Parameters**

- `CCatalogAuthorId`: the stored author, zero for the row of uncredited sources.
- `CCatalogAuthorName`: the author's name as shown.
- `CCatalogAuthorWork`: how many sources credit the author, worded.
- `CCatalogAuthorCount`: how many examples cite the author's sources, worded by Core and blank while uncited.
- `CCatalogAuthorIcon`: the icon key of the row's mark.
  A stored author wears the guild mark, and the uncredited row the unlink mark.
- `CCatalogAuthorChosen`: whether the vista has this author chosen.
