# LRegisterRow.cs

## `public sealed record LRegisterRow(`

One stored Register a register field offers, its name already split around the typed word.

**Parameters**

- `LRegisterRowId`: the stored Register, so a pick links it by id.
- `LRegisterRowLead`: the name before the match, or the whole name when the word is not found in it.
- `LRegisterRowMark`: the matched part of the name, empty when the word is not found in it.
- `LRegisterRowTail`: the name after the match.
