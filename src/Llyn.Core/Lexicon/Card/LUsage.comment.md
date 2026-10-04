# LUsage.cs
Hash: `c1cc5edfa39740bb`

## `public sealed record LUsage(long LUsageId, LOwner LUsageOwner, long LUsageEntry, string LUsageHeadword, string LUsageLanguage, LStateValue LUsageTitle)`

One referring side of a shared object, named rather than counted.
A shared object is referenced by Meanings, Collocations or Examples, never by an Entry as a whole.
A count alone flattens that.
It says how many, not which side, and not under which Entry.
`LUsage` keeps the side intact so a panel can show where the shared object is used.
It is a read result rather than stored data, so nothing here is written back.
The Entry it carries is the way from the referring side to the panel holding it.

**Parameters**

- `LUsageId` — Id of the referring side itself, a Meaning, a Collocation or an Example.
- `LUsageOwner` — Whether the referring side is a Meaning, a Collocation or an Example.
- `LUsageEntry` — Id of the Entry that referring side belongs to.
- `LUsageHeadword` — Headword of that Entry, so the row reads without a second query.
- `LUsageLanguage` — Language of that Entry.
- `LUsageTitle` — What names the referring side, and what is known about it.
  A Meaning is named by its title, and by its definition when it carries no title.
  A Collocation is named by its title, and by its expression when it carries no title.

## `public string LUsageName { get; init; }`

The headword as a usage row names it, until the clerk tells twin headwords apart.
Two rows citing Entries that share a headword then read differently, so a reader can tell them apart.

## `public string LUsageEpithet { get; init; }`

The epithet of the cited Entry, filled by the engine when the setting asks for it, else empty.
