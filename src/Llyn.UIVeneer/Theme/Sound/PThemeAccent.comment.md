# PThemeAccent.xaml

The look of a further pronunciation row, shared by the editor and the reading view.
The pronunciation styles are merged, so a row reads exactly as the primary pronunciation does.

## `<Style x:Key="Theme.Accent.Tool" TargetType="Button">`

The small icon button every pronunciation row wears.
A `PLook` row draws the icon named in its tag, and gives the row as the command parameter.
It is the playback action's look, so the row's buttons match the play button beside them.
The icon is drawn by a template, since one shared element could stand in only one row at a time.

## `<Style x:Key="Theme.Accent.Playback" TargetType="Button">`

The play button of a row, whose command and icon are `PLook` rows.
The row fill hides it while the row has no audio.

## `<Style x:Key="Theme.Accent.Phonetician" TargetType="Button">`

The lookup button of a row, whose notation command and icon are `PLook` rows.

## `<Style x:Key="Theme.Accent.Downloader" TargetType="Button">`

The download button of a row, whose clip command and icon are `PLook` rows.

## `<Style x:Key="Theme.Accent.Prompt" TargetType="TextBlock">`

The reading of an editor row, which the row fill turns into the muted placeholder while blank.

## `<Style x:Key="Theme.Accent.Blank" TargetType="StackPanel">`

The room the editor's lookup, download and plus-minus buttons take after a row's play button.
The reading view fills it with the same buttons drawn hidden.
So the volume tray after the primary row stands at one x in both views.
Buttons rather than a width, because a width is a figure that drifts.

## `<Style x:Key="Theme.Accent.Control" TargetType="StackPanel">`

The plus and minus pair of a row, unseen and untouchable by default.
`PLook` rows on the row surface show the part named `PAccentShelf` on hover or focus.
The pair reads as the example rows' pair does, so adding and dropping look the same everywhere.

## `<Style x:Key="Theme.Accent.Handle" TargetType="Button">`

The bare small button the plus and the minus share.
A `PLook` row gives it the row as the command parameter.

## `<Style x:Key="Theme.Accent.Addition" TargetType="Button">`

The plus that adds a blank row after this one.
Its addition command and icon are `PLook` rows.
The plus is drawn by a template, since one shared element could stand in only one row at a time.

## `<Style x:Key="Theme.Accent.Remove" TargetType="Button">`

The minus that drops a row.
Its removal command and icon are `PLook` rows.
The minus is drawn by a template, since one shared element could stand in only one row at a time.

## `<DataTemplate x:Key="Theme.Accent.Slot">`

The plus and minus pair built into a row's slot on first hover or focus.
The row fill names this key in the slot's tag, so the markup holds no order string.
It is drawn at full opacity, since the slot itself is what the hover reveals.

## `<DataTemplate x:Key="Theme.Accent.Row">`

A row as the editor draws it: chip, bracketed field, play, lookup, download, and the plus and minus pair.
`LAccentItem.LAccentItemApply` fills every named part from the row.
The row fill tags the reading cell with the field that grows over it while edited.
The chip sits in the shared lead column, so the bracket starts where every other row's reading starts.

## `<DataTemplate x:Key="Theme.Accent.Display">`

A row as the reading view draws it: chip, bracketed text and play, nothing to type into.
Its chip sits in the shared lead column too.
`LAccentItem.LAccentItemApply` fills its named parts as well.
