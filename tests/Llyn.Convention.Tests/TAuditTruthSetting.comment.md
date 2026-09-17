# TAuditTruthSetting.cs

## `internal static class TAuditTruthSetting`

Hand-written and tracked: the truth-audit scope, the handle types and the waivers live here.
No script writes this file.

## `public const bool TAuditTruthEnforced = false;`

False makes every custody fact a warning that passes.
Flip it once the waivers are settled and the rest is fixed.

## `public const string TAuditTruthReport = "temp/audit/Custody-{0}.md";`

Where the report lands, with the version in the name.
`temp` is ignored by Git, so a run never dirties the tree.

## `public const string TAuditFieldPrefix = "_p";`

Only fields with this prefix are audited, so a framework backing field never enters.

## `public static readonly string[] TAuditTruthInclude`

The `git ls-files` patterns of the shell sources.

## `public static readonly string[] TAuditTruthHandles`

Types a shell field may hold as a handle to the engine rather than as a value.
A handle is called on and passed back, so the argument and guard rules skip it.
`PObserver` is the shell's subscription and is attached and detached by the engine.

## `public static readonly string[] TAuditTruthWaiver`

Each waiver is `File.cs:_pField:Kind`, and this file names why each one is waived.
A waiver matching no hit is reported as stale.
