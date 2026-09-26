# TAuditChainSetting.cs

## `internal static class TAuditChainSetting`

Hand-written and tracked: the ring chain, its surfaces, its floors, its ceilings and its waivers.
No script writes this file, and the chain fact reads no script configuration.
`TAuditParity` holds it to scripts/auditstructure.json.
The reach and the cut are tied to the ring table and the UI roots by facts, not by hand.

## `public const string TAuditChainHost = "Llyn.Host";`

The one project that names every project, so it is no ring of the chain.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditChainReach`

Every project under `src`, as its name to the one ring it may reach.
A ring reaches its neighbour alone and carries the data of every ring inside it.
The two adapters reach `Llyn.Core` as spokes, since the contracts they implement live there.
The core reaches nothing.

## `public static readonly string[] TAuditChainCut`

The UI rings above the cut.
A cut ring names only its neighbour, and nothing from below the cut crosses into it.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditChainSurface`

For a `ring>neighbour` pair, the neighbour types the ring may name at all.
Conduct names ShellEngine through the six `L*Port` interfaces, which ShellEngine declares, and four handles.
Every other engine type is a helper Conduct may not reach.
Both drivers name Conduct only through the display types.

## `public static readonly IReadOnlyDictionary<string, int> TAuditChainFloor`

The fewest source files a ring may hold.
The application ring emptied once when its records were mistaken for the core's.
Raise a floor when a plan lifts another clerk in, never lower it.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditChainStray`

Patterns a ring may not declare a type name under.
A request record belongs to the application, so one in the core is misplaced.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditChainBanned`

Words no bound source under a folder may name, matching `banned` in scripts/auditstructure.json.

## `public static readonly IReadOnlyDictionary<string, int> TAuditChainCeiling`

The name count each `kind:ring>target` pair may hold.
A count above fails the fact, a ceiling above the count is stale and fails too.
Lower a ceiling when a ring sheds a name, never raise one to admit a new one.
The `cross` pairs are the bulk, as the UI rings still name core data and engine types.

## `public static readonly string[] TAuditChainWaiver`

The `path:name` rows that break the chain today, one per break, each deleted by a later plan.
A path ending in `/*` waives a whole folder for one name.
Every row must still match a hit, so a fixed break deletes its row.
