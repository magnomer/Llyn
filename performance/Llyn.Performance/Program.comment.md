# Program.cs
Hash: `78c451c217af7731`

The entry point of the drill process.
`scripts/TracePerformance.ps1` starts it with the runtime tracing switched on.

## Arguments

| Argument | Meaning |
|---|---|
| `--list` | Print every drill name and exit. |
| `--drill <a,b>` | Run only the named drills. The default runs every drill. |
| `--repeat <n>` | Timed cycles per drill, 20 by default. |
| `--warmup <n>` | Untimed cycles per drill before the timed ones, 3 by default. |

A drill's name is its type name without the `LDrill` prefix.
An unknown argument or drill name exits with code 2.

## Output

Each drill prints its wall time per cycle from a stopwatch.
The report's time for the same drill should come close to it.
A large gap means the samples were too few or the sampling was disturbed.
