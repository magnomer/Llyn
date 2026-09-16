# PThemeReflex.xaml

The look of a reflex row, shared by the editor and the reading view.
The pronunciation and accent styles are merged, so a row sits in the reading stack as a pronunciation row does.
A row has no border and no box.
The language leads, then the kind, the reading, the note with its remark, and the anchors last.
The first four columns are shared across every row, so the rows read as one table under the headword.
So the anchored reading of every row starts at one x, whatever the notes before it measure.
A language whose readings have no kind leaves that cell blank, so its readings still line up with the rest.
The language prints once on the first row of its run.
Hovering the language shows the region its readings come from, and the remark prints after the note.
A folded row hides at no height while the fold toggle under it is closed.
It is still measured, so the shared columns hold the same widths folded or open.
The editor row and the view row lay out alike, so a reading stands at one place in both modes.
The editor's own fields, hover controls and row template live in `PThemeReflexEditor.xaml`.

## `<Style x:Key="Theme.Reflex.Surface" TargetType="Border">`

The row surface, a little lower than a pronunciation row so a language's readings read as one block.
While the row is hidden under the fold it is unseen and flat, yet still measured for the shared columns.

## `<Style x:Key="Theme.Reflex.Label" TargetType="TextBlock">`

The language of a lead row, drawn as the variety name of a pronunciation row is drawn.
Beneath the lead it is hidden rather than collapsed, so the shared column keeps its width.
Its tooltip is the region of the row, and no tooltip at all when the row names none.

## `<Style x:Key="Theme.Reflex.Text" TargetType="TextBlock">`

The reading of a row in the reading view, in the phonetic face every pronunciation row uses.
It carries no side margin, so it stands where the editor's field stands.
A row marked as the reading in common use is drawn in the accent colour, at the same weight.
The weight is left alone because the faces the scripts fall back to carry no heavier one.

## `<Style x:Key="Theme.Reflex.Edge" TargetType="TextBlock">`

The slash before or after the reading of a phonemic language, collapsed when there is none.
Both the editor and the reading view draw it in the reading's colour, as part of the reading.

## `<Style x:Key="Theme.Reflex.Fold" TargetType="ToggleButton">`

The small muted toggle under the stack that opens and closes the folded languages.
It stands centred under the table, so both panels hold the table in a stack no wider than its rows.
It reads `More readings` with a chevron, which turns over and reads `Fewer readings` while open.
It lights in the accent under the pointer.

## `<Style x:Key="Theme.Reflex.Tag" TargetType="TextBlock">`

The kind before the reading, small and muted, such as Go-on.
It is hidden rather than collapsed when the row has none, so the shared column keeps its width.

## `<Style x:Key="Theme.Reflex.Side" TargetType="TextBlock">`

The note after the reading, drawn as the kind is: the pinyin or the Korean 훈.
It is hidden rather than collapsed when the row has none, so it never pulls the row.

## `<Style x:Key="Theme.Reflex.Remark" TargetType="TextBlock">`

The remark after the note, drawn as the note is, such as literary or vernacular.

## `<Style x:Key="Theme.Reflex.Anchor" TargetType="TextBlock">`

The anchored placements after the remark, drawn as the note is, collapsed when the row is anchored to none.
Both modes draw it, so the placement stands at one place in the editor and the reading view.

## `<Style x:Key="Theme.Reflex.Loading" TargetType="TextBlock">`

The muted line that stands where the rows will be drawn while a fill runs.
Both the editor and the reading view show it, and hide it when the fill answers.

## `<DataTemplate x:Key="Theme.Reflex.Display">`

One reading-view row: the language on a lead row, the kind, the reading, its note, its remark and its anchors.
The reading stands between the same slashes the editor row draws, in the same stack with the same inset.
