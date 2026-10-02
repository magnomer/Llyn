# TAuditBorderSetting.cs
Hash: `957dcb1f347e9342`

## `internal static class TAuditBorderSetting`

Hand-written and tracked: the ring neighbours, their offers, their ceilings and their exempt rows.
No script writes this file, and the border fact reads no script configuration.
`auditstructure.json` holds its own copy, kept in step by hand as scripts/principles.md asks.
The neighbours and the cut are tied to the charter and the UI roots by facts, not by hand.

## `public const string TAuditBorderHost = "Llyn.Host";`

The one project that names every project, so it is no ring of the border.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditBorderNeighbour`

Every ring project under `src`, Host excepted, as its name to the one ring it may name as neighbour.
A ring names its neighbour alone and ferries the data of every ring inside it.
The two adapters name `Llyn.Core` as spokes, since the contracts they implement live there.
The core names no neighbour.

## `public static readonly string[] TAuditBorderCut`

The UI rings above the cut.
A cut ring names only its neighbour, and nothing from below the cut undercuts into it.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditBorderOffer`

For a `ring>neighbour` pair, the neighbour types the ring may name at all.
Conduct names ShellEngine through the six `L*Port` interfaces, which ShellEngine declares, and ten other `L` types.
Every other engine type is a helper Conduct may not name.
Both drivers name Conduct only through the `L` display types and the `C` shapes sealed controllers hand over.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditUnsealingPrefix`

For a UI ring, the type prefixes it seals.
A public type of that ring whose name starts with one is a sealed controller.
Its public members may name no type from below the ring's neighbour.
Deportment seals `L`, the medium-free controllers that later sink into Conduct.

## `public static readonly IReadOnlyDictionary<string, int> TAuditBorderCeiling`

The name count each `Kind:Ring>Target` pair may hold.
A count above fails the fact, a ceiling above the count is stale and fails too.
Lower a ceiling when a ring sheds a name, never raise one to admit a new one.
Every `Undercutting` and `Unsealing` ceiling is zero.

## `public static readonly string[] TAuditBorderExempt`

The `path:name` rows that break the border today, one per break, each deleted by a later plan.
A path ending in `/*` exempts a whole folder for one name.
Every row must still match a hit, so a fixed break deletes its row.
