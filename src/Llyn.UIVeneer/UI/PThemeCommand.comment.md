# PThemeCommand.xaml

## `<Style x:Key="Theme.Command.Group" TargetType="Border">`

The tray a panel's commands sit in, and the same tray the input header's pair sits in.
It has no ground, no edge and no shadow of its own.
The commands therefore read as commands rather than as a boxed group.
Each command keeps its own hover and press, which is where a button says it is a button.
A disabled tray is dimmed whole, there being no edge left to pale.

## `<Style x:Key="Theme.Command.IconToggle" TargetType="ToggleButton">`

A command inside the tray that carries an icon alone and holds a menu open.
It is tinted while its menu stands open, so the tray says which picker is showing.

## `<ControlTemplate x:Key="Theme.Command.Segment.Template" TargetType="ButtonBase">`

One command inside the tray, drawn for a button and for a mode radio alike.
The icon is the button's Tag, masked into a rectangle that takes its colour from Foreground.
A command with no Tag collapses the icon and keeps its word alone.
The label is a TextBlock the template owns, because the app-wide TextBlock style outranks an inherited colour.
State is set on the button rather than on the surface, so a style below can overrule it.

## `<Style x:Key="Theme.Command.Primary" TargetType="Button">`

The one command in a tray that commits, filled so the eye finds it without reading.
It drops to the quiet treatment when it cannot be pressed, because a filled slab reads as pressable.

## `<Style x:Key="Theme.Command.Mode" TargetType="RadioButton">`

The two halves of the reading and writing switch, which is a choice rather than two commands.
The chosen half is tinted, so the panel says which half it is on before it is asked.
A disabled switch keeps that tint, because the mode it stands on is still true.
