# CSlateRow.cs

## `public sealed record CSlateRow(`

One stored Tag the tag dropdown offers, already split around the typed word.

**Parameters**

- `CSlateRowId`: the stored Tag a pick links by id.
- `CSlateRowLead`: the text before the match, or the whole text when the word is not found in it.
- `CSlateRowMark`: the matched part, which the row draws in weight.
- `CSlateRowTail`: the text after the match.
