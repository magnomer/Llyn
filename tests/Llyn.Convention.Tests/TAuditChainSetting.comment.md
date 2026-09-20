# TAuditChainSetting.cs

## `internal static class TAuditChainSetting`

Hand-written and tracked: the ring chain, its root, its surfaces, its floors, its ceilings and its waivers.
No script writes this file, and the chain fact reads no script configuration.
The values mirror `auditstructure.json`, kept by hand, so either tool alone holds the chain.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditChainReach`

Every project under `src`, as its name to the one ring it may reach.
A ring reaches its neighbour alone and carries the data of every ring inside it.
The two adapters reach `Llyn.Core` as spokes, since the ports live there.
The core reaches nothing.

## `public static readonly string[] TAuditChainRoot`

The composition root files, free of the chain.
`App.xaml.cs` builds the engine and `LRigFactory.cs` builds every adapter of a rig.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditChainSurface`

For a `ring>neighbour` pair, the neighbour types the ring may name at all.
The deportment names the engine only through the six ports and the four handles.
Every other engine type is a helper the deportment may not reach.

## `public static readonly IReadOnlyDictionary<string, int> TAuditChainFloor`

The fewest source files a ring may hold.
The application ring emptied once when its records were mistaken for the core's.
Raise a floor when a plan lifts another clerk in, never lower it.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditChainStray`

Patterns a ring may not declare a type name under.
A request record belongs to the application, so one in the core is misplaced.

## `public static readonly IReadOnlyDictionary<string, int> TAuditChainCeiling`

The file count each `kind:ring>target` pair may hold.
A count above fails the fact, a ceiling above the count is stale and fails too.
Lower a ceiling when a ring sheds a file, never raise one to admit a new one.
The engine's reach into the core is the bulk: every vault it calls belongs to a clerk.

## `public static readonly string[] TAuditChainWaiver`

The `path:name` rows that break the chain today, one per break, each deleted by a later plan.
A path ending in `/*` waives a whole folder for one name.
Every row must still match a hit, so a fixed break deletes its row.
