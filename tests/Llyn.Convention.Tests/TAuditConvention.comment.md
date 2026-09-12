# TAuditConvention.cs

## `public sealed class TAuditConvention`

Holds the generation this checkout of the convention tests applies.
A generation names the set of checks the audits apply, not a count of edits.
The name, line, and comment audits share one number, so a report from any of them compares across projects.

## `public const int TAuditGeneration`

The generation of this tooling.
Raise it only when a check is added, removed, or changed in what it reports.

## `public void AuditConvention_SidecarGenerations_MatchTheTooling()`

Reads the generation each generated sidecar carries and compares it with this one.
A sidecar written at another generation names the audit that must be rerun.
The check runs first because every other test compiles against those sidecars.
