# LTagRow.cs
Hash: `9102f844e4387a6d`

## `public sealed record LTagRow(`

One stored Tag a tag field offers, its text already split around the typed word.

**Parameters**

- `LTagRowId`: the stored Tag, so a pick links it by id.
- `LTagRowLead`: the text before the match, or the whole text when the word is not found in it.
- `LTagRowMark`: the matched part of the text, empty when the word is not found in it.
- `LTagRowTail`: the text after the match.
