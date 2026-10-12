# CCompassRow.cs
Hash: `55e13f23dc73ca46`

## `public sealed record CCompassRow(CCompassPart CCompassRowPart, int? CCompassRowCard, string CCompassRowName, string CCompassRowNumber, int CCompassRowDepth, bool CCompassRowMuted)`

One row of the floating contents, ready to show.

**Parameters**

- `CCompassRowPart`: the section the row names or stands under.
- `CCompassRowCard`: the card's place in its section's list, or null for the section itself.
  A count line carries the place of its meaning.
- `CCompassRowName`: the wording, numbered while another row carries the same one.
- `CCompassRowNumber`: the card's number, or empty for a section and for a count line.
- `CCompassRowDepth`: zero for a section, one for a card under it, two for the count line under a meaning.
- `CCompassRowMuted`: true for the count line, which is shown muted and never numbered as a twin.
