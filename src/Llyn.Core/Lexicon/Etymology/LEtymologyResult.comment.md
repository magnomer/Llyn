# LEtymologyResult.cs
Hash: `ca0d5ab7dc7d79d7`

## `public sealed record LEtymologyResult(IReadOnlyList<LTranslationTarget> LEtymologyResultTargets, bool LEtymologyResultNarrated)`

The engine's answer for an entry's etymology as the reading view shows it.
Named fields keep the narrated and linked verdicts from trading places.

**Parameters**

- `LEtymologyResultTargets` — The source links whose entries still stand, named and in the draft's order.
- `LEtymologyResultNarrated` — Whether the narrative holds words, by the etymology draft's own rule.

## `public bool LEtymologyResultLinked`

Whether any source link still stands, which shows the row of links.

## `public bool LEtymologyResultFilled`

Whether the etymology has anything to show: words in the narrative or a standing link.
