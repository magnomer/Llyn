# CAuthorRow.cs
Hash: `80965ba6672f0a42`

## `public sealed record CAuthorRow(long CAuthorRowId, string CAuthorRowName, int CAuthorRowPosition, bool CAuthorRowEarlier, bool CAuthorRowLater)`

One credit of the Source draft, as the imprint lists its authors.

**Parameters**

- `CAuthorRowId`: the credited author.
- `CAuthorRowName`: the author's name as shown.
- `CAuthorRowPosition`: the credit's place in the draft's order, from zero.
- `CAuthorRowEarlier`: whether the credit can move one place earlier.
- `CAuthorRowLater`: whether the credit can move one place later.
