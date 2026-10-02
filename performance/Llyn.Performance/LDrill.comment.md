# LDrill.cs
Hash: `adec4890ce1e91a4`

## `internal abstract class LDrill`

One fixed workload of the performance project.
A drill is prepared once, warmed up outside the measurement, then run a counted number of cycles.
The profiler counts only samples taken beneath `LDrillRun`, so preparation and warmup never reach the report.

## `public abstract void LDrillPrepare();`

Builds the drill's input before any cycle runs.
Its cost stays out of the report.

## `public abstract void LDrillCycleRun();`

Runs the workload once.
It is the root of the drill's call tree in the report.

## `public void LDrillRun(int repeat)`

The measured region.
It runs `repeat` cycles back to back.
It is never inlined, because the profiler finds the measured region by this frame.
