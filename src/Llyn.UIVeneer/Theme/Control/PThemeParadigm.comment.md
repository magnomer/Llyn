# PThemeParadigm.xaml

## `<Style x:Key="Theme.Paradigm.Box" TargetType="Border">`

The bordered plate the paradigm rows sit on, above the first meaning.
It takes the raised ground and a thin line, so it reads as a table and not as a chip.
The box hugs its rows rather than stretching, because the rows are short.

## `<Style x:Key="Theme.Paradigm.Part" TargetType="TextBlock">`

The part of speech heading a group of rows, in the accent like the part chips above.
It collapses when empty, so a lone part or a row inside a group leaves no gap.
It pins the interface font, since the box itself carries the headword font for the forms.

## `<Style x:Key="Theme.Paradigm.Name" TargetType="TextBlock">`

The name of the form, muted so the form itself stays the mark.
It pins the interface font for the same reason the part does.

## `<Style x:Key="Theme.Paradigm.Text" TargetType="TextBlock">`

The form, inked and in the headword font the box inherits from `LFontApply`.
The size is fixed here, so the headword's own size does not carry into the rows.
The template carries no bindings, so `PParadigm` writes each row's text and marks.
An empty form draws an ellipsis and says the lookup is off.
A pending form draws the same ellipsis and says the form is being looked up.
An unknown form draws a dash and says no source listed it.
The unknown mark wins over the empty one.

## `<DataTemplate x:Key="Theme.Paradigm.Row">`

One row: part, name, and form in three columns.
The part and name columns share their widths across rows, so the forms line up.
Each text block is named, so Deportment finds it in a realized row.
