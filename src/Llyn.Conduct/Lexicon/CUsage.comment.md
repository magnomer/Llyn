# CUsage.cs

## `public sealed record CUsage(`

One place that cites what a vita shows, as its citation list reads it.

**Parameters**

- `CUsageId`: the citing side itself, an Example when the place quotes one.
- `CUsageEntry`: the entry the citing place belongs to.
- `CUsageName`: the headword that names the place.
- `CUsageEpithet`: the epithet printed after the headword.
- `CUsageLanguage`: the language of the citing entry.
- `CUsageTitle`: what names the citing side, uncertain when unknown.
- `CUsageQuoted`: whether the place quotes an Example rather than naming an Entry.
- `CUsageCollocated`: whether a Collocation, not a Meaning, holds the place.
