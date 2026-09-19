# PThemeLanguage.xaml

## `<Thickness x:Key="Theme.Language.Inset">`

The room between the pill's edge and its face, written once for the chip and the toggle.

## `<Style x:Key="Theme.Language.Chip" TargetType="Border">`

The blue pill an entry's language is named on.
The reading view wears it and the editor's language toggle copies it.
Both are defined here, so the two views cannot drift apart.
It carries the toggle's one-pixel border in its own fill, so the two pills measure the same.

## `<Style x:Key="Theme.Language.Face" TargetType="StackPanel">`

The row of flag, name and chevron a pill shows.
The chip and the toggle both fill it from these three styles, so their faces are one shape.

## `<Style x:Key="Theme.Language.Flag" TargetType="Grid">`

The cell the flag, or the globe standing in for it, is drawn in, with the gap before the name.

## `<Style x:Key="Theme.Language.Expand" TargetType="Path">`

The chevron that says the editor's pill opens a menu.
The reading view draws it hidden, so its pill keeps the chevron's room.
The heart and the stars after the pill then stand still when the mode switches.

## `<Style x:Key="Theme.Language.Toggle" TargetType="ToggleButton">`

The editor's language pill, drawn as the reading view's pill is drawn.
It stays a toggle, because a language is chosen here and only reported there.
The border is transparent until pointed at or opened, which is the only hint of the menu it holds.
