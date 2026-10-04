# CVita.cs
Hash: `955cf08101155649`

## `public sealed record CVita(string CVitaName, bool CVitaNamed, string CVitaWork, string CVitaTally, IReadOnlyList<CFellow> CVitaFellows, IReadOnlyList<CUsage> CVitaUsages)`

The sheet the guild shows for its chosen author.

**Parameters**

- `CVitaName`: the author's name, or the unnamed wording.
- `CVitaNamed`: whether the author has a name, so the view mutes the unnamed wording.
- `CVitaWork`: how many sources credit the author, worded.
- `CVitaTally`: how many examples cite those sources, worded.
- `CVitaFellows`: the authors who share a source with this one.
- `CVitaUsages`: the places that cite the author's sources.

## `public bool CVitaFellowShown`

Whether any fellow is listed, so the fellow section shows.

## `public bool CVitaUsageShown`

Whether any citing place is listed, so the citation section shows.
