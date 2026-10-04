# CProfferRow.cs
Hash: `87a509fbfbd29b73`

## `public sealed record CProfferRow(long CProfferRowId, string CProfferRowLead, string CProfferRowMark, string CProfferRowTail, string CProfferRowCount = "")`

One stored row the dropdown offers, already split around the typed word.

**Parameters**

- `CProfferRowId`: the stored row a pick links by id.
- `CProfferRowLead`: the text before the match, or the whole text when the word is not found in it.
- `CProfferRowMark`: the matched part, which the row draws in weight.
- `CProfferRowTail`: the text after the match.
- `CProfferRowCount`: the ready count of cards or Examples already using the row, empty where none is shown.
