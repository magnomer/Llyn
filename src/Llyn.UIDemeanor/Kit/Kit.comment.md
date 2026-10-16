# Kit

The console framework Demeanor builds for itself: the loop and the port to the terminal.
It stands apart from `Shell` because no piece here knows the program, only the console.

## `BConsole`

Demeanor's port to the terminal: diagnostics, results, one input line, the terminal test and the modes.
Diagnostics go to stderr through `BConsoleNoticePrint`, and results to stdout through `BConsoleResultPrint`.
The port holds only what a live driver calls.
Size and keys return with the drivers that call them.
The port decides nothing, so every console decision stays in Demeanor's drivers.
The host wires one implementation, and Demeanor never asks which one it holds.

## `BConsoleResultPrint`

Writes one result line to stdout, the only stream a script reads for the answer.

## `BConsoleLineRead`

Reads one line from stdin, or null at its end, and nothing else.
The question is printed through `BConsoleNoticePrint` first, so each member keeps one role.

## `BConsoleTerminalCheck`

Whether both stdin and stdout are a terminal, the test the mode choice reads.
A strict question reads the same test before it asks for a line.

## `BConsoleModeApply`

Sets UTF-8 for both streams and remembers the code pages it found.

## `BConsoleModeRestore`

Puts back the remembered code pages once, so a second call does nothing.

## `BConsoleStream`

The portable port over `System.Console`, with no operating-system check.
`System.Console` sets the Windows console code page itself when the encoding changes.
UTF-8 is set without a byte-order mark, so a redirected stream receives none.

## `_bConsoleStreamInterruption`

The Ctrl+C handler, which restores the code pages and ends the process with exit code 130.
Ctrl+C ends the process at once, and no `finally` of the caller runs.
The handler exits itself, since the default end gives Windows' own code in place of 130.

## `_bConsoleStreamOutput`

The output encoding found before the apply, exchanged away when restored.
The exchange keeps the Ctrl+C thread and the main thread from restoring twice.

## `BLoop`

The console's one UI thread, and its `SynchronizationContext`.
Conduct's awaits resume on it, as they resume on the WPF dispatcher.
Work runs one item at a time, in the order it was deferred.

## `BLoopRun`

Runs deferred work until the given task completes.
A call inside a work item nests, so a sublevel waits for its answer while queued work keeps running.
The task's completion defers an empty item, so a run blocked on an empty queue wakes up.
A work item that throws goes to the fault handler, and the loop runs on.

## `BLoopDefer`

Queues one work item from any thread.
`Post` defers through it, so every continuation lands on the loop.

## `Send`

Runs at once on the loop's own thread, and otherwise waits for the loop to run it.
An exception from the work is thrown again to the waiting caller, as the dispatcher does.

## `CreateCopy`

Returns the loop itself, since a copy would break the identity checks of `BObserver`.

## `BObserver`

Builds the `marshal` delegate panel factories take, the console's `QObserver`.

## `BObserverCreate`

Captures the loop that is the current context when the delegate is built.
A call on the loop's thread runs at once, and a call from another thread is deferred onto the loop.
It refuses a thread with no loop, since the marshal would have nowhere to go.

## Stamp

Commit: `ed7d18d90760`
- `BConsole.cs` `d3f3566823aa415c`
- `BConsoleStream.cs` `15ce67e32788d41a`
- `BLoop.cs` `19a29a372875f2a4`
- `BObserver.cs` `55a41481c90c90c6`
