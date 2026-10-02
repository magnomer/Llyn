# TAuditCharterSetting.cs
Hash: `9c22b209210e186b`

## `internal static class TAuditCharterSetting`

Hand-written and tracked.
The project edge table lives here.
No script writes this file.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditCharterEdges`

Every `ProjectReference` under `src`, as project name to referenced project names.
The fact holds the `.csproj` files to this table exactly, so a new edge is an edit here first.
An edge is wider than a neighbour.
The engine references the application alone and ferries core records through it.

## `public static readonly IReadOnlyDictionary<string, int> TAuditCharterCeiling`

The count each charter kind may hold.
`Piggybacking` counts the cut projects that still compile against rings past their neighbour.
A count above fails the fact, a ceiling above the count is stale and fails too.
