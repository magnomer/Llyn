# TFrequency.cs
Hash: `9bf0396b5536deed`

## `public sealed class TFrequency`

Covers how a source spec turns a raw figure into a once interval and a band.
The interval is the number of words in which the word appears once, rounded to two figures.
A spec with a total divides the total by the raw count, and a per-million total divides the million.
A spec with a factor and a power raises the power to the raw class and scales it.
A spec with only a factor multiplies the raw rank by the factor.
The band grades the unrounded interval by decade into core, everyday, advanced and rare.
A spec with no figures, or a zero count against a total, yields no interval and no band.
