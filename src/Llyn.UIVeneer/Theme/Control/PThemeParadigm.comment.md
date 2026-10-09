# PThemeParadigm.xaml
Hash: `e595775c0aa5ba4f`
Hash: `82e28a6350ce0db9`

## `<Style x:Key="Theme.Paradigm.Box" TargetType="Border">`

The frame the paradigm rows sit in, above the first meaning.
The inflection switch and both inflection tables sit in the same frame below the rows.
It draws no ground, line or corner, so the table reads as open text ruled by its own lines.
The border stays in the tree, so the control keeps one shape whether or not a plate is drawn.
The box hugs its rows rather than stretching, because the rows are short.

## `<Style x:Key="Theme.Paradigm.Part" TargetType="TextBlock">`

The part of speech heading a group of rows, in the accent like the part chips above.
Empty headings keep their text block because this style defines no visibility trigger.
It pins the interface font, since the box itself carries the headword font for the forms.

## `<Style x:Key="Theme.Paradigm.Name" TargetType="TextBlock">`

The name of the form, muted so the form itself stays the mark.
It pins the interface font for the same reason the part does.
An inflection table uses `Theme.Paradigm.Label` instead, which keeps this look with table spacing.

## `<Style x:Key="Theme.Paradigm.Group" TargetType="TextBlock">`

The group name of an inflection table, such as a mood, beside its group's first line.
It is italic ink rather than accent, so the group reads as a heading and not as a link.
The wide right margin keeps the group column apart from the labels.
It pins the interface font, since the box carries the headword font for the forms.

## `<Style x:Key="Theme.Paradigm.Label" TargetType="TextBlock" BasedOn="{StaticResource Theme.Paradigm.Name}">`

The line label of an inflection table, muted like a form name in the list rows.
Its vertical margin matches the forms, so every cell of a line shares one band.

## `<Style x:Key="Theme.Paradigm.Text" TargetType="TextBlock">`

The form, inked and in the headword font the box inherits from `QFontRefine`.
The size is fixed here, so the headword's own size does not carry into the rows.
The template carries no bindings, so `QParadigm` writes each row's text and tip.
A form that is not text draws a placeholder mark in the muted ink.
Its tooltip names the lookup state the mark stands for.

## `<Style x:Key="Theme.Paradigm.Form" TargetType="TextBlock" BasedOn="{StaticResource Theme.Paradigm.Text}">`

One form in an inflection table, inked like a listed form and a step larger.
The vertical margin leaves room above each rule, and the right margin keeps the columns apart.

## `<Style x:Key="Theme.Paradigm.Header" TargetType="TextBlock" BasedOn="{StaticResource Theme.Paradigm.Name}">`

A column header of an inflection table, muted like a form name.
The deeper bottom margin sets the header row off from the closing rule beneath it.

## `<Style x:Key="Theme.Paradigm.Rule" TargetType="Rectangle">`

The dotted line under a table line that another line of the same group follows.
It stays faint so the lines of a group read as one block.
It never takes the mouse, so the cells beneath keep their tooltips.

## `<Style x:Key="Theme.Paradigm.Close" TargetType="Rectangle">`

The solid line under the header row and under the last line of a group.
It closes a block, so the eye finds where each group ends without a plate.
It never takes the mouse, so the cells beneath keep their tooltips.

## `<Style x:Key="Theme.Paradigm.Fold" TargetType="RadioButton">`

One text button of the switch between the short and the full table.
It has no frame, so the pair reads as two words.
Both start muted, and the state sheet gives hover its ink and checked its accent, since markup never branches.
The hand cursor and the hover ink tell the user a word can be clicked.
The default focus visual stays, so a keyboard user still sees which button holds focus.

## `<Style x:Key="Theme.Paradigm.Marked" TargetType="Run">`

The part of a form holding letters the rules did not predict.
It uses Theme.Situation, so an irregular part reads as notable rather than as an error.

## `<Style x:Key="Theme.Paradigm.Cut" TargetType="Run">`

The faint hyphen between the root and the ending of a divided form.
It uses Theme.Muted, so the cut guides the eye without reading as a letter.

## `<DataTemplate x:Key="Theme.Paradigm.Row">`

Three columns separate the part, name and form.
The part and name columns share their widths across rows, so the forms line up.
Each text block is named, so Deportment finds it in a realized row.
