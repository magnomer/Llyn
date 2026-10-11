# LSheetInflection.cs
Hash: `e0cadba77db7f1cb`

## `internal static class LSheetInflection`

Writes the rows of one inflection sheet, the grid `QParadigm` draws under its switch.
The HTML export's paradigm box and the Joplin writer `LLiveryInflection` both call it.
So the two keep one copy of the row and cell rules.
The `llyn-` class names stay, so Joplin output is unchanged.
Text is HTML encoded only, since the Joplin block is raw HTML too.

## `internal static string? LSheetInflectionResolve(string collapsed, string expanded)`

Decides whether two formatted sheets need a Short/Full switch.
It returns the one sheet to show when they do not, or null when they do.
One empty sheet, or two that render the same, need no switch, since it would change nothing.
The non-empty sheet wins, preferring the expanded one.
Two empty sheets return an empty string, so the caller writes nothing.
The paradigm box and the Joplin writer both ask here, so the decision has one copy.

## `internal static string LSheetInflectionFormat(LParadigmTable? table, Func<string, string> lookup)`

Writes the rows of one sheet, without the table tags.
Callers wrap them, so two sheets can be compared as text.
A null table or a table without lines yields an empty string.
Headers, groups and labels are localization keys, so each passes through `lookup`.
An empty key writes an empty cell, as the app leaves its grid cell empty.
The header row exists only when the sheet has headers, after two empty cells for group and label.
The first line has no rule above it.
A later line with a group gets `llyn-close`, the full rule the app draws before a group.
Any other later line gets `llyn-rule`, the dotted rule that skips the group column.

## `internal static string LSheetCellFormat(LParadigmForm form, Func<string, string> lookup)`

Reads the cell's ready text and tip, which the Core view build resolved for the app box too.
A cell with a tip is a `llyn-muted` span of its stand-in mark, titled with the tip key's text.
That is how the app shows a muted cell with a tooltip.
A text form writes the pieces `LParadigmForm.LParadigmFormScan` cuts, the same pieces `QParadigm` paints.
A marked piece becomes a `llyn-marked` span.
A piece followed by the split adds a `llyn-cut` hyphen after it.
A text form with empty text writes an empty cell, the gap where the layout has no slot.
