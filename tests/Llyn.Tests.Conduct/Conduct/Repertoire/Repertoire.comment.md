# Repertoire

This folder holds the repertoire panel's gates.
They cover the situation catalog, occurrence list, hold, rows, scribe, portrait and vista.

## `TAtlas`

Covers the repertoire panel's situation list end to end on a real workspace.
Before a vista arrives the list reads nothing, and a blank title shows the localized untitled word.
A failing engine shows the load failure once and answers no rows.
The order menu offers name, kind and usage.
A typed query narrows the rows, no order keeps the ordering, and a hidden language marks the list filtered.
A Situation a card cites is worded as cited once when chosen, and the filter offers the catalog's languages.
The tally is empty before a vista arrives and reads none while nothing is chosen.
A tally the engine refuses shows the load failure once and answers an empty tally.
The draft map answers none for no Situation, and a stored one carries every field with its unknown mark.
The shown-situation map words a written field by its text and an unknown one by the unknown mark.
An unwritten title words untitled, and an unwritten kind or description words nothing.
It mutes only an unwritten field, and carries only media rows whose location is not blank.
The shown description arrives parsed, so a bold span and a list item are ready blocks.
The list is built through its internal constructor over the atelier's ports and a repertoire desk.
Its seams answer that the tab is in front and that every finish succeeds.

## `TOccurrence`

Covers the repertoire panel's occurrence list end to end on a real workspace.
Its rows are the entries referencing the chosen Situation, and every entry while none is chosen.
An unmatched query empties the rows, and the empty list then reads as unmatched rather than vacant.
A failed rows read shows the load failure once and answers no rows.
The empty key reads vacant before a vista arrives.
Its print and export are covered by `TRepertoirePortrait` through the repertoire panel.
With no vista the file name reads entry.
Its flag-fill load answers the same rows once the fill has run.

## `TRepertoire`

Covers how the repertoire panel chooses and shows a Situation, end to end on a real workspace.
With a Situation chosen, a fresh start opens a linked entry that shows the link from its first paint.
An opened Situation shows on the vignette, and one hidden by the query drops the query first.
A click with nothing unsaved records the voyage and shows the Situation.
A workspace notice clears both lists.
An entry notice keeps the display while an occurrence is chosen.
A rows read that no longer lists the shown Situation clears both lists.
The occurrence side deletes nothing.
The held scenario lives in `TRepertoireScribe`, and the print and export gates in `TRepertoirePortrait`.

## `TRepertoirePrepare`

Builds the repertoire over the atelier and the envoy the test supplies.
Its seam answers that the tab is in front, and its marshal runs each notice at once.
Building the repertoire restores one vista, and this helper restores none after it.

## `TRepertoireTitleDefer`

Writes a title into the held scenario, so the playwright's desk holds an unsaved change.

## `TRepertoireHold`

Covers two hooks that building a repertoire registers, and the playwright's typing gates.
A typed title is written and answers the typed line, and a cleared description answers its hint.
A draft notice on the scenario desk reaches the driver through the marshal the repertoire was given.
The window's exit gate closes the entry editor and stops the display's playback.
A typed kind is worded by the measure only while it is empty.
A stored unknown kind asks with the unknown mark, and a cancel raises the blank scenario.

## `TRepertoirePortrait`

Covers the repertoire panel's export and print gates on a real workspace.
An export writes only the entry on display, and a print with nothing chosen prints nothing.

## `TRepertoireRow`

Covers the situation rows the atlas lists, which arrive with their kind and count already worded.
An unknown kind reads with the localized unknown word, and a blank kind reads blank.
A Situation nothing cites carries a blank count.

## `TRepertoireScribe`

Covers the playwright's scenario desk, how it opens and how it is left, on a real workspace.
It builds each repertoire through `TRepertoire.TRepertoirePrepare`.
A fresh start with no row chosen opens a blank scenario under a checked scribe.
A saved fresh scenario shows the stored Situation on the vignette, outside the scribe.
A stored leave on a click stores the scenario and shows the clicked Situation.
A kept leave stays on the scenario, from a Situation click, an occurrence click and the leave question.
A kept Situation click records nothing.
A leave with nothing unsaved asks nothing.
Closing the scenario cancels the desk, and closing the occurrence editor falls back to the chosen Situation.
An entry notice with no chosen occurrence keeps the scenario held.

## `TRepertoireVista`

Covers the repertoire panel's vista restore and its observers on a real workspace.
A second restore carries both held queries into the fresh vistas.
After it a Situation notice raises each list once, through the marshal, from the fresh vistas only.
A create alone restores once, so the same notice after only a create raises once too.
An entry notice refills the situation rows, so their tallies follow.
A workspace notice closes the chosen Situation and tells the driver.
A settings notice raises the situation rows once, through the marshal.
The flag-fill load answers the rows and their languages once the fill has run.

## Stamp

Commit: `bd299c638b2c`
- `TAtlas.cs` `3c441439964eefec`
- `TOccurrence.cs` `536fa923acc28ac4`
- `TRepertoire.cs` `9caa5ec9bcceaeec`
- `TRepertoireHold.cs` `36998939fdcde2a4`
- `TRepertoirePortrait.cs` `fc919f1d67df0ecc`
- `TRepertoireRow.cs` `fee32a749b0766b0`
- `TRepertoireScribe.cs` `c33d7758ddf1ec02`
- `TRepertoireVista.cs` `c805a883a8b90f6d`
