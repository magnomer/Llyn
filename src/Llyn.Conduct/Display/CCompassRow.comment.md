# CCompassRow.cs
Hash: `4e72fde382d89dbf`

## `public sealed record CCompassRow(`

One row of the floating contents, ready to show.

**Parameters**

- `CCompassRowPart`: the section the row names or stands under.
- `CCompassRowCard`: the card's place in its section's list, or null for the section itself.
- `CCompassRowName`: the wording, numbered while another row carries the same one.
- `CCompassRowNumber`: the card's number, or empty for a section.
- `CCompassRowDepth`: zero for a section and one for a card under it.
