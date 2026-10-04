# LArticulation.cs
Hash: `37b8467d3b4304c6`

## `public sealed record LArticulation(IReadOnlyList<string> LArticulationHeaders, IReadOnlyList<string> LArticulationSides, IReadOnlyList<IReadOnlyList<IReadOnlyList<string>>> LArticulationCells)`

One IPA chart, the universal notation the input aid offers.
It is no language's fact, so it lives here and not in a language pack.
Rows are one axis of articulation and columns the other.

**Parameters**

- `LArticulationHeaders`: the column names, in chart order.
- `LArticulationSides`: the row names, in chart order.
- `LArticulationCells`: per row, per column, the symbols the cell holds.

## `public static LArticulation LArticulationConsonantRead()`

The pulmonic consonant chart.
Rows are manner of articulation and columns are place of articulation.
A cell holds the voiceless consonant and then the voiced one.

## `private static readonly string[] LArticulationLocation`

The place-of-articulation column names of the consonant chart, in chart order.

## `private static readonly string[] LArticulationManner`

The manner-of-articulation row names of the consonant chart, in chart order.

## `private static readonly string[,] LArticulationConsonant`

An empty cell is a position judged impossible or one the IPA gives no symbol for.
It is kept as a cell so that the rows and columns stay aligned with their headers.

## `public static LArticulation LArticulationVowelRead()`

The vowel chart.
Rows are height and columns are backness.
A paired cell holds the unrounded vowel and then the rounded one.

## `private static LArticulation LArticulationRead(string[] headers, string[] sides, string[,] cells)`

Splits each written cell into its symbols, so an empty cell holds none.
