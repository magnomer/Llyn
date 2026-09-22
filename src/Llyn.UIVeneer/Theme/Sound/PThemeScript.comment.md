# PThemeScript.xaml

## `<Style x:Key="Theme.Script.Box" TargetType="Border">`

The bordered plate the script rows sit on, above the first meaning.
It takes the plain surface and a thin line, lighter than the paradigm box so the black glyphs stand out.
It stretches across the view, unlike that box, so a long row of pictures wraps instead of running off.

## `<Style x:Key="Theme.Script.Character" TargetType="TextBlock">`

The character heading a group of rows, inked and in the glyph font the box inherits from `PFontApply`.
It collapses when empty, so a one-character headword leaves no column.

## `<Style x:Key="Theme.Script.Chip" TargetType="Border">`

The soft accent chip carrying the style name, drawn like the part of speech chips.

## `<Style x:Key="Theme.Script.Style" TargetType="TextBlock">`

The style name inside the chip, in the interface font so the glyph font does not carry into it.

## `<Style x:Key="Theme.Script.Shape" TargetType="Rectangle">`

The glyph itself: a rectangle of the theme's ink masked by the picture.
The stored picture is black on a clear ground, so as a mask it takes whatever ink the theme paints.
Its width and height come from the item, which keeps the original's aspect at the drawn height.

## `<Style x:Key="Theme.Script.Caption" TargetType="TextBlock">`

The caption under a picture, muted, centered and wrapped within a narrow width.
It sits right under the age when there is one, so the two read as one caption of two lines.
It collapses when empty.

## `<Style x:Key="Theme.Script.Epoch" TargetType="TextBlock">`

The age above the caption, drawn like it but semibold and in the interface font.
The font is named here because the box inherits the glyph font, which is for the source's words, not ours.
It collapses when the picture carries no age, or when no shipped language names the stored code.

## `<Style x:Key="Theme.Script.Gloss" TargetType="TextBlock">`

The gloss under a row's pictures, inked and wrapped.
It collapses when empty.

## `<Style x:Key="Theme.Script.Head" TargetType="TextBlock">`

The box's name at the head of a folded box, small and muted in the interface font, beside the switch.

## `<Style x:Key="Theme.Script.Loading" TargetType="TextBlock">`

The muted line under the rows saying the pictures are being fetched, in the interface font.
It is the only content of the box while nothing is stored yet.

## `<DataTemplate x:Key="Theme.Script.Picture">`

One picture with its age and caption beneath, spaced from the next.
The two lines sit in a panel of their own.
The gap under the picture then stands whichever line is shown.

## `<DataTemplate x:Key="Theme.Script.Row">`

One row: the character column, the style chip, and the pictures wrapping in the rest with the gloss below.
The first two columns share their widths across rows, so the chips line up.
