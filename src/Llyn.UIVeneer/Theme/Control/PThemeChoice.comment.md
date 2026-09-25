# PThemeChoice.xaml

## `<Style x:Key="Theme.Choice.Row" TargetType="Button">`

One row the user picks from.
It is a whole row that takes the click, used by the entry index and by the ordering dropdown.
Keeping it templated avoids falling back to the platform's square, grey Button chrome.
This is not a picker-popup row.
Those carry no click of their own and offer their actions separately.
They are Theme.Popup.RowSurface.

## `<Setter Property="Background" Value="{StaticResource Theme.Surface}" />`

The rows sit on the panel's own ground, so they carry a card of their own.
The hover trigger below still repaints it.

## `<Style x:Key="Theme.Choice.Filter" TargetType="CheckBox">`

One language row of a filter dropdown, drawn like an ordering row with a box in place of the tick.
A shown language is boxed in accent and inked, a hidden one is bare and muted.
So a dropdown with every box ticked reads at a glance as hiding nothing.

## `<Style x:Key="Theme.Choice.Bare" TargetType="ComboBox">`

The frame fields of an Example are typed into like text.
They offer what the language has saved and are framed only under the pointer.
The frame is drawn outward, so a field being pointed at or written in never moves the sentence beside it.
The frame is closed onto the text it rings, clearing it by a hairline and no more.
A marker and a role are parted by one space, so a frame drawn outward crosses the word beside it.
