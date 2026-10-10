# PThemeParadigm.xaml
Hash: `61034931bc36ccb6`
Hash: `82e28a6350ce0db9`

## `<Style x:Key="Theme.Paradigm.Box" TargetType="Border">`

The frame the paradigm rows sit in, above the first meaning.
The inflection switch and both inflection tables sit in the same frame below the rows.
It draws a hairline outline with the corner and padding of the link cards.
So it reads as one of the cards on the page.
Its ground stays clear, so the table's own rules carry the structure and no tint competes with the chips.
It hugs its table rather than stretching, so the switch sits at the table's right edge.
A stretched box would park the switch under the contents popup at the page's far right.

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
It is accent and semibold, like the particle and dependence labels of a sentence.
So the row headings read as headings against the small muted labels.
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

## `<Style x:Key="Theme.Paradigm.Switch" TargetType="Border">`

The track of the switch between the short and the full table.
Its tinted ground and round corners make the two words read as one segmented control.
It sits at the table's right edge, and the bottom margin parts it from the table.

## `<Style x:Key="Theme.Paradigm.Fold" TargetType="RadioButton">`

One side of the switch, sitting on the `Theme.Paradigm.Switch` track.
Its template part `PSurface` is a clear thumb, so an unchecked side reads as a word on the track.
The state sheet gives the checked side the page ground, a faint outline and accent ink.
So the chosen side reads as a raised thumb, since markup never branches.
The hand cursor and the hover ink tell the user a word can be clicked.
The default focus visual stays, so a keyboard user still sees which button holds focus.

## `<Style x:Key="Theme.Paradigm.Marked" TargetType="Run">`

The part of a form holding letters the rules did not predict.
It uses Theme.Warning, so the letters outside the expected forms stand out in red.

## `<Style x:Key="Theme.Paradigm.Cut" TargetType="Run">`

The faint hyphen between the root and the ending of a divided form.
It uses Theme.Muted, so the cut guides the eye without reading as a letter.

## `<DataTemplate x:Key="Theme.Paradigm.Row">`

Three columns separate the part, name and form.
The part and name columns share their widths across rows, so the forms line up.
Each text block is named, so Deportment finds it in a realized row.
