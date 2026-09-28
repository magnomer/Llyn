# CUsage.cs

## `public sealed record CUsage(`

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

The localization key naming the kind of the citing side: a Collocation, an Example or a Meaning.
Both drivers word the place by it, so neither picks the key.

## `public string? CUsageTitleKey`

The key of the unknown mark while the title is uncertain, and null while the title's own text stands.

## `public void CUsageOpen(Func<long, bool> exampleSeam, Func<long, bool> entrySeam)`

The gate for a citing place the user clicked, which opens the place it names.
A place that quotes an Example opens the Example, and any other place opens its Entry.
The two seams switch the tab, which only the surface does until `CNavigation` owns it.
