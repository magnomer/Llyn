# QFrequencyLabel.cs
Hash: `8784d33ed0c70320`

## `internal static class QFrequencyLabel`

What a frequency chip shows, shared by the reading view and the editor.
One entry must read the same in both modes, so neither surface words or colours the chip itself.
It only paints, and [CFrequency](../../Llyn.Conduct/Sound/CFrequency.comment.md) carries the rank, its key and the spare stars.

## `private const string QFrequencyEmpty`

The brush suffix of the stars past the earned count.

## `private const char QFrequencyStar`

A four-pointed star, so the row cannot be mistaken for the five-pointed grasp stars.

## `internal static void QFrequencyChipRefine(UIElement section, FrameworkElement chip, TextBlock name, TextBlock band, CFrequency? frequency)`

Fills one whole frequency section with the chip's name, star row and source tooltip.
A null frequency collapses the section, so both surfaces hide it the same way.

## `private static void QFrequencyLabelRefine(TextBlock name, TextBlock band, CFrequency frequency)`

Words and colours one chip with the rung name in the rung's brush, then the star row.
The name looks up the key Conduct chose, so the chip speaks the user's language while the engine keeps English.
The rung's brush is the theme's resource under the rank's name.
The earned stars take the rung's brush and the spare ones the faint empty brush.
The engine counts the spare stars, so the row stays as wide as the ladder.
The spare stars are the same glyph dimmed, since a hollow glyph beside a filled one reads as five stars.
An unranked chip hides the star row and shows the name muted.
