# TAuditName.cs

## `public sealed class TAuditName`

Runs the name audit over every source the naming scope enumerates.
The scope comes from `TAuditNameSetting`, so the file set follows the naming configuration alone.

## `public void AuditName_AllSourceNames_ReportsNoViolation()`

Loads the registry, enumerates the sources, and hands both to `TAuditNameWalker`.
An empty source list fails, since the audit would otherwise pass vacuously.
Every violation is listed with its file, line, kind, name, and reason.

## `public void AuditName_Exempt_MatchesSource()`

Every registry exemption still spares a name in the sources, so a stale row is removed from the registry.

## `private static string TViolationFormat(string repoRoot, IReadOnlyList<TViolation> violations)`

Sorts the violations by file and line and renders one line per violation.
