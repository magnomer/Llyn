# PThemeDiwei.xaml

## `<Style x:Key="Theme.Diwei.Empty" TargetType="TextBlock">`

The muted line under the headword saying no placement is stored, in the interface font.

## `<Style x:Key="Theme.Diwei.Switch" TargetType="Border">`

The pill at the right end of a section heading holding the IPA and respelling halves.
The section fill shows it only while the switch is on, respelling being on for the language.

## `<Style x:Key="Theme.Diwei.Choice" TargetType="Button">`

One half of the switch in muted text.
Its switch command, padding, ground and hover are `PLook` rows.

## `<Style x:Key="Theme.Diwei.Ipa" TargetType="Button" BasedOn="{StaticResource Theme.Diwei.Choice}">`

The IPA half, which the section fill lights in the accent tint while the section prints the IPA set.
A `PLook` row gives it `false` as the switch command's parameter.

## `<Style x:Key="Theme.Diwei.Respelling" TargetType="Button" BasedOn="{StaticResource Theme.Diwei.Choice}">`

The respelling half, which the section fill lights while the section prints the respelling set.
A `PLook` row gives it `true` as the switch command's parameter.

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
`PLook` rows underline it on hover and tint the part while checked, since a click lists the characters.

## `<Style x:Key="Theme.Diwei.MarkPopup" TargetType="Popup">`

The popup under a mark, closing when the pointer clicks elsewhere.

## `<Style x:Key="Theme.Diwei.MarkList" TargetType="ItemsControl">`

The characters of a mark wrapping inside its popup, each a chip opening its entry.

## `<DataTemplate x:Key="Theme.Diwei.CharacterChip">`

One character as a chip, shared by the plate rows and the mark popups.

## `<DataTemplate x:Key="Theme.Diwei.MarkChip">`

One mark: the toggle drawing the part text and its raised count, and the popup it opens listing the characters.
`PTallyMark.PTallyMarkApply` fills the toggle, opens the popup under it and lists the characters.

## `<DataTemplate x:Key="Theme.Diwei.TallyLine">`

One tally line across shared columns: the language name, the kind, then its chips wrapping.
`PTally.PTallyApply` fills its named parts.

## `<Style x:Key="Theme.Diwei.Tally" TargetType="ItemsControl">`

The tally lines of a section between its heading and its plate.
A `PLook` row collapses it while the section has none.

## `<Style x:Key="Theme.Diwei.Head" TargetType="TextBlock">`

A section heading, a division or a place, drawn as the reading view's section titles are.

## `<Style x:Key="Theme.Diwei.Plate" TargetType="Border">`

The bordered plate the rows of one section sit on, the same plate the fanqie box uses.

## `<Style x:Key="Theme.Diwei.Reading" TargetType="TextBlock">`

The reading column, the final or the onset, bold in the interface font.

## `<Style x:Key="Theme.Diwei.Label" TargetType="TextBlock">`

The label column, the rime or the initial, in the glyph font the page inherits.

## `<Style x:Key="Theme.Diwei.Rounded" TargetType="Border">`

The bare 合 mark of a 合口 row, on no ground so it never reads as a chip.
The line fill collapses it on an 開口 row.

## `<Style x:Key="Theme.Diwei.RoundedText" TargetType="TextBlock">`

The 合 word, red semibold in the warning ink, the same mark the fanqie block draws.

## `<Style x:Key="Theme.Diwei.Chip" TargetType="Button">`

One character as a link back to its entry, in accent ink, as the glyph chips are.
`PLook` rows give its entry command, parameter, text, ink and hover underline from the character it stands for.
The word takes the button's ink itself, since the default text style would paint it black.

## `<DataTemplate x:Key="Theme.Diwei.Line">`

One row across shared columns: reading, label, 合口 chip, then the characters wrapping.
`PDiweiLine.PDiweiLineApply` fills its named parts.

## `<DataTemplate x:Key="Theme.Diwei.Section">`

One section: its heading with the switch at its right end, its tally lines and the plate of rows.
The rows share column widths within the section.
`PDiweiItem.PDiweiItemApply` fills its named parts and attaches the tally and line fills.
