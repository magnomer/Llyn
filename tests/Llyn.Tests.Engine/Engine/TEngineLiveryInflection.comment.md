# TEngineLiveryInflection.cs
Hash: `f92f64ecc5403946`

## `public sealed class TEngineLiveryInflection`

Writes the inflection table of a stored page through the `TLiveryFormat` relay.

## `public void LiveryFormat_InflectionView_WritesExpandedTableWithHeaderLabelAndForm()`

Gives a stored page a hand-built view whose collapsed and expanded sheets differ.
Builds the view, its sheets, lines and forms through the `TInterfaceInflection` relays.
The note mirrors the app's Short/Full switch without script, so both sheets reach the body.
Checks the `llyn-inflection-box` div opens the `llyn-inflection-full` details.
The box has its own class, since the rime box's `llyn-paradigm` fill would leak in.
Checks the summary carries the `Paradigm.Short` and `Paradigm.Full` captions in their switch spans.
Checks the `llyn-inflection` table inside the details opens on its own line.
Checks the header row, the group cell, the label cell and the form cell of the expanded sheet there.
Checks no label of the collapsed sheet sits inside the details.
Checks the `llyn-inflection-short` table follows the details with the collapsed sheet's line.
Checks the block closes with the blank line Markdown needs.

## `public void LiveryFormat_EmptyCollapsedSheet_WritesOneTableWithoutSwitch()`

Gives a stored page a view whose collapsed sheet has no lines.
Checks the expanded sheet is written as one `llyn-inflection` table inside the `llyn-inflection-box` div.
Checks the div closes with a blank line.
Checks no `llyn-inflection-full` details and no `llyn-inflection-short` table appear.

## `public void LiveryFormat_IdenticalSheets_WritesOneTableWithoutSwitch()`

Gives a stored page a view whose collapsed and expanded sheets are the same table.
Checks exactly one table is written and no `llyn-inflection-full` details appears.
Checks the table sits inside the `llyn-inflection-box` div, closed by a blank line.

## `public void LiveryFormat_MissingAndMarkedForms_RenderMutedGlyphWithTipAndMarkedRun()`

Gives a stored page a line with a marked split form, a lost form and an unknown form.
Builds the sheet and its ready forms through the `TInterfaceInflection` relays.
Checks the marked run and the cut hyphen spans of the split form.
Checks the lost form writes a muted ellipsis titled with its tip key.
Checks the unknown form writes a muted dash titled with its tip key.

## `public void LiveryFormat_NullInflectionView_WritesNoInflectionTable()`

Writes a stored page whose inflection view is null.
Checks no `llyn-inflection` class appears.

## `private static LParadigmForm TLiveryFormCreate(string text)`

A plain text form with no marks, for the hand-built sheets, built through the `TParadigmFormCreate` relay.
