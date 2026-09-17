# TAuditBoundary.cs

## `public sealed class TAuditBoundary`

Keeps the shell on its side of the state rule.
The shell sends what was written and shows what the engine holds.
It never turns text or a chosen id into a stored state itself.

## `private static readonly string[] TAuditBoundaryState =`

The three state names a panel used to compare against, before the converter alone read them.

## `private static readonly string[] TAuditBoundaryConverter =`

The two files that may read a state apart, because showing a mark is their whole job.

## `private const string TAuditBoundaryTimer = "CancellationTokenSource";`

The type a panel used to hold its own debounce timer in, before the tenure owned the quiet.

## `public void AuditBoundary_ShellSources_BuildNoStateValue()`

Scans every `.cs` file under `src/Llyn.UIShell` for the calls that read text or an id into a state.
Any hit names the file and line.
The request that should carry the raw field instead is easy to find.

## `public void AuditBoundary_UIShell_ComparesNoState()`

Scans every `.cs` file under `src/Llyn.UIShell` but the two converters for a state name.
A panel that needs to know whether a value is unknown asks the converter rather than reading the state.

## `public void AuditBoundary_HoldSources_KeepNoTimer()`

Scans every `P*Hold.cs` and `PEditor*.cs` file for a timer of its own.
The tenure waits out the typing, so a second wait in the panel would write twice or out of order.

## `private static List<string> TAuditBoundaryScan(Func<string, bool> chosen, IReadOnlyList<string> forbidden)`

The one walk both facts share, over the shell sources the chooser admits.
Each hit is one line naming the file, the line and the pattern found.
