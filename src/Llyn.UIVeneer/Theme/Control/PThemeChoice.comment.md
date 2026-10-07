# PThemeChoice.xaml
Hash: `168b1d5620f22041`

## `<Style x:Key="Theme.Choice.Row" TargetType="Button">`

One row the user picks from.
It is a whole row that takes the click, used by the entry index and by the ordering dropdown.
Keeping it templated avoids falling back to the platform's square, grey Button chrome.
This is not a picker-popup row.
Those carry no click of their own and offer their actions separately.
They are Theme.Popup.RowSurface.

## `<Setter Property="Background" Value="{StaticResource Theme.Surface}" />`

The rows sit on the panel's own ground, so they carry a card of their own.
The hover row in `QLook` still repaints it.

## `<Style x:Key="Theme.Choice.Order" TargetType="RadioButton">`

One ordering in a sort dropdown.
`QChoice` builds these radio buttons in code, so no markup names the style.
The tick stays collapsed until `QLook` shows it on the checked one.

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
The dropdown shows at once, with no fade, so the pick never looks slow to answer.

## `<Style x:Key="Theme.Choice.Switch" TargetType="ToggleButton">`

The on-off switch of the settings pages.
It carries no trigger, so `QLook` rows colour the track and slide the knob when checked.

## `<Thickness x:Key="Theme.Choice.Switch.KnobInset">0,0,3,0</Thickness>`

The knob's margin while a switch is on, so it rests against the right edge of the track.
