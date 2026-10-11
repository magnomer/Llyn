# TParadigmGrid.cs
Hash: `2bf441a588307d49`

## `public sealed class TParadigmGrid`

Covers the inflection tables of the paradigm box, their rules and the switch between them.
Each box runs on its own STA thread, since it is a WPF control.
Assertions depend on fixed child indices for the switch track and both tables.

## `public void ParadigmView_Set_ShowsCollapsedHidesExpanded()`

A box handed a sheet shows itself, the switch track and the collapsed table.
The expanded table stays hidden until the full button is checked.

## `public void ParadigmSwitch_FullChecked_ShowsExpanded()`

Checking the full button hides the collapsed table and shows the expanded one.
Setting the check raises the checked event, so no click has to be faked.

## `public void ParadigmForm_Marked_PaintsMarkedRun()`

A form fed three pieces paints three runs, the marked middle one between two plain ones.
The middle run uses `Theme.Paradigm.Marked`, and the first run does not.
Where Core cuts the pieces is `TParadigmPiece`'s fact, so this one pins only the painting.
The test lends the application that style and removes it once it ends.

## `public void ParadigmForm_Split_PaintsRootCutAndEnding()`

A form fed a marked root with a cut paints the root and a hyphen.
A plain ending then follows, and the form paints it after the hyphen.
The root takes `Theme.Paradigm.Marked`, and the hyphen takes `Theme.Paradigm.Cut`.
The test lends the application both styles and removes them once it ends.

## `public void ParadigmTable_TwoGroups_ClosesGroupAndRulesInside()`

A table of three lines in two groups draws exactly two rules.
The line inside the first group gets an open rule that starts at the label column.
The last line of the first group gets a closed rule from the group column.
The last line of the table gets none, and no header row adds one.

## `public void ParadigmView_NullAndNoItems_Collapses()`

A box whose sheet returns to null while it has no rows collapses again.

## `private static QParadigmSheet? TParadigmSheetCreate(IReadOnlyList<CParadigmPiece> pieces)`

A sheet with one collapsed line and one expanded line under two person headers.
It is mapped from a Conduct view by `QParadigmSheetCreate`, as the drivers map it.
The expanded line also carries a pending form with its tip key.
Every other form uses `pieces`, so one sheet serves the mark and split tests.

## `private static UIElementCollection TParadigmPartsRead(QParadigm box)`

The four parts of the box's stack, in their fixed order.
The second part is the switch track, whose panel holds the full button second.
