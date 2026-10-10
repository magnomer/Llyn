# LLiveryRime.cs
Hash: `4db006890840f32c`

## `internal static class LLiveryRime`

Writes the paradigm box, the inflection table and the rime card of a Joplin entry body for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryPage` and the `link` and `lookup` it is handed.

## `private const int LLiveryRimeColumns = 12;`

The number of columns in every rime card row, from the book chip to the source badge.

## `public static void LLiveryRimeAppend(StringBuilder sheet, LLiveryPage page, Func<string, string, string, string> link, Func<string, string> lookup)`

Writes the paradigm rows, then `LLiveryInflection.LLiveryInflectionAppend`, then `LLiveryFanqieAppend`.
That is the order view mode stacks the paradigm rows, the paradigm sheet and the rime card.
It asks `link` under the entry draft's trimmed language, the name the courier keys its notes by.
A chip with an id is wrapped whole as `[chip](:/id)`, so the chip keeps its look inside the link.
An empty chip, an empty key or an empty id leaves the chip unchanged.
`LLiverySheet.LLiveryFormat` calls it right after `LLiveryHeader.LLiveryChipAppend`.

## `private static void LLiveryParadigmAppend(StringBuilder sheet, IReadOnlyList<LParadigmRow> rows)`

Writes `LLiveryPageParadigm` as a two-column Markdown table inside a `llyn-paradigm` div.
The left column holds `LParadigmRowName` as a `llyn-label` chip.
The right column holds the inflection text of `LParadigmRowFirst`, or stays empty without one.
A row with a `LParadigmRowPart` gets a `llyn-speech` chip row above it.
No rows write nothing.

## `private static void LLiveryFanqieAppend(StringBuilder sheet, IReadOnlyList<LFanqieGroup> groups, Func<string, string, string, string> wrap, Func<string, string> lookup)`

Writes `LLiveryPageFanqie` as one Markdown table inside a `llyn-card` div.
A blank line sits inside both ends of the div, so Joplin reads the table.
The header row is empty, since the card shows no column titles.
A group's heading character gets its own `llyn-heading` row.
A group's stems follow in one row, after a `llyn-series` chip read through `Display.FanqieShengfu`.
Each row then writes book, reading, tone, initial, rime, bracket, knot, medial, grade, tone class, spelling and source.
The book chip and the source badge stand on the group's first row only.
`LFanqieGroupLabel` is already blank for a repeated book, so that group shows no chip.
The tone chip is `LFanqieRowLabel`, or `Display.FanqieTone` formatted with `LFanqieRowClass` when no label is stored and the row is classed.
A row without a rime cell writes `LFanqieRowRemainder` in the rime column.
A closed medial adds `llyn-on` to its chip.
`wrap` turns a chip, its kind and its key into the chip or its link.
Each stem chip is keyed by the stem string, under `LLiveryStem.LLiveryStemKind`.
The initial chip is keyed by `LFanqieRowInitial`, under `LDiwei.LDiweiInitial`.
The rime chip is keyed by `LFanqieRowCell`, under `LDiwei.LDiweiRime`.
The tone chip is keyed by `LFanqieRowClass`, under `LDiwei.LDiweiTone`, even when a stored label shows.
Those keys are the ones the rime-table pages carry, so a chip finds its category note.
The source badge is plain text, since the page carries no address for a fanqie source.
No groups write nothing.

## `internal static string LLiveryRowFormat(IReadOnlyList<string> cells)`

Joins the cells into one Markdown table row ending in a line break.
It is internal so `LLiveryYunjing` writes its rows through it.

## `internal static string LLiveryChipFormat(string style, string text)`

A span of class `style` around the escaped `text`, or empty when `text` is empty.
`LLiveryHeader.LLiveryTextFormat` escapes the text, so a stored pipe never splits a cell.
It is internal so the reconstruction builders write their chips through it.
