# Reflex

The reflex block of the input panel and the inflection (paradigm) box.
The reflex rows are the readings of a headword.
The paradigm box lists a headword's forms and optionally an inflection sheet.
Both read Deportment items only, so no Conduct record reaches the surface.

## `QReflex`

The reflex rows of the input panel, under the headword at the head of the reading stack.
They are the rows the reading view prints, with bare fields where it prints text.
Romanization, meaning and note each have a field after the reading.
The region is only a hover on the language field.
Folded languages hide under the hinge `PReflexHinge`, opened or closed as the engine stores it for the held entry.
All edit requests go through `CKindred`.
Draft refreshes reconcile rows by id, and typed answers update retained rows in place.
A typed cell is handed to `CKindred.CKindredSet`, and the answer's row goes to the row as it stands.
So no Conduct rule is replayed, and the list takes every row's lead from the same answer.
A null row id in an add is Conduct's word for the header's plus, so no magic id is sent.
The clerk places the new row with the pressed row's language and kind.
A refresh paints the ready block from `CKindred.CKindredRead`, and the bulletin's draft is not read.
A row still being typed into is kept.
The stack and the fetch-again button show and hide together by the block's shown verdict.
The opened state, anchor labels and fetching line are painted after the rows.
Anchor labels repaint on a desk start and on a fanqie change.
A stored opened state comes back through the fold bulletin, which repaints the draft.
The hinge is heard on its click alone, so a repaint that sets it writes nothing back.
When the gate refuses the write, the hinge goes back to its previous state.
A stored write paints nothing there, because the fold bulletin repaints.
The fetch-again command is bound on its own button, with its size hold bound first.
The hold fixes the table's width and height, so the button stays where it was pressed while rows are fetched.
The gate does nothing for a draft not yet stored, and the repaint then lets the size go.
The held size is also let go once the fill is over, so the new rows size the table again.
The fetching line and the turning icon follow `CKindred.CKindredPending`.
The icon turns while the button carries the `Pending` cue.
The anchor command is heard by the anchor menu's observe on `QAnchor`.

## `QReflexCommand`

The routed commands of the reflex block, bound once on the panel with the row as parameter.
Addition adds a row after the pressed one, in the same language and kind.
Removal drops the pressed row.
Main marks or unmarks the row as the reading in common use.
Anchor opens the anchor dropdown under the row's placement label.
Renewal fetches the reflexes again and is raised by the renewal button rather than by a row.
It carries no parameter, and the size hold runs before the fetch.

## `QReflexFrame`

The frame the Veneer places around the reflex list, on the reading view and in the editor.
It draws nothing and only hands the framework a surface peer for itself and its list.
So the dozens of text blocks a Han character's readings make are not reported one by one to accessibility.
The fields and buttons inside the rows stay reachable through that peer.
The list stays a plain items control, since only an element's own type can hand over a peer.
The peer lists only the focusable controls beneath the framed list.

## `QParadigm`

The box that lists paradigm rows and optionally draws an inflection sheet.
The sheet is two tables, a collapsed one and an expanded one.
It is a drawn leaf deriving from `Decorator`, and the Veneer only places it.
It is built in code like `PGrasp`, so no template has to be kept in step.
It never asks the engine which rows to show.
It collapses while it holds nothing, so a headword without forms leaves no gap.
It is collapsed with no sheet, so a box without one draws no empty track.
The list is a shared size scope, so part and name columns line up across every row.
The row template is read from the theme by key.
Each realized row is filled in code, since the template carries no bindings.
The box never takes focus, and only the two switch buttons are tab stops.
The switch is two text buttons on one track, so both choices stay readable at once.
They share the track's panel as their group, so checking one unchecks the other.
The short button starts checked, so the collapsed table shows first.
The track carries the `Paradigm.Fold` tooltip and accessible name.
A sheet change rebuilds both tables and keeps the buttons' state.
So a repaint after a save does not fold the table back.
The fold is a visual state only, so it calls no gate and stores nothing.
Both buttons call it on check, since an unchecked button raises no check of its own.
The button colours come from the state sheet, so no handler decides a colour from a control.
With no sheet both tables stay hidden.
A row with a tip key shows its form muted with that tooltip.
A row without a tip clears the mark, so a reused row shows no stale tooltip.

### Table layout

A table is a grid of a group column, a label column and one column per form.
The form columns number the larger of the header count and the widest line.
Headers take the first row when there are any.
Group, label and header texts are resource keys, looked up like tip keys.
An empty key adds no text block, so its cell stays blank.
A group name shares the row of its group's first line, so form columns hold one position across groups.
Lines keep the order the sheet gives, and rules mark the group ends that order implies.
The header row and every line but the last get a rule.
The rule shares the row with its cells at their bottom.
A closed rule spans every column, since it ends the header or a whole group.
An open rule starts at the label column, so the group name stands clear of the inner rules.
The span is at least one column, so a table without forms still draws a valid rule.

### Forms

A form paints one run per piece Core cut, in order, so it counts no letters itself.
A marked piece takes `Theme.Paradigm.Marked`, leaving its colour to the theme.
A piece the split follows adds a hyphen run styled by `Theme.Paradigm.Cut`.
A form with a tip key shows muted with that tooltip.

## Paradigm data

These records copy ready Conduct records, so the box names no Conduct type.
Each is built once and never changes, and a new read hands the box new objects.

### `QParadigmItem`

One row of the box, holding the part, the form's name and the text.
The row copies a ready slot, so the mark and the tip key were chosen below the driver.
Several slots sharing one stored form fold into one row naming every form, so `walked` is not listed twice.
The name is several joined by a comma when the row folds them.
The part shows only on the first row of its part, and only when more than one part inflects.
So a lone part does not repeat the chip above.
The text is the stored form or the mark standing in for a missing one.
The tip is null when the row shows its form.
The lectern and the editor draw alike, since the read already chose each row's mark and tip.

### `QParadigmSheet`

The inflection view of one entry holds two tables.
The collapsed table shows while the switch is unchecked.
The expanded one shows while it is checked.
A null view yields no sheet.
The lectern and the editor both map through it.

### `QParadigmTable`

One table, with header keys over the form columns and its lines in drawn order.
Headers are empty when there is no header row.

### `QParadigmLine`

One line of a table, with a group key, a label key and one form per column.
The group key is empty when the line opens no group.

### `QParadigmForm`

One cell, ready to draw.
Conduct already chose the pieces and the tip key.
The tip key is null for a plain cell.

### `QParadigmPiece`

One run of a form's text, painted as one inline.
It is marked when it departs from the prediction.
It is cut when a hyphen follows it, where the ending starts.

## Stamp

Commit: `c2f335c331fe`
- `QParadigm.cs` `e8f336bfd747b6b5`
- `QParadigmForm.cs` `a56b5f254ebf3c42`
- `QParadigmItem.cs` `d502b1646f6c45e3`
- `QParadigmLine.cs` `75464daa85fdc979`
- `QParadigmPiece.cs` `3f316f249c1582e2`
- `QParadigmSheet.cs` `d8b4e9a8721103fa`
- `QParadigmTable.cs` `3dfbc81903cdf33d`
- `QReflex.cs` `fced449836fc5563`
- `QReflexCommand.cs` `d8942c5c953b1bcb`
- `QReflexFrame.cs` `851450c503d34bce`
