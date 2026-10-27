# Cadence

This folder holds the tests of the sound boxes under the editor header.
They cover fold switches, paradigm grid, reflex hinges and labels, and transcription schemes.

## `TFoldSwitch`

The open switches of the editor's rime-book and script boxes are covered.
Each click goes through `QCadence`, which holds the fold and hears both switches.
The reading view's switches are covered too, which `QLecternSound` hears over the display's own fold area.
A click is the switch's own state change followed by its click event, as a user's press is.
With no stored entry held each gate refuses the write.
So both switches go back to the state before the click, and the store holds nothing.
With a stored entry held only the clicked box is stored, and its switch stays open.
The reading view's boxes show their head with the switch.
Each switch shows the shown entry's stored state.

### `TFoldIconPrepare`

It points the icon root at the test assembly's own icons, so a box can build its switch chevron.
The override keeps switch icon lookup within the test assembly.

## `TParadigmGrid`

The inflection tables of the paradigm box, their rules and the switch between them are covered.
Assertions depend on fixed child indices for the switch track and both tables.
A box handed a sheet shows itself, the switch track and the collapsed table.
The expanded table stays hidden until the full button is checked.
Setting the check raises the checked event, so no click has to be faked.
A box whose sheet returns to null while it has no rows collapses again.
Where Core cuts the pieces is `TParadigmPiece`'s fact, so these cases pin only the painting.
A marked piece takes `Theme.Paradigm.Marked`, and a cut hyphen takes `Theme.Paradigm.Cut`.
The tests lend the application those styles and remove them once they end.
A table of three lines in two groups draws exactly two rules.
The line inside the first group gets an open rule that starts at the label column.
The last line of the first group gets a closed rule from the group column.
The last line of the table gets none, and no header row adds one.

### `TParadigmSheetCreate`

The sheet is mapped from a Conduct view by `QParadigmSheetCreate`, as the drivers map it.
The expanded line also carries a pending form with its tip key.
Every other form uses the given pieces, so one sheet serves the mark and split tests.

### `TParadigmPartsRead`

The box's stack holds four parts in a fixed order.
The second part is the switch track, whose panel holds the full button second.

## `TReflexHinge`

The two "More readings" hinges are covered, `PDisplayReflexHinge` in the reading view and `PReflexHinge` in the editor.
A click is the hinge's own state change followed by its click event, as a user's press is.
With nothing shown, or an empty desk, the gate refuses the write and the hinge goes back to closed.
A shown entry stored opened paints the renamed hinge checked from the stored state.
After repainting, the hinge and persisted opening both remain true.

## `TReflexLabel`

A reflex row's language label and kind tag are relabelled after a typed edit.
The labels look up application resources.
The two stored names get translations for the test and lose them once it ends.
Typing a new language and kind relabels the row under the keys of the typed names.
The typed names have no translation, so the row prints them as typed.
A row keeping the stored keys would still print the previous translations.

## `TTranscriptionScheme`

The scheme choices a transcription row offers in its combo box are covered.
No application runs, so each choice label falls back to its scheme name.
The same schemes arriving in a new order rebuild the choices in that order.
Each taken flag lands on its own scheme.
A count check alone kept the old order and handed each flag to the wrong scheme.
A later refresh with the same schemes keeps the choice objects and only moves the taken flags.
So a bound combo box keeps its items when nothing but the flags changed.

## Stamp

Commit: `9377e657b395`
- `TFoldSwitch.cs` `eac8e39c6f39589c`
- `TParadigmGrid.cs` `2bf441a588307d49`
- `TReflexHinge.cs` `6418639d792f799a`
- `TReflexLabel.cs` `0bd08074bf9fafc0`
- `TTranscriptionScheme.cs` `b3977faf30f7a61f`
