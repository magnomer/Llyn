# check.ps1

Runs the whole done gate in one call and prints one tally line.
It covers the build, the full `test.ps1`, and the nine audits.
It stays untracked through `.git/info/exclude`.

## Usage

```
check [-Grep <pattern>[,<pattern>...]] [-NoBuild] [-NoTest] [-NoAudit] [-Verbose] [-Help]
```

`-Grep` searches `src` and `tests` `.cs` and `.xaml` files for leftover names.
Every grep hit counts as a failure.
`-NoBuild`, `-NoTest` and `-NoAudit` skip a stage for quick loops.
A run with any skip flag never counts as done.
`-Verbose` prints the full audit output when a counter is above zero.
`-Help`, `-?` or `--help` prints the usage and exits without running anything.

## Output

The last line reads `OK` or `FAIL`, then one counter per stage.
The exit code is 0 on `OK` and 1 on `FAIL`.

## Resilience

It passes no flags to the other scripts, so their parameter changes cannot break it.
It reads only the `Counters` section that `report.md` gives every audit.
Every `OK` or `FAIL` row adds to the audit's sum, and any sum above zero fails the run.
A `WARN` row adds to a separate warning sum, shown as `WARN n` after the audit's counter.
A warning never fails the run, since the audit itself turns it to `FAIL` above its ceiling.
An output without a readable `Counters` section shows `UNPARSED` and fails the run.
A missing script, a thrown error, or a non-zero exit also fails the run.
An audit exits 1 when it finds violations, which shows as counters and not as a script error.
The build takes the one `.slnx` or `.sln` file at the root, so no project name is written in.
The build is judged by the exit code and by warning or error codes.
Tests are judged by the exit code and the pass or fail summary lines.
A new audit needs one name added to `$audits`, and a renamed counter needs no edit.
Korean match words are built from character codes to keep the source ASCII.
The console reads and writes UTF-8, so native output and grep hits keep their characters.
