# PChoiceOrder.xaml
Hash: `3cc3fc84935eb03b`

## `<Grid>`

The ordering picker every browse panel shares, so its button and menu read the same in each.
The markup is unplaced, and the owning panel sets its grid cell.
Its parts keep fixed names, so one driver finds them in whichever panel holds the picker.

## `<ToggleButton x:Name="PChoiceOrderDropper" ...>`

The button carries no tooltip, because the driver sets it from the title the owner names.

## `<StackPanel x:Name="PChoiceOrderList" />`

The menu starts empty, because the driver builds one row per ordering the owner offers.

## Hooks

The markup carries no hook.
The Deportment driver `QChoiceOrder` sets the icon, the tooltip, the rows and the popup's anchor.
