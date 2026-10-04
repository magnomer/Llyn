# TBootstrapContext.cs
Hash: `160359e5c04253c5`

## `public sealed class TBootstrapContext`

Covers the dispatcher context the bootstrap installs on the startup thread.
The test runs on its own STA thread, as the host's startup does, with no context yet.

## `public void BootstrapContext_ResumesAnUnfinishedAwaitOnTheStartupThread()`

An await on a task still running resumes on the startup thread once the context is installed.
Without it the continuation lands on the thread pool, which is how the first launch on an empty workspace crashed.
A dispatcher frame is pumped until the task ends, standing in for the run that follows the window build.
