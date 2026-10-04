# LBylineRow.cs
Hash: `6b68dcd53dc298d0`

## `public sealed record LBylineRow(long LBylineRowId, string LBylineRowLead, string LBylineRowMark, string LBylineRowTail)`

One stored Author the byline dropdown offers, its name already split around the typed word.

**Parameters**

- `LBylineRowId` — The stored Author, so a pick credits it by id.
- `LBylineRowLead` — The name before the match, or the whole name when the word is not found in it.
- `LBylineRowMark` — The matched part of the name, empty when the word is not found in it.
- `LBylineRowTail` — The name after the match.
