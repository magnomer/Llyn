# CLecternEtymology.cs
Hash: `0fc451bebb59080d`

## `public sealed record CLecternEtymology(string CLecternEtymologyText, IReadOnlyList<CTranslationTarget> CLecternEtymologyTargets, bool CLecternEtymologyShown, bool CLecternEtymologyNarrated, bool CLecternEtymologyLinked, bool CLecternEtymologyDerived)`

The etymology of the shown entry in the reading view, ready to show.

**Parameters**

- `CLecternEtymologyText`: the narrative as written.
- `CLecternEtymologyTargets`: the source links whose entries still stand, named and in order.
- `CLecternEtymologyShown`: whether the field has a narrative or a link to show.
- `CLecternEtymologyNarrated`: whether the narrative holds words, which shows its read face.
- `CLecternEtymologyLinked`: whether a source link still stands, which shows the row of links.
- `CLecternEtymologyDerived`: whether the entry has an etymology at all, which shows its section.
