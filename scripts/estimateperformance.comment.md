# estimateperformance.ps1

Estimates how long each Llyn method takes, with the time of every method it calls folded in.
It only measures and reports.
It gates nothing and stays out of `check.ps1`, so it has no convention test counterpart.

## Usage

```
estimateperformance [<drill> ...] [-Repeat <n>] [-Warmup <n>] [-Method <name>] [-Depth <n>] [-Share <percent>] [-Top <n>] [-NoInline]
estimateperformance -Test [-Project <name> ...] [-Filter <expression>] [-Repeat <n>] [-Method <name>] ...
estimateperformance -List
estimateperformance -Help
```

## Examples

```
estimateperformance
estimateperformance Markup -Repeat 50 -Method LMarkupFile.LMarkupFileParse
estimateperformance -Test -Filter "FullyQualifiedName~TMarkup" -Repeat 3
estimateperformance -Test -Project Llyn.Internal -Method LEngine.LEngineWorkspaceOpen
```

## Sources

| Mode | Workload | Root of the call tree |
|---|---|---|
| Drill | The drills of `performance/Llyn.Performance` | Each drill's `LDrillCycleRun` beneath `LDrill.LDrillRun` |
| Test | `dotnet test` on the configured test projects | The outermost frame in a test project, the test method |

Drill mode counts only samples beneath `LDrill.LDrillRun`.
So preparation, warmup and the runner never reach the report.
Test mode counts every sample with a test method on its stack.
Test fixtures and shared setup therefore show up inside each test.

## Capture

The script builds the workload first, with tracing off.
It then sets `DOTNET_EnableEventPipe`, `DOTNET_EventPipeOutputPath` and `DOTNET_EventPipeConfig` for the measured runs only.
The runtime of every started process then writes its own trace, named by its process id.
So the `testhost` process that `dotnet test` starts is traced without any attach step.
The providers are the sample profiler and the runtime's loader and JIT events.
Those two runtime keywords are enough to name every sampled method.

## Folding

The folding is a file-based C# app that reads the traces with TraceEvent.
Its source lives inside the script, as `tracer.ps1` keeps its own.
So the name audit never reads tooling code.
The script writes it to the `fold` folder under the output folder, only when its text changed.
`dotnet run --file` restores the package once, then builds from its cache.
It walks every sampled stack from the root towards the innermost frame.

- Only frames of modules whose name starts with the configured prefix are kept.
- Framework frames between them are dropped.
- Framework time at the top of a stack goes to the innermost Llyn frame as its self time.
- Compiler-made names fold back to their method.
  An async state machine, a lambda and a local function each count as the method that holds them.
- Inclusive time counts a method once per sample, however often it recurs on that stack.
- Self time counts only the innermost kept frame.
- An edge counts once per sample for each caller and callee pair on the stack.

A sample of Llyn code with no root on its stack goes under an `[async]` root.
That is code resumed after an `await`, whose caller the stack no longer shows.

## Time

The runtime sampler does not tick at a fixed rate on Windows.
So each sample weighs the time since its thread's previous sample.
A thread's first sample, or one after a gap longer than `gap`, weighs the mean interval instead.
Every time is then divided by `-Repeat`, giving milliseconds per cycle or per test run.
CPU time counts samples that found the thread running, wait time those that found it blocked.
Threads run side by side in test mode, so the times there are summed thread time, not wall time.

The drill process prints a stopwatch time per cycle next to the report.
The two should agree closely.

## Limits

- A method shorter than the sampling interval shows up only statistically, over many cycles.
- The JIT inlines small methods into their callers, and their time then counts as the caller's.
  `-NoInline` keeps every frame, at the cost of slower code overall.
- A test's first call pays for compiling the code it reaches.
  `-Repeat` runs the tests again, but every run starts a fresh process.

## Output

Each run writes a stamped folder under the configured output folder.

| File | Content |
|---|---|
| `report.md` | Sample counts, roots, top methods by inclusive and by self time, and the call tree. |
| `report.json` | Every method, every edge and the call tree, with full names. |
| `flame.html` | The call tree as a flame graph: click a bar to zoom, click the path to go back. |
| `trace/` | The raw `.nettrace` traces and their converted `.etlx` files. |

`-Method` merges every place the method was called from into one tree.
It lists the method's callers, and the flame graph starts at the method.

## Settings

`estimateperformance.json` holds every project-specific value.

| Key | Meaning |
|---|---|
| `configuration` | Build configuration of the measured code. |
| `output` | Folder the stamped report folders go to. |
| `own` | Module name prefix of the kept frames. |
| `providers` | The `DOTNET_EventPipeConfig` value. |
| `interval` | Sample weight in milliseconds before any interval is observed. |
| `gap` | Longest interval in milliseconds taken as a real sample interval. |
| `top`, `depth`, `share` | Defaults of `-Top`, `-Depth` and `-Share`. |
| `drill.project`, `drill.assembly` | The drill project and its built binary. |
| `drill.roots`, `drill.gate` | The drill module and the frame that marks the measured region. |
| `drill.repeat`, `drill.warmup` | Defaults of `-Repeat` and `-Warmup` in drill mode. |
| `test.folder`, `test.projects` | The tests folder and the projects measured by default. |
| `test.windowsOnly` | Projects skipped on a host that is not Windows. |
| `test.repeat` | Default of `-Repeat` in test mode. |
