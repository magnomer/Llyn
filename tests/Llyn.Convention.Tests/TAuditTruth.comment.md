# TAuditTruth.cs

## `public sealed class TAuditTruth`

Keeps ground truth out of the shell.
A shell field may shape the UI or mirror what the engine holds.
It may not carry a value the engine has to be told.
The walk runs once and every fact reads the same hits.
While `TAuditTruthEnforced` is false every fact passes and reports as a warning.

## `private static readonly Lazy<string> TAuditTruthWritten = new(TAuditReportSave);`

The report is written once, on the first fact that runs.

## `public TAuditTruth(ITestOutputHelper output)`

Keeps the runner's output so a passing fact can still print its count.

## `public void AuditTruth_ShellFields_ReachNoRequest()`

A mutable shell field that reaches an engine call, as an argument or as the condition around one.
The engine needed the value, so the engine did not hold it.

## `public void AuditTruth_ShellFields_KeepOneWriter()`

A shell field written from an engine result in one place and from the shell in another.
Two writers make a copy that drifts from what the engine holds.

## `public void AuditTruth_ShellSources_MutateNoLogic()`

A shell line that assigns a logic member instead of sending a request.

## `public void AuditTruth_Waiver_MatchesAHit()`

Every waiver must still match a hit, else it is stale.

## `private void TAuditTruthCheck(IReadOnlyList<string> kinds, string summary)`

Filters the hits to the given kinds, drops the waived ones, and prints the count and the report path.
The rest fails the fact only when enforced.

## `private static string TAuditReportSave()`

Writes the markdown report under `temp/audit` and returns its repo-relative path.
The report opens with the counts, then the fields by hit count, then every hit by kind.
A waived hit is still listed and marked.

## `private static string TAuditWaiverFormat(TViolation hit)`

The waiver key of a hit: file name, field name and kind.

## `private static IReadOnlyList<TViolation> TAuditTruthRead()`

Enumerates the shell sources with Git and runs the walker once, with paths made repo-relative.
