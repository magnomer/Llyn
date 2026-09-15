# PThemeReflex.xaml

The look of a reflex row, shared by the editor and the reading view.
The pronunciation and accent styles are merged, so a row sits in the reading stack as a pronunciation row does.
A row has no border and no box.
The language leads, the kind stands in the next column, the reading after it and the note last.
The first three columns are shared across every row, so the rows read as one table under the headword.
A language whose readings have no kind leaves that cell blank, so its readings still line up with the rest.
The language prints once on the first row of its run.

## `<Style x:Key="Theme.Reflex.Surface" TargetType="Border">`

The row surface, a little lower than a pronunciation row so a language's readings read as one block.

## `<Style x:Key="Theme.Reflex.Label" TargetType="TextBlock">`

The language of a lead row, drawn as the variety name of a pronunciation row is drawn.
Beneath the lead it is hidden rather than collapsed, so the shared column keeps its width.

## `<Style x:Key="Theme.Reflex.Text" TargetType="TextBlock">`

The reading of a row in the reading view, in the phonetic face every pronunciation row uses.
A row marked as the reading in common use is drawn in the accent colour, at the same weight.
The weight is left alone because the faces the scripts fall back to carry no heavier one.

## `<Style x:Key="Theme.Reflex.Tag" TargetType="TextBlock">`

The kind before the reading, small and muted, such as Go-on.
It is hidden rather than collapsed when the row has none, so the shared column keeps its width.

## `<Style x:Key="Theme.Reflex.Side" TargetType="TextBlock">`

The note after the reading, drawn as the kind is: the pinyin or the Korean 훈.
It is hidden rather than collapsed when the row has none, so it never pulls the row.

## `<Style x:Key="Theme.Reflex.Field" TargetType="TextBox">`

The bare field editing a row's reading, standing where the reading view prints it, accented when marked.

## `<Style x:Key="Theme.Reflex.Measure" TargetType="TextBlock">`

The unseen twin of a row's reading field.
It is sized by the row's text, or by the placeholder when the text is blank.

## `<Style x:Key="Theme.Reflex.Name" TargetType="TextBox">`

The bare field editing a row's language.
It takes the label size and colour, so it reads as the label does.
It overhangs its unseen twin by the two pixels a field pads its text with.
So the column is as wide as the view's.

## `<Style x:Key="Theme.Reflex.Gauge" TargetType="TextBlock">`

The unseen twin of the language field, sizing it by its text.

## `<Style x:Key="Theme.Reflex.Note" TargetType="TextBox">`

The bare field editing a row's kind or note, in their size and colour.
It overhangs its unseen twin as the language field does, for the same reason.

## `<Style x:Key="Theme.Reflex.Scale" TargetType="TextBlock">`

The unseen twin of the kind or note field, sizing it by its text.

## `<Style x:Key="Theme.Reflex.Star" TargetType="Button">`

The hover control marking a row as the reading in common use, lit in the accent when it is.

## `<Style x:Key="Theme.Reflex.Addition" TargetType="Button">`

The hover plus adding a row after this one in the same language.

## `<Style x:Key="Theme.Reflex.Remove" TargetType="Button">`

The hover minus dropping this row.

## `<Style x:Key="Theme.Reflex.Rebuild" TargetType="Button">`

The button at the top right corner of the editor's reflex block that drops the rows and fetches them again.
It is drawn as the icon commands of a meaning card are: ink at rest, lit only under the pointer.

## `<DataTemplate x:Key="Theme.Reflex.Row">`

One editor row: the language, kind, reading and note fields, and the hover star, plus and minus.
Every field is bare and stands where the reading view prints its text.

## `<DataTemplate x:Key="Theme.Reflex.Display">`

One reading-view row: the language on a lead row, the kind, the reading and its note.
