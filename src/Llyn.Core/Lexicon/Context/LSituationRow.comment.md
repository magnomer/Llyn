# LSituationRow.cs
Hash: `d6494e9dc89cf02b`

## `public sealed record LSituationRow(`

One stored Situation a situation field offers, its title already split around the typed word.

**Parameters**

- `LSituationRowId`: the stored Situation, so a pick links it by id.
- `LSituationRowLead`: the title before the match, or the whole title when the word is not found in it.
- `LSituationRowMark`: the matched part of the title, empty when the word is not found in it.
- `LSituationRowTail`: the title after the match.
- `LSituationRowCount`: how many cards already use the Situation, empty when none does.
