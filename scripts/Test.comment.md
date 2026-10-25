# Test.ps1

## Why this copy differs

The header says the script is identical in every project.
Every project carries the same copy.
The generation stays, since no test is selected or run differently.

Full runs ended with "being used by another process" on the test output file.
They printed no failure summary and no exit code, and they left their `Test-<guid>` folders in the temp folder.
Some process still held the output file open for writing after its project finished.
That process was never identified.
The runs use `--no-build`, so a build server is unlikely.

## Read-TestOutput

`File.ReadAllLines` lets no other handle write, so a live writer made it throw and end the script.
`Read-TestOutput` opens the file with share mode `ReadWrite`, so the read succeeds while the writer stays.
No retry is needed.

## Output file names

Each run writes `<project>.<run>.out` and `<project>.<run>.err`.
A later `-Repeat` run never reopens a file that a leftover process of an earlier run still holds.

## Cleanup

Removing the output folder is tried five times, 500 ms apart.
A folder that stays is named in a notice instead of being left behind in silence.
The script's exit code does not change.

## Time limit

One hung test stalled the run forever, since the poll loop had no limit.
Each run is now timed from its own start, by `$runWarningMinutes` (10) and `$runStopMinutes` (15) at the top of the script.
At the warning, one line names every project still running.
At the stop, each such project is killed with its whole process tree, its output so far is printed, and it is named as timed out.
A project that exits between the last poll and its kill is shown as finished, with its own exit code, never as timed out.
A timed-out project has no exit code of its own, so it gives 1 unless an earlier project in discovery order failed; no later run starts.
The limit is a constant, not a parameter.
The generation stays, since no test is selected or run differently.

## Process tree

`dotnet test` starts vstest and `testhost` as children.
Killing `dotnet` alone left `testhost` running and holding files.
Windows PowerShell 5.1 has neither `Kill($true)` nor `Process.Parent`.
`Stop-ProcessTree` lists every descendant first, from `Win32_Process` on Windows and from `ps` elsewhere, then kills the root and each descendant and waits up to 5 s for each.
On Windows a child started before the process that now holds its parent's id is not counted as a child.
On Windows a listed id is killed only while its process start time equals the listed one, to the microsecond, so an id that passed to an unrelated process after the listing is passed over; `ps` lists no start time, so elsewhere no such check is made.
The `finally` block uses it too, so a run broken off by an error or Ctrl+C leaves no `testhost` behind.

## Repeat

A failing run still finishes every project, and no later run starts.
The help text said execution stops at the first failure, and now says what the loop does.
