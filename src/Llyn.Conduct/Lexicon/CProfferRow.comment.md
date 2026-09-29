# CProfferRow.cs

## `public sealed record CProfferRow(`

One stored row the dropdown offers, already split around the typed word.

**Parameters**

- `CProfferRowId`: the stored row a pick links by id.
- `CProfferRowLead`: the text before the match, or the whole text when the word is not found in it.
- `CProfferRowMark`: the matched part, which the row draws in weight.
- `CProfferRowTail`: the text after the match.
- `CProfferRowCount`: the ready count of cards or Examples already using the row, empty where none is shown.
