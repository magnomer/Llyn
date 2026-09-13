# PThemeAccent.xaml

The look of a further pronunciation row, shared by the editor and the reading view.
The pronunciation styles are merged, so a row reads exactly as the primary pronunciation does.

## `<Style x:Key="Theme.Accent.Tool" TargetType="Button">`

The small icon button every pronunciation row wears, drawn from the SVG named in its tag.
It is the playback action's look, so the row's buttons match the play button beside them.
The icon is drawn by a template, since one shared element could stand in only one row at a time.

## `<Style x:Key="Theme.Accent.Playback" TargetType="Button">`

The play button of a row, raising the playback command and hidden while the row has no audio.

## `<Style x:Key="Theme.Accent.Transcriber" TargetType="Button">`

The lookup button of a row, raising the notation command so the menu opens for that row.

## `<Style x:Key="Theme.Accent.Downloader" TargetType="Button">`

The download button of a row, raising the clip command so the menu opens for that row.

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

A row as the editor draws it: chip, bracketed field, play, lookup, download, and the cross.
The field is bound to the row so typing reaches the row model, which the editor listens to.

## `<DataTemplate x:Key="Theme.Accent.Display">`

A row as the reading view draws it: chip, bracketed text and play, nothing to type into.
