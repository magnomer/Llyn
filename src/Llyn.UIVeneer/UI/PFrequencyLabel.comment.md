# PFrequencyLabel.cs

## `internal static class PFrequencyLabel`

What a frequency chip shows, shared by the reading view and the editor.
One entry must read the same in both modes, so neither surface words or colours the chip itself.

## `private static readonly string[] PFrequencyBands`

The four ladder names from the lowest rung up, so a star count names its rung.

## `private const string PFrequencyUnknown`

The rung of an entry whose sources answered but earned no ladder name.

## `private const string PFrequencyEmpty`

The brush suffix of the stars past the earned count.

## `private const char PFrequencyStar`

A four-pointed star, so the row cannot be mistaken for the five-pointed grasp stars.

## `internal static int PFrequencyBandResolve(IReadOnlyList<LFrequency> rows)`

The star count of the first band any row carries in pack order, four for core, one for rare.
Zero when no row carries a band or the band is not a ladder name.

## `internal static string PFrequencyLabelResolve(int count)`

The localization key of the rung, so the chip reads in the user's language while the engine keeps English.

## `internal static void PFrequencyLabelShow(TextBlock name, TextBlock band, int count, string label)`

Words and colours one chip: the rung name in the rung's brush, then the star row.
The earned stars take the rung's brush and the rest the faint empty brush, so the row stays four wide.
The empty stars are the same glyph dimmed, since a hollow glyph beside a filled one reads as five stars.
An unknown rung hides the star row and shows the name muted.

## `internal static void PFrequencyChipShow(`

Fills one whole frequency section from the engine's rows: the chip's name, star row and source tooltip.
An empty row list collapses the section, so both surfaces hide it the same way.

## `internal static string PFrequencySourceFormat(IReadOnlyList<LFrequency> rows, string once)`

One line per row, each naming its source, so the band is never the only thing said.
A row with a word interval prints it through the `once` pattern as a round number.
The line then reads once every 1,300 words.
A row without one prints its unit before the raw figure when the pack names one, as CantoDict's Level does.
A row with neither, such as a Longman list mark or an HSK level, prints its raw answer.

## `private static string PFrequencyBandRead(int count)`

The ladder name at a star count, or the unknown name at zero.
