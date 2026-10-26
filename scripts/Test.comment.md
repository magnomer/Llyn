# Test.ps1
Hash: `4dd4d6dac089918d`

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

## Default scope

Generation 6 leaves the convention project out of a plain run, a run given symbol names and a `-Platform` run.
`-All` adds it and `-Convention` runs it alone; `-Main` still works and names the default.
`Test.json` is raised to generation 6 to match.

## Folder switches

Every folder under the tests folder that holds a test project, at any depth, gives a switch built in the `dynamicparam` block, so the script names no folder.
The switch is the last dot-separated part of the folder name: `Infrastructure` gives `-Infrastructure`, `Llyn.Tests.Console` gives `-Console`.
Folders that share a name share the switch, and `bin` and `obj` folders are not walked.
A name a fixed parameter or alias already takes gives no switch, so `Llyn.Tests.Convention` is left to `-Convention`.
Several switches may be given, and only the projects inside their folders run.
They cannot be combined with `-All`, `-Convention`, `-Main`, `-Platform` or `-Project`.
`-Help` lists the switches the tree gives.
A `dynamicparam` block needs the body in a named block, so the body sits in `end`, left at the margin so the help text prints as it reads.
The generation stays 6: these switches are part of it.

## TestSrc and TestAudit

`TestSrc.ps1` runs `Test.ps1` with no scope option, the default: every test project except the convention project.
`TestAudit.ps1` runs `Test.ps1 -Convention`, the convention project, which holds the audits, only.
Each passes every other argument on unchanged and returns the exit code of `Test.ps1`.
They hold no test logic of their own, so `Test.ps1` stays the one test script.

## Changed

Workers ran whole test projects while they worked, since naming the symbols they had touched took a guess.
`-Changed` takes the symbols from git, so no guess is needed.
`TraceSymbol.ps1 -Changed` compares the working tree with `-Since`, `HEAD` by default, staged and unstaged edits alike.
Each changed line seeds the innermost symbol that holds it.
A line no symbol holds, such as a `using` directive, seeds nothing.
A change that only removes lines seeds the holders of the line before the gap and of the next line.
A deleted source seeds nothing, since the code that used it fails to build instead.
A source git does not track yet seeds every top-level type it declares.
A new test file therefore runs its own tests.
A graft query writes no card for a new file, so `-Changed` rebuilds the index with `graft build` first.
The run is then narrowed as symbol names narrow it, and names given beside `-Changed` add their own seeds.
When no test is affected, the script says so and exits 0 without a build.
The convention project still runs whole when `-All` or `-Convention` selects it.
`-Since` takes a commit or a tree, so a supervisor's saved tree marks where a worker's part began.
`-Since` without `-Changed` is refused, and so is a revision git cannot find.
The walk is the one symbol names take, unbounded, since `Test.ps1` passes no depth.
A Conduct change that the console window builds therefore reaches the tests that boot the window.
`Test.json` is raised to generation 7 and `TraceSymbol.json` to generation 2 to match.

## KeepTrx

`ScanTest.ps1` needs trx files, and a plain run leaves none.
`-KeepTrx` adds a trx logger to every project of every run and keeps each file.
The folder is the `trx` property of `Test.json`, which is required with this switch and created when missing.
Each file is named `<Project>.<tree>.<yyyyMMdd-HHmmss>.trx`.
The stamp is the start of that run, so each run of `-Repeat` writes its own files.
`<tree>` is the first 12 hex digits of the git tree of the whole working tree, staged or not.
The tree is written through a copy of the index, so the real index stays untouched.
That copy takes every file git does not ignore, new files included.
The tree in the name lets ScanTest compare only runs of one unchanged tree.
A fix is then not mistaken for a flaky test.
`-KeepTrx` cannot be combined with `-List`, since a listing runs no test.
A failure of `git add` or `git write-tree` stops the script before any test runs.
`Test.json` is raised to generation 7 for this switch too.
