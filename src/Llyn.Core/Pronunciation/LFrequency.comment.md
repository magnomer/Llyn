# LFrequency.cs

## `public sealed record LFrequency(`

One frequency value an entry carries from one source.
The engine fetches it once from a web source the pack names and stores the raw figure and its band.
The raw figure is the ground truth and the band is a cached label recomputed from it on every read.
The pack knows where the value lives and how its unit turns into a word interval.
The interval is graded on one ladder shared by every language, so a label means the same everywhere.

**Parameters**

- `LFrequencySource` — The name of the pack source the raw figure came from.
- `LFrequencyRaw` — The figure exactly as the source returned it, such as a rank or a count.
- `LFrequencyBand` — The label the raw figure earns, graded by interval or matched by a pack [LBand](LBand.comment.md).
  It is `null` when the figure has no interval and no band matches.
- `LFrequencyOnce` — The estimated number of running words between two occurrences, or `null` when the source gives no basis.
- `LFrequencyUnit` — The pack's word for a bare numeric answer, stamped only when the raw figure is numeric, else `null`.

## `private static readonly (double LFrequencyLimit, string LFrequencyName)[] LFrequencyBands`

The shared ladder, one rung per decade of the word interval.
Core is one occurrence within 10,000 words, Everyday within 100,000, Advanced within 1,000,000, Rare beyond.
The decades follow the Zipf scale of van Heuven, Mandera, Keuleers and Brysbaert (2014).
Zipf 5 and above is at least 100 per million, Zipf 4 at least 10, Zipf 3 at least 1.
The names are vocabulary tiers: the core thousand, the everyday five thousand, the advanced thirty thousand, the rest.
The ladder lives here so no pack can grade its own figures on a scale of its own.
Every language observes these four rungs and these four names, whatever its source measures.
A pack may declare pattern bands only when its source yields no figure an interval can be read from.
Even then the bands must carry these four names and rank the source's levels onto the same rungs.

## `private const string LFrequencyRare`

The label past the last rung, fewer than one occurrence per million words.

## `public static string? LFrequencyBandResolve(LSourceSpec spec, double raw)`

Grades a raw figure on the shared ladder through the interval the source's pack figures describe.
The first rung whose limit the interval does not exceed names the band.
An interval past every rung is rare.
A source with no once figures yields `null`, so a pack pattern may still label the figure.

## `public static long? LFrequencyOnceResolve(LSourceSpec spec, double raw)`

Turns a raw figure into the word interval rounded to two significant figures, so a tooltip reads round.
An interval under one word is held at one.
A source with none of the figures yields `null`.

## `public static double? LFrequencyOnceRead(LSourceSpec spec, double raw)`

The exact word interval the source's pack figures describe, before any rounding.
A total divides by the raw count or per-million figure.
A factor with a base scales the base raised to the raw class.
A factor alone scales the raw rank.
A source with none of the figures, or an interval that is not a finite non-negative number, yields `null`.
