# TParadigmGrid.cs
Hash: `83f07524c17c7143`

## `public sealed class TParadigmGrid`

Covers the inflection tables of the paradigm box, their rules and the switch between them.
Each box runs on its own STA thread, since it is a WPF control.
Assertions depend on fixed child indices for the switch panel and both tables.

## `public void ParadigmView_Set_ShowsCollapsedHidesExpanded()`

A box handed a sheet shows itself, the full button and the collapsed table.
The expanded table stays hidden until the full button is checked.

## `public void ParadigmSwitch_FullChecked_ShowsExpanded()`

Checking the full button hides the collapsed table and shows the expanded one.
Setting the check raises the checked event, so no click has to be faked.

## `public void ParadigmForm_Marked_PaintsMarkedRun()`

A form marked at offset 1 for 2 letters splits into three runs around the mark.
The middle run uses `Theme.Paradigm.Marked`, and the first run does not.
The test lends the application that style and removes it once it ends.

## `public void ParadigmForm_Split_PaintsRootCutAndEnding()`

A form split at 4 with its root marked paints the root, a hyphen and the ending.
The root takes `Theme.Paradigm.Marked`, and the hyphen takes `Theme.Paradigm.Cut`.
The test lends the application both styles and removes them once it ends.

## `public void ParadigmTable_TwoGroups_ClosesGroupAndRulesInside()`

A table of three lines in two groups draws exactly two rules.
The line inside the first group gets an open rule that starts at the label column.
The last line of the first group gets a closed rule from the group column.
The last line of the table gets none, and no header row adds one.

## `public void ParadigmView_NullAndNoItems_Collapses()`

A box whose sheet returns to null while it has no rows collapses again.

## `private static QParadigmSheet? TParadigmSheetCreate(IReadOnlyList<CParadigmMark> marks, string text = "tuve", int split = 0)`

A sheet with one collapsed line and one expanded line under two person headers.
It is mapped from a Conduct view by `QParadigmSheetCreate`, as the drivers map it.
The expanded line also carries a pending form with its tip key.
Every shown form uses `text`, `marks` and `split`, so one sheet serves the mark and split tests.

## `private static UIElementCollection TParadigmPartsRead(QParadigm box)`

The four parts of the box's stack, in their fixed order.
The second part is the switch panel, whose second button is the full button.
