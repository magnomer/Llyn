# LReferenceRow.cs

## `public sealed record LReferenceRow(`

One stored Source a citation field offers, its byline already split around the typed word.

**Parameters**

- `LReferenceRowId`: the stored Source, so a pick cites it by id.
- `LReferenceRowLead`: the byline before the match, or the whole byline when the word is not found in it.
- `LReferenceRowMark`: the matched part of the byline, empty when the word is not found in it.
- `LReferenceRowTail`: the byline after the match.
- `LReferenceRowCount`: how many Examples already cite the Source, empty when none does.
