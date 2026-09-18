# TAuditBoundary.cs

## `public sealed class TAuditBoundary`

Keeps the shell on its side of the state rule.
The shell sends what was written and shows what the engine holds.
It never turns text or a chosen id into a stored state itself.

## `private static readonly string[] TAuditBoundaryForbidden =`

The calls that read text or an id into a state, and a `using static` hiding a logic name.
Each is a regular expression, so a space before the parenthesis does not slip past.

## `private static readonly string[] TAuditBoundaryState =`

The three state names a panel used to compare against, before the converter alone read them.
Each is matched bare, so a qualified read, markup and a `using static` are caught alike.
A bare name followed by `:` or `=` is a declaration or a named argument and is not a read.

## `private static readonly string[] TAuditBoundaryConverter =`

The two files that may read a state apart, because showing a mark is their whole job.

## `private static readonly string[] TAuditBoundaryHidden =`

The constructs that keep code out of a syntax walk: a preprocessor branch, reflection, `dynamic`, inline markup code.
Also an enum parsed from text and a `using` alias, which each give a logic name a second spelling.
The walkers parse without symbols, so a branch would be skipped, and reflection names nothing the walk can follow.

## `private static readonly string[] TAuditBoundaryLoader =`

The three files that read embedded resources through the assembly, and so may name reflection.

## `private const string TAuditBoundaryReflection = @"\bSystem\.Reflection\b";`

The namespace that reaches a member by its name as text.

## `private const string TAuditBoundaryTimer = "CancellationTokenSource";`

The type a panel used to hold its own debounce timer in, before the tenure owned the quiet.

## `public void AuditBoundary_ShellSources_BuildNoStateValue()`

Scans every source under `src/Llyn.UIShell` for the calls that read text or an id into a state.
Any hit names the file and line.
The request that should carry the raw field instead is easy to find.

## `public void AuditBoundary_UIShell_ComparesNoState()`

Scans every source and markup under `src/Llyn.UIShell` but the two converters for a state name.
A panel that needs to know whether a value is unknown asks the converter rather than reading the state.

## `public void AuditBoundary_ShellSources_HideNothing()`

Scans every shell source and markup for a construct that hides code from the walkers.

## `public void AuditBoundary_PanelSources_ReflectNothing()`

Scans every shell source but the loaders for the reflection namespace.

## `public void AuditBoundary_Tracked_SkipNoSource()`

Lists every tracked file under `src` and `tests` that the audits would skip by segment, suffix or prefix.
A source named like a generated file would otherwise never be walked.

## `public void AuditBoundary_LogicSources_HoldNoPanel()`

Scans every source outside the shell for a `P` or `PS` type.
A panel declared in another project would sit outside every shell audit.

## `public void AuditBoundary_HoldSources_KeepNoTimer()`

Scans every `P*Hold.cs` and `PEditor*.cs` file for a timer of its own.
The tenure waits out the typing, so a second wait in the panel would write twice or out of order.

## `private static List<string> TAuditBoundaryScan(Func<string, bool> chosen, IReadOnlyList<string> forbidden)`

The one walk both facts share, over the shell sources the chooser admits.
Each hit is one line naming the file, the line and the pattern found.
The chooser sees the file name and the patterns are regular expressions.
