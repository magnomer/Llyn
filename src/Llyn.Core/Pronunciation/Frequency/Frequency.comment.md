# Frequency

This folder holds the frequency value an entry carries per source and the ladder gauge that grades it.

## `LFrequency`

One frequency value an entry carries from one source.
The raw figure is the ground truth.
The band is a cached label recomputed from the raw figure on every read.
The pack says where the value lives and how its unit turns into a word interval.
The interval is graded on one ladder shared by every language, so a label means the same everywhere.
`LFrequencyBand` is null when the figure has no interval and no pack band matches.
`LFrequencyUnit` is stamped only when the raw figure is numeric.

## `LFrequencyBands`

The shared ladder has one rung per decade of the word interval.
Core is one occurrence within 10,000 words, Everyday within 100,000, Advanced within 1,000,000, and Rare beyond.
The decades follow the Zipf scale of van Heuven, Mandera, Keuleers and Brysbaert (2014).
The names are vocabulary tiers.
They are the core thousand, the everyday five thousand, the advanced thirty thousand, and the rest.
The ladder lives here so no pack can grade its own figures on a scale of its own.
A pack may declare pattern bands only when its source yields no figure an interval can be read from.
Even then the bands must carry these four names and rank the source's levels onto the same rungs.

## `LFrequencyRare`

The label past the last rung, fewer than one occurrence per million words.

## `LFrequencyScale`

The band names from rare to core.
Every reader of the ladder reads this one list.

## `LFrequencyRanks`

Rare followed by the ladder's names in reverse, built from `LFrequencyBands`.
So each band name is spelled once, and the ranks cannot drift from the ladder.

## `LFrequencyRank`

The band's place on the scale, from one for rare up to four for core.
No band, or a name off the ladder, ranks zero.

## `LFrequencySort`

The one order every frequency list is shown in.
Rows follow the pack's declared source order, so the gauge's first ranked row is the pack's choice.
Sources the pack no longer declares follow, by name in codepoint order.
The raw figure breaks a tie, so the result never depends on storage order.

## `LFrequencyBandResolve`

Grades a raw figure on the shared ladder through the interval the source's pack figures describe.
The first rung whose limit the interval does not exceed names the band.
An interval past every rung is rare.
A source with no once figures yields null, so a pack pattern may still label the figure.

## `LFrequencyOnceResolve`

Turns a raw figure into the word interval rounded to two significant figures, so a tooltip reads round.
An interval under one word is held at one.
A source with none of the figures yields null.

## `LFrequencyOnceRead`

The exact word interval the source's pack figures describe, before any rounding.
A total divides by the raw count or per-million figure.
A factor with a base scales the base raised to the raw class.
A factor alone scales the raw rank.
A source with none of the figures yields null.
An interval that is not a finite non-negative number also yields null.

## `LFrequencyGauge`

The frequency of one entry, gathered from all its sources into one answer.
The rules that rank and word the sources live here, beside the frequency record they read.
`LFrequencyGaugeBand` is the star count of the first ladder band in row order.
Core counts the ladder's limit, and rare counts one.
A band that is not a ladder name is passed over, so a later row can still name the band.
It is zero when no row carries a ladder band.
`LFrequencyGaugeSource` holds one line per source in arrival order.
Callers pass rows in `LFrequencySort` order, so the lines follow the pack.

## `LFrequencyGaugeUnknown`

The rank name of an entry no source ranks.

## `LFrequencyGaugeRanked`

Whether the band names a ladder entry, so the chip shows a star row.
A band below one or past the ladder's end is unranked, so the rank never reads outside the ladder.

## `LFrequencyGaugeRank`

The ladder name at the band, or the unknown name while the band names no ladder entry.
The ladder is `LFrequencyScale`, so the ranking and the chip read one list.

## `LFrequencyGaugeSpare`

The stars past the band in a full row.
The band is held within zero and the ladder's length before the subtraction.
The spare is then never below zero nor above the ladder, so the row stays as wide as the ladder.
A hostile band such as the lowest integer cannot overflow.

## `LFrequencyGaugeResolve`

Gathers the rows into one answer, or null when the entry has no frequency at all.
The `once` text is a format with one slot for the word interval.

## `LFrequencyGaugeFormat`

One line per row, each naming its source, so the band is never the only thing said.
A row with a word interval prints it through the `once` pattern as a round number.
A row without one prints its unit before the raw figure when the pack names one, as CantoDict's Level does.
A row with neither, such as a Longman list mark or an HSK level, prints its raw answer.

## Stamp

Commit: `c2f335c331fe`
- `LFrequency.cs` `0937426ee9ff34cc`
- `LFrequencyGauge.cs` `f2f497c4d7cf7476`
