# LFrequencyGauge.cs

## `public sealed record LFrequencyGauge(int LFrequencyGaugeBand, string LFrequencyGaugeSource)`

The frequency of one entry, gathered from all its sources into one answer.
The rules that rank and word the sources live here, beside the frequency record they read.

**Parameters**

- `LFrequencyGaugeBand` — The star count of the first ladder band in pack order, the limit for core, one for rare.
  A band that is not a ladder name is passed over, so a later row can still name the band.
  It is zero when no row carries a ladder band.
- `LFrequencyGaugeSource` — Every source's figure, one line each, in the order the sources arrive.

## `private const string LFrequencyGaugeUnknown`

The rank name of an entry no source ranks.

## `public bool LFrequencyGaugeRanked`

Whether any source ranks the entry, so the chip shows a star row.

## `public string LFrequencyGaugeRank`

The ladder name at the band, or the unknown name while no source ranks the entry.
The ladder is `LFrequency.LFrequencyScale`, so the ranking and the chip read one list.

## `public int LFrequencyGaugeSpare`

The stars past the band in a full row, never below zero.
The row then stays as wide as the ladder.

## `public static LFrequencyGauge? LFrequencyGaugeResolve(IReadOnlyList<LFrequency> rows, string once)`

Gathers the rows into one answer, or null when the entry has no frequency at all.
The `once` text is a format with one slot for the word interval.

## `private static string LFrequencyGaugeFormat(IReadOnlyList<LFrequency> rows, string once)`

One line per row, each naming its source, so the band is never the only thing said.
A row with a word interval prints it through the `once` pattern as a round number.
The line then reads once every 1,300 words.
A row without one prints its unit before the raw figure when the pack names one, as CantoDict's Level does.
A row with neither, such as a Longman list mark or an HSK level, prints its raw answer.
