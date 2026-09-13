# PThemeAccent.xaml

The look of a further pronunciation row, shared by the editor and the reading view.
The pronunciation styles are merged, so a row reads exactly as the primary pronunciation does.

## `<Style x:Key="Theme.Accent.Flag" TargetType="Image">`

The variety flag of a row, bound to the row and hidden while the row has none.

## `<Style x:Key="Theme.Accent.Label" TargetType="TextBlock">`

The variety name of a row, shown only while no flag stands for it.

## `<Style x:Key="Theme.Accent.Measure" TargetType="TextBlock">`

The unseen twin of a row's field, sized by the row's text or by the placeholder when it is blank.

## `<Style x:Key="Theme.Accent.Remove" TargetType="Button">`

The cross that drops a row, raising the removal command with the row as its parameter.
The cross is drawn by a template, since one shared element could stand in only one row at a time.

## `<DataTemplate x:Key="Theme.Accent.Row">`

A row as the editor draws it: chip, bracketed field, and the cross.
The field is bound to the row so typing reaches the row model, which the editor listens to.

## `<DataTemplate x:Key="Theme.Accent.Display">`

A row as the reading view draws it: chip and bracketed text, nothing to type into.
