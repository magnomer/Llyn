# TAuditRingSetting.cs

## `internal static class TAuditRingSetting`

Hand-written and tracked: the project edge table lives here.
No script writes this file.

## `public static readonly IReadOnlyDictionary<string, string[]> TAuditRingEdges`

Every `ProjectReference` under `src`, as project name to referenced project names.
The fact holds the `.csproj` files to this table exactly, so a new edge is an edit here first.
An edge is wider than a reach.
The engine references the application alone and carries core records through it.

## `public static readonly IReadOnlyDictionary<string, int> TAuditRingCeiling`

The count each ring kind may reach.
`Transitive` counts the cut projects that still compile against rings past their neighbour.
A count above fails the fact, a ceiling above the count is stale and fails too.
