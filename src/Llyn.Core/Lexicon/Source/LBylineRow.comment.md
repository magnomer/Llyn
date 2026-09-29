# LBylineRow.cs

## `public sealed record LBylineRow(`

One stored Author the byline dropdown offers, its name already split around the typed word.

**Parameters**

- `LBylineRowId`: the stored Author, so a pick credits it by id.
- `LBylineRowLead`: the name before the match, or the whole name when the word is not found in it.
- `LBylineRowMark`: the matched part of the name, empty when the word is not found in it.
- `LBylineRowTail`: the name after the match.
