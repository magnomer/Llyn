# TFrequency.cs

## `public sealed class TFrequency`

Covers the stored form of one frequency value, `<source>|<raw>`, and its parse.
A formatted value parses back to the same source and raw, split on the first bar only.
A raw that carries a bar of its own survives the round trip whole.
A stored text without a bar parses as an empty source and the whole text as raw.
The band is never stored, so parse always leaves it null for the engine to resolve.
