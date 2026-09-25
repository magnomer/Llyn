# LFrequencyLabel.cs

## `public static class LFrequencyLabel`

What a frequency chip shows, shared by the reading view and the editor.
One entry must read the same in both modes, so neither surface words or colours the chip itself.
It only draws, and [LDisplay](../../Llyn.Conduct/Display/LDisplay.comment.md) decides the rung and the tooltip.

## `private const int LFrequencyLimit`

The rung count of the ladder, so the star row stays four wide.

## `private const string LFrequencyEmpty`

The brush suffix of the stars past the earned count.

## `private const char LFrequencyStar`

A four-pointed star, so the row cannot be mistaken for the five-pointed grasp stars.

## `public static void LFrequencyChipShow(`

Fills one whole frequency section from the engine's rows: the chip's name, star row and source tooltip.
An empty row list collapses the section, so both surfaces hide it the same way.

## `private static void LFrequencyLabelShow(TextBlock name, TextBlock band, int count)`

Words and colours one chip: the rung name in the rung's brush, then the star row.
The name reads through the localization key, so the chip speaks the user's language while the engine keeps English.
The earned stars take the rung's brush and the rest the faint empty brush.
The empty stars are the same glyph dimmed, since a hollow glyph beside a filled one reads as five stars.
An unknown rung hides the star row and shows the name muted.
