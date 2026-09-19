# PThemeFanqie.xaml

## `<Style x:Key="Theme.Fanqie.Box" TargetType="Border">`

The bordered plate the fanqie blocks sit on, under the script box and above the first meaning.
It takes the plain surface and a thin line, the same plate the script box uses.

## `<Style x:Key="Theme.Fanqie.Character" TargetType="TextBlock">`

The character heading a group of blocks, inked and in the glyph font the box inherits from `PFontApply`.
It collapses when empty, so a one-character headword leaves no column.

## `<Style x:Key="Theme.Fanqie.Chip" TargetType="Border">`

The soft accent chip carrying the book name, drawn like the script style chips.
It is hidden, not collapsed, on a block repeating the book above it, so the column keeps its width.

## `<Style x:Key="Theme.Fanqie.Book" TargetType="TextBlock">`

The book name inside the chip, in the interface font so the glyph font does not carry into it.

## `<Style x:Key="Theme.Fanqie.Origin" TargetType="Border">`

The outlined chip at the right of a block naming the site its lines came from.
It is lighter than the book chip.

## `<Style x:Key="Theme.Fanqie.Source" TargetType="TextBlock">`

The site name inside the origin chip, small and muted, in the interface font.

## `<Style x:Key="Theme.Fanqie.Part" TargetType="TextBlock">`

One column of a line, in the glyph font family at reading size rather than the headword size.

## `<Style x:Key="Theme.Fanqie.Link" TargetType="Button">`

The initial and rime columns as chip buttons, accent ink on the soft accent ground the book chip uses.
Each opens the rime table on its category.
The word takes the button's ink itself, since the default text style would paint it black.
The border is always drawn and only coloured under the mouse, so hovering moves nothing beside it.
It fades while pressed and collapses when the part is empty.

## `<Style x:Key="Theme.Fanqie.Heading" TargetType="TextBlock" BasedOn="{StaticResource Theme.Fanqie.Part}">`

The bracketed rime heading after the rime link, plain ink, so only the rime itself reads as the link.

## `<Style x:Key="Theme.Fanqie.Reading" TargetType="TextBlock" BasedOn="{StaticResource Theme.Fanqie.Part}">`

The reading column, bold in the interface font, collapsed when the line has no reading.

## `<Style x:Key="Theme.Fanqie.Medial" TargetType="Border">`

The 開合 column, a bare word on no ground, so it never reads as a link chip.
Collapsed when the line carries no parts.

## `<Style x:Key="Theme.Fanqie.MedialText" TargetType="TextBlock">`

The word inside the medial column, muted for 開 and red for 合, at the size the other parts use.
Red is the warning ink, chosen because it is the one colour the link chips never wear.

## `<Style x:Key="Theme.Fanqie.Spelling" TargetType="TextBlock" BasedOn="{StaticResource Theme.Fanqie.Part}">`

The 反切 column, smaller and muted, since it is the source's evidence rather than the placement itself.

## `<Style x:Key="Theme.Fanqie.Text" TargetType="TextBlock" BasedOn="{StaticResource Theme.Fanqie.Part}">`

The placement as fetched, shown only on a line that carries no parts.

## `<DataTemplate x:Key="Theme.Fanqie.Line">`

One placement across the shared columns: reading, initial, rime, 開合, division, tone, 反切.
The initial and the rime are links into the rime table, the rime heading sitting after its link.
The columns share their widths across every block of the box, so the parts line up under each other.

## `<Style x:Key="Theme.Fanqie.Head" TargetType="TextBlock">`

The box's name at the head of a folded box, small and muted in the interface font, beside the switch.

## `<Style x:Key="Theme.Fanqie.Loading" TargetType="TextBlock">`

The muted line under the blocks saying the books are being asked, in the interface font.
It is the only content of the box while nothing is stored yet.

## `<DataTemplate x:Key="Theme.Fanqie.Row">`

One block: the character column, the book chip, the list of lines and the origin chip at the right.
The columns share their widths across the box.

## `<Style x:Key="Theme.Fanqie.Rebuild" TargetType="Button" BasedOn="{StaticResource Theme.Input.Pill}">`

The regenerate button at the foot of the box in the editor: a small accent pill, left-aligned under the blocks.
