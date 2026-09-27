# CCatalogAuthor.cs

## `public sealed record CCatalogAuthor(`

One author a search found, as the guild's roll and union list read it.

**Parameters**

- `CCatalogAuthorId`: the stored author, zero for the row of uncredited sources.
- `CCatalogAuthorName`: the author's name as shown.
- `CCatalogAuthorWork`: how many sources credit the author, worded.
- `CCatalogAuthorUsage`: how many examples cite the author's sources.
- `CCatalogAuthorStored`: whether the row is a stored author rather than the uncredited row.
- `CCatalogAuthorChosen`: whether the vista has this author chosen.
