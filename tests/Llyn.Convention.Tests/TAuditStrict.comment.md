# TAuditStrict.cs

## `public sealed class TAuditStrict`

The strict shell rules, ahead of the split into a veneer and a deportment.
A veneer class holds no state and never branches, it only shows and calls.
A deportment class may hold logic for the UI itself but never computes over logic values.
While `TAuditStrictEnforced` is false every fact passes and reports as a warning.

## `private static readonly Lazy<(IReadOnlyList<TViolation> TAuditHits, IReadOnlyList<string> TAuditVeneers)>`

The walk runs once and every fact reads the same hits and the same veneer list.

## `private static readonly Lazy<string> TAuditStrictWritten = new(TAuditReportSave);`

The report is written once, on the first fact that runs.

## `public TAuditStrict(ITestOutputHelper output)`

Keeps the runner's output so a passing fact can still print its count.

## `public void AuditStrict_VeneerFields_HoldNothing()`

A mutable field in a veneer class, or a mutable static anywhere.

## `public void AuditStrict_VeneerMembers_BranchNever()`

A veneer member containing a branch, a loop, an operator or a pattern.

## `public void AuditStrict_ShellSources_TreatNoData()`

A shell line that compares, computes or queries over a logic value.
A bare logic verdict deciding a branch is asking the engine and is not treatment.

## `private void TAuditStrictCheck(string kind, string summary)`

Prints the count and the report path, then asserts only when enforced.

## `private static (IReadOnlyList<TViolation> TAuditHits, IReadOnlyList<string> TAuditVeneers) TAuditStrictRead()`

Enumerates the shell sources with Git and runs the walker once, with paths made repo-relative.

## `private static string TAuditReportSave()`

Writes the markdown report under `temp/audit` and returns its repo-relative path.
The report opens with the counts, then a per-veneer table, then every hit by kind.

## `private static int TAuditClassRead(IReadOnlyList<TViolation> hits, string type, string kind)`

The hits of one kind whose name starts with the class.
