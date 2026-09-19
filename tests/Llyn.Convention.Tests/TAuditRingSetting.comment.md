# TAuditRingSetting.cs

## `internal static class TAuditRingSetting`

Hand-written and tracked: the ring table, the project edges, the waivers and the ceilings live here.
No script writes this file.

## `public static readonly IReadOnlyDictionary<string, int> TAuditRingCeiling`

The hit count each kind may reach.
`EngineField` counts the `LEngine _lEngine` fields in the veneer.
`EngineHelper` counts the tenure, vista and foray type names in the veneer.
`AdapterEngine` counts the archives, loaders and sessions the engine constructs itself.
A count above fails the fact, a ceiling above the count is stale and fails too.
Lower a ceiling when a ring sheds a hit, never raise one to admit a new one.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditRingEdges`

Every `ProjectReference` under `src`, as project name to referenced project names.
The fact holds the `.csproj` files to this table exactly, so a new edge is an edit here first.
The engine's edge to the infrastructure stands until plan 08 removes it.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditRingRoles`

Every project under `src`, as project name to the namespaces its sources may not name.
The core names no other project and neither the file system, reflection nor the network.
The application adds only threading to what the core may name.
The engine names the infrastructure through its waiver row until plan 08.
The adapters name no other adapter and nothing above them.
The deportment never names an adapter, and the veneer never names the infrastructure.

## `public static readonly string[] TAuditRingExempt`

The `path:namespace` rows that stand for good.
`App.xaml.cs` composes the application and so may name the infrastructure.

## `public static readonly string[] TAuditRingStream`

A file naming one of these types uses the file system, so its `System.IO` is a break.
A file that names only `StringReader` or `TextReader` under `System.IO` reads text and is not.

## `public static readonly string[] TAuditRingWaiver`

The `path:namespace` rows that break the rings today, one per break, each deleted by a later plan.
A path ending in `/*` waives a whole project for one namespace.
Every row must still match a source line, so a fixed break deletes its row.
