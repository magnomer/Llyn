# PThemeDiwei.xaml

## `<Style x:Key="Theme.Diwei.Empty" TargetType="TextBlock">`

The muted line under the headword saying no placement is stored, in the interface font.

## `<sys:Boolean x:Key="Theme.Diwei.Spoken">False</sys:Boolean>`

The parameter the IPA half sends the switch command.

## `<sys:Boolean x:Key="Theme.Diwei.Spelled">True</sys:Boolean>`

The parameter the respelling half sends the switch command.

## `<Style x:Key="Theme.Diwei.Switch" TargetType="Border">`

The pill at the right end of a division heading holding the IPA and respelling halves.
It shows only while the section says the switch is on, respelling being on for the language.

## `<Style x:Key="Theme.Diwei.Choice" TargetType="Button">`

One half of the switch: muted text raising the switch command, tinted by the two styles below.

## `<Style x:Key="Theme.Diwei.Ipa" TargetType="Button" BasedOn="{StaticResource Theme.Diwei.Choice}">`

The IPA half, lit in the accent tint while the section prints the IPA set.

## `<Style x:Key="Theme.Diwei.Respelling" TargetType="Button" BasedOn="{StaticResource Theme.Diwei.Choice}">`

The respelling half, lit in the accent tint while the section prints the respelling set.

## `<Style x:Key="Theme.Diwei.Language" TargetType="TextBlock">`

The language name opening a tally line, muted and semibold, sharing its column across the section.

## `<Style x:Key="Theme.Diwei.Kind" TargetType="TextBlock">`

The kind of a tally line after the language, such as `Go-on`, muted and small, sharing its column.

## `<Style x:Key="Theme.Diwei.MarkText" TargetType="TextBlock">`

The part text of a mark, semibold in the interface font so IPA and jamo both read.

## `<Style x:Key="Theme.Diwei.MarkNumber" TargetType="TextBlock">`

The character count raised after the part text, small and in the accent colour, as an exponent reads.

## `<Style x:Key="Theme.Diwei.Mark" TargetType="ToggleButton">`

One mark: the part text and its count side by side, no box, a gap before the next mark.
It underlines on hover and tints the part while its popup is open, since a click lists the characters.

## `<Style x:Key="Theme.Diwei.MarkPopup" TargetType="Popup">`

The popup under a mark, closing when the pointer clicks elsewhere.

## `<Style x:Key="Theme.Diwei.MarkList" TargetType="ItemsControl">`

The characters of a mark wrapping inside its popup, each a chip opening its entry.

## `<DataTemplate x:Key="Theme.Diwei.MarkChip">`

One mark: the toggle drawing the part text and its raised count, and the popup it opens listing the characters.
The popup places itself under the toggle and its chips raise the same entry command as the plate's.

## `<DataTemplate x:Key="Theme.Diwei.TallyLine">`

One tally line across shared columns: the language name, the kind, then its chips wrapping.

## `<Style x:Key="Theme.Diwei.Tally" TargetType="ItemsControl">`

The tally lines of a section between its heading and its plate, collapsed while the section has none.

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

One division: its heading with the switch at its right end, its tally lines and the plate of rows.
The rows share column widths within the section.
