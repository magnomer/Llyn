# CAuthorRow.cs
Hash: `bf9488280af627f5`

## `public sealed record CAuthorRow(long CAuthorRowId, string CAuthorRowName, int CAuthorRowPosition, bool CAuthorRowEarlier, bool CAuthorRowLater, bool CAuthorRowBlank)`

One credit of the Source draft, as the imprint lists its authors.

**Parameters**

- `CAuthorRowId`: the credited author.
- `CAuthorRowName`: the author's name as shown.
- `CAuthorRowPosition`: the credit's place in the draft's order, from zero.
- `CAuthorRowEarlier`: whether the credit can move one place earlier.
- `CAuthorRowLater`: whether the credit can move one place later.
- `CAuthorRowBlank`: whether this is the imprint's blank row, not a credit.
  The driver reads this mark, so no id value has to mean blank.
