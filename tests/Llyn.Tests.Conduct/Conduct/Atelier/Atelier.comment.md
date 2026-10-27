# Atelier

These tests cover atelier startup, workspace changes and navigation without opening a window.
Real workspaces exercise restoration alongside fake ports that isolate failures and notifications.

## `TAtelier`

The tests distinguish live volume from stored posture by reopening an atelier over the same engine.
Faulting posture storage checks notice deduplication separately from recording every fault.
Startup ordering protects view restoration from blank leftover drafts.
The about lines are checked against an injected key lookup.
So the test reads no localization.
It pins the product term and the three-part assembly version.

## `TAtelierVista`

Vista starts reach the engine by subject and order name.
A detached atelier reads and writes no stored layout, and the posture file stays byte-identical.
An attached atelier restores the stored layout and keeps later changes for a reopened atelier.

## `TAtelierWorkspace`

Workspace changes are tested against pointer writes, leave answers and view restoration.
Failure cases protect the current workspace from unsuccessful moves.
A folder leased by another engine is refused, so the old workspace stays and no pointer is written.
Reopening with another envoy checks that later status failures reach the latest recipient.

## `TNavigation`

Registered callbacks expose leave questions, landed records and navigation notifications without a window.
Refused jumps are retried to check that refusal preserves the voyage trail.
Specialized jumps check attached area callbacks separately from entry arrivals.

## `TVoyage`

Relay calls exercise the voyage without a workspace or window.
Undo and redo checks distinguish accepted arrivals from refused jumps.
The capacity test records fifty-one stations to verify that only the oldest station is lost.

## Stamp

Commit: `9377e657b395`
- `TAtelier.cs` `b2f9104f5936e9b2`
- `TAtelierVista.cs` `9172f7a6f3ee053c`
- `TAtelierWorkspace.cs` `f6002fe2ea103522`
- `TNavigation.cs` `c4ee3943ab861846`
- `TVoyage.cs` `75c8d50a4bd8b856`
