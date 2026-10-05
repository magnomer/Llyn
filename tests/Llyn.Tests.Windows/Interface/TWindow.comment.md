# TWindow.cs
Hash: `20d60e5a8413b7cf`

## `internal static class TWindow`

The shared runner for test steps that build WPF objects.
A WPF control demands an STA thread, and the xunit thread is not one.
It carries no test logic of its own, so a test keeps its arrange and assert beside it.

## `internal static void TWindowRun(Action run)`

Runs the step on a new STA thread and waits for it to end.
A failure on that thread is rethrown on the test thread with its original stack.
So an assertion or a crash inside the step fails the test instead of vanishing with the thread.
The wait is bounded, so a step that hangs fails the test rather than the whole run.
