# PThemeDiwei.xaml

## `<Style x:Key="Theme.Diwei.Empty" TargetType="TextBlock">`

The muted line under the headword saying no placement is stored, in the interface font.

## `<Style x:Key="Theme.Diwei.Head" TargetType="TextBlock">`

A division heading, drawn as the reading view's section titles are.

## `<Style x:Key="Theme.Diwei.Plate" TargetType="Border">`

The bordered plate the rows of one division sit on, the same plate the fanqie box uses.

## `<Style x:Key="Theme.Diwei.Reading" TargetType="TextBlock">`

The final's reading column, bold in the interface font.

## `<Style x:Key="Theme.Diwei.Rime" TargetType="TextBlock">`

The rime column, in the glyph font the page inherits.

## `<Style x:Key="Theme.Diwei.Rounded" TargetType="Border">`

The soft accent chip marking a 合口 row, collapsed on an 開口 row.

## `<Style x:Key="Theme.Diwei.RoundedText" TargetType="TextBlock">`

The 合 inside the chip.

## `<Style x:Key="Theme.Diwei.Chip" TargetType="Button">`

One character as a link back to its entry: accent ink, underlined under the mouse, as the glyph chips are.
The word takes the button's ink itself, since the default text style would paint it black.

## `<DataTemplate x:Key="Theme.Diwei.Line">`

One row across shared columns: reading, rime, 合口 chip, then the characters wrapping.

## `<DataTemplate x:Key="Theme.Diwei.Section">`

One division: its heading and the plate of rows, sharing column widths within the section.
