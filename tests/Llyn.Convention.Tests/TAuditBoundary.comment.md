# TAuditBoundary.cs

## `public sealed class TAuditBoundary`

Keeps the shell on its side of the state rule.
The shell sends what was written and shows what the engine holds.
It never turns text or a chosen id into a stored state itself.

## `public void AuditRun_ShellSources_BuildNoStateValue()`

Scans every `.cs` file under `src/Llyn.UIShell` for the calls that read text or an id into a state.
Any hit names the file and line, so the request that should carry the raw field instead is easy to find.
