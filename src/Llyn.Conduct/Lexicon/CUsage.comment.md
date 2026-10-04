# CUsage.cs
Hash: `4618200229fd8db0`

## `public sealed record CUsage(long CUsageId, long CUsageEntry, string CUsageName, string CUsageEpithet, string CUsageLanguage, CStateValue CUsageTitle, bool CUsageQuoted, bool CUsageCollocated)`

One place that cites what a vita or the lectern shows, as their citation lists read it.

**Parameters**

- `CUsageId`: the citing side itself, an Example when the place quotes one.
- `CUsageEntry`: the entry the citing place belongs to.
- `CUsageName`: the headword that names the place.
- `CUsageEpithet`: the epithet printed after the headword.
- `CUsageLanguage`: the language of the citing entry.
- `CUsageTitle`: what names the citing side, uncertain when unknown.
- `CUsageQuoted`: whether the place quotes an Example rather than naming an Entry.
- `CUsageCollocated`: whether a Collocation, not a Meaning, holds the place.

## `public string CUsageOwnerKey`

The localization key naming the citing side as a Collocation, an Example or a Meaning.
Both drivers word the place by it, so neither picks the key.

## `public string? CUsageTitleKey`

The key of the unknown mark while the title is uncertain, and null while the title's own text stands.
