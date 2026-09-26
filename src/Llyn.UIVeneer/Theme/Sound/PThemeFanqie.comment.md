# PThemeFanqie.xaml

## `<Style x:Key="Theme.Fanqie.Box" TargetType="Border">`

The bordered plate the fanqie blocks sit on, under the script box and above the first meaning.
It takes the plain surface and a thin line, the same plate the script box uses.

## `<Style x:Key="Theme.Fanqie.Character" TargetType="TextBlock">`

The character heading a group of blocks, inked and in the glyph font the box inherits from `LFontApply`.
A `PLook` row collapses it when empty, so a one-character headword leaves no column.

## `<Style x:Key="Theme.Fanqie.Chip" TargetType="Border">`

The soft accent chip carrying the book name, drawn like the script style chips.
The row fill hides it, not collapses it, on a block with no book, so the column keeps its width.

## `<Style x:Key="Theme.Fanqie.Book" TargetType="TextBlock">`

The book name inside the chip, in the Han font at interface size, not the headword glyph.

## `<Style x:Key="Theme.Fanqie.Origin" TargetType="Border">`

The outlined chip at the right of a block naming the site its lines came from.
It is lighter than the book chip.

## `<Style x:Key="Theme.Fanqie.Source" TargetType="TextBlock">`

The site name inside the origin chip, small and muted, in the interface font.

## `<Style x:Key="Theme.Fanqie.Part" TargetType="TextBlock">`

One column of a line, in the glyph font family at reading size rather than the headword size.

## `<Style x:Key="Theme.Fanqie.Link" TargetType="Button">`

The initial and rime columns as chip buttons, accent ink on the soft accent ground the book chip uses.
Each opens the rime table on its category, and each reads as the key that category is stored under.
The word takes the button's ink itself, since the default text style would paint it black.
The border is always drawn and only coloured under the mouse, so hovering moves nothing beside it.
It fades while pressed and collapses when the part is empty.
Its parameter, word, ink, hover, press and fold are `PLook` rows in `PLookSound.cs`.

## `<Style x:Key="Theme.Fanqie.Initial" TargetType="Button" BasedOn="{StaticResource Theme.Fanqie.Link}" />`

The initial link, whose initial command is a `PLook` row.

## `<Style x:Key="Theme.Fanqie.Rime" TargetType="Button" BasedOn="{StaticResource Theme.Fanqie.Link}" />`

The rime link, whose rime command is a `PLook` row.

## `<Style x:Key="Theme.Fanqie.Knot" TargetType="TextBlock" BasedOn="{StaticResource Theme.Fanqie.Part}">`

The 重紐 letter after the heading, such as `(重紐:X)`, small and muted like the 反切 column.
A `PLook` row collapses it on a rime that carries no letter, which is most of them.
It is read out of the link so the chip can carry the rime key alone.

## `<Style x:Key="Theme.Fanqie.Reading" TargetType="TextBlock" BasedOn="{StaticResource Theme.Fanqie.Part}">`

The reading column, bold in the interface font, collapsed by a `PLook` row when the line has no reading.

## `<Style x:Key="Theme.Fanqie.Medial" TargetType="Border">`

The 開合 column, a bare word on no ground, so it never reads as a link chip.
The line fill collapses it when the line carries no parts.

## `<Style x:Key="Theme.Fanqie.MedialText" TargetType="TextBlock">`

The word inside the medial column, muted by default, at the size the other parts use.
The line fill turns it red and semibold for 合.
Red is the warning ink, chosen because it is the one colour the link chips never wear.

## `<Style x:Key="Theme.Fanqie.Spelling" TargetType="TextBlock" BasedOn="{StaticResource Theme.Fanqie.Part}">`

The 反切 column, smaller and muted, since it is the source's evidence rather than the placement itself.

## `<Style x:Key="Theme.Fanqie.Text" TargetType="TextBlock" BasedOn="{StaticResource Theme.Fanqie.Part}">`

The placement as fetched, collapsed by a `PLook` row on a line that carries parts.

## `<Style x:Key="Theme.Fanqie.RepresentativeIcon" TargetType="local:PIconImage">`

The star inside the representative button, unseen by default.

## `<Style x:Key="Theme.Fanqie.RepresentativeOrder" TargetType="TextBlock">`

The order number after the star, in the representative ink, collapsed until the line is marked.

## `<Style x:Key="Theme.Fanqie.Representative" TargetType="Button">`

The per-line button marking one reconstruction as representative among its siblings.
It is independent per line, so more than one reconstruction can carry the mark.
The line fill sets its tag to `Marked`, or `Faded` for a marked secondary line, and its order as content.
`PLook` rows show the star and order by that tag, dim it on hover, and carry its command.

## `<DataTemplate x:Key="Theme.Fanqie.Line">`

One placement across the shared columns: representative, reading, initial, rime, heading, 重紐, 開合, division, tone, 反切.
The initial and the rime are links into the rime table, the rime heading sitting in the next column.
The columns share their widths across every block of the box, so the parts line up under each other.
`PFanqieLine.PFanqieRowApply` fills its named parts.

## `<Style x:Key="Theme.Fanqie.Head" TargetType="TextBlock">`

The box's name at the head of a folded box, small and muted in the interface font, beside the switch.

## `<Style x:Key="Theme.Fanqie.Loading" TargetType="TextBlock">`

The muted line under the blocks saying the books are being asked, in the interface font.
It is the only content of the box while nothing is stored yet.

## `<Style x:Key="Theme.Fanqie.Stem" TargetType="Grid">`

The series line above the books of a character, which the row fill collapses while there is no series.
It keeps a gap under itself, so the series stands apart from the books.

## `<Style x:Key="Theme.Fanqie.StemChip" TargetType="Border">`

The accent chip carrying the series label, sitting in the book chip's column.
It wears the book chip's colours, so the series and the books read as one family.

## `<Style x:Key="Theme.Fanqie.StemLabel" TargetType="TextBlock">`

The localized word for the series inside the chip, in the interface font.

## `<Style x:Key="Theme.Fanqie.StemKey" TargetType="Button">`

One series as a pressable chip, inked in the glyph font the box inherits from `LFontApply`.
Pressing it opens that series in the xiesheng panel, as the initial and rime links open a cell.
Its stem command and its series as content are `PLook` rows.

## `<DataTemplate x:Key="Theme.Fanqie.StemChipItem">`

Draws one series of the line as a chip, so a character under two series shows two.
It is drawn at the headword size, since it names the whole character rather than one line of it.

## `<DataTemplate x:Key="Theme.Fanqie.Row">`

One block: the series line above, then the character column, the book chip, the lines and the origin chip.
The columns share their widths across the box, the series line included.
The series chips stand over the reading column, clearing the star column the lines carry.
`PFanqieItem.PFanqieItemApply` fills its named parts and attaches the line fill.
