# PThemeReflex.xaml

The look of a reflex row, shared by the editor and the reading view.
The pronunciation and accent styles are merged, so a row sits in the reading stack as a pronunciation row does.
A row has no border and no box.
The language leads, then the kind, reading, romanization, meaning, note and anchors.
The text columns are shared across every row, so the rows read as one table under the headword.
So the anchored reading of every row starts at one x, whatever the notes before it measure.
A language whose readings have no kind leaves that cell blank, so its readings still line up with the rest.
The language prints once on the first row of its run.
Hovering the language shows the region its readings come from.
A folded row hides at no height while the fold toggle under it is closed.
It is still measured, so the shared columns hold the same widths folded or open.
The editor row and the view row lay out alike, so a reading stands at one place in both modes.
The editor's own fields, hover controls and row template live in `PThemeReflexEditor.xaml`.

## `<Style x:Key="Theme.Reflex.Surface" TargetType="Border">`

The row surface, a little lower than a pronunciation row so a language's readings read as one block.
The row fill makes it unseen and flat while hidden under the fold, yet still measured for the shared columns.

## `<Style x:Key="Theme.Reflex.Label" TargetType="TextBlock">`

The language of a lead row, drawn as the variety name of a pronunciation row is drawn.
Beneath the lead a `PLook` row hides it rather than collapsing it, so the shared column keeps its width.
The row fill gives it the region of the row as its tooltip, when the row names one.

## `<Style x:Key="Theme.Reflex.Text" TargetType="TextBlock">`

The reading of a row in the reading view, in the phonetic face every pronunciation row uses.
It carries no side margin, so it stands where the editor's field stands.
The row fill draws a reading marked as the one in common use in the accent colour.
The weight is left alone because the faces the scripts fall back to carry no heavier one.

## `<Style x:Key="Theme.Reflex.Edge" TargetType="TextBlock">`

The slash before or after the reading of a phonemic language, collapsed by a `PLook` row when there is none.
Both the editor and the reading view draw it in the reading's colour, as part of the reading.

## `<Style x:Key="Theme.Reflex.Fold" TargetType="ToggleButton">`

The small muted toggle under the stack that opens and closes the folded languages.
It stands centred under the table, so both panels hold the table in a stack no wider than its rows.
It reads `More readings` with a chevron, which turns over and reads `Fewer readings` while open.
It lights in the accent under the pointer.
Its icon, turn, label change and hover are `PLook` rows in `PLookSound.cs`.

## `<Style x:Key="Theme.Reflex.Tag" TargetType="TextBlock">`

The kind before the reading, small and muted, such as Go-on.
It is hidden rather than collapsed when blank, so the shared column keeps its width.

## `<Style x:Key="Theme.Reflex.Minor" TargetType="TextBlock">`

The shared look of romanization, meaning, note and anchors after the reading.
Meaning and note wear it directly, and a `PLook` row hides it while blank.

## `<Style x:Key="Theme.Reflex.Romanization" TargetType="TextBlock">`

The romanization after the reading, drawn in the minor style.

## `<Style x:Key="Theme.Reflex.Anchor" TargetType="TextBlock">`

The anchored placements after the note, drawn as the romanization is.
A `PLook` row collapses it when the row is anchored to none.
Both modes draw it, so the placement stands at one place in the editor and the reading view.

## `<Style x:Key="Theme.Reflex.Loading" TargetType="TextBlock">`

The muted line that stands where the rows will be drawn while a fill runs.
Both the editor and the reading view show it, and hide it when the fill answers.

## `<DataTemplate x:Key="Theme.Reflex.Display">`

One reading-view row: the language on a lead row, the kind, reading, romanization, meaning, note and anchors.
The reading stands between the same slashes the editor row draws, in the same stack with the same inset.
`LReflexItem.LReflexItemApply` fills every named part from the row.
