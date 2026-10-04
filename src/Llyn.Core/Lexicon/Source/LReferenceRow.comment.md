# LReferenceRow.cs
Hash: `1d28a386f96d681f`

## `public sealed record LReferenceRow(long LReferenceRowId, string LReferenceRowLead, string LReferenceRowMark, string LReferenceRowTail, string LReferenceRowCount)`

One stored Source a citation field offers, its byline already split around the typed word.

**Parameters**

- `LReferenceRowId` — The stored Source, so a pick cites it by id.
- `LReferenceRowLead` — The byline before the match, or the whole byline when the word is not found in it.
- `LReferenceRowMark` — The matched part of the byline, empty when the word is not found in it.
- `LReferenceRowTail` — The byline after the match.
- `LReferenceRowCount` — How many Examples already cite the Source, empty when none does.
