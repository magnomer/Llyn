# CDiwei.cs

## `public sealed record CDiwei(long CDiweiId, string CDiweiKey, int CDiweiCount, bool CDiweiFinal, bool CDiweiChosen);`

One diwei cell as the yunjing initial or rime column lists it.

**Parameters**

- `CDiweiId`: the stored cell, which a click on the row names.
- `CDiweiKey`: the cell key the row prints.
- `CDiweiCount`: how many readings the cell holds.
- `CDiweiFinal`: whether the cell is a rime rather than an initial.
- `CDiweiChosen`: whether the column has this cell chosen.
