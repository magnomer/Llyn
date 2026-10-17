# Console

The port to the terminal and the loop that runs the console's one UI thread.
It stands apart from `Shell` because no piece here knows the program, only the console.

## `BConsole`

Demeanor's port to the terminal: diagnostics, results, one input line, the terminal test and the modes.
It also prints a frame, reads the size, reads a key and applies the window.
Diagnostics go to stderr through `BConsoleNoticePrint`, and results to stdout through `BConsoleResultPrint`.
The port holds only what a live driver calls.
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

## `BConsoleFramePrint`

Writes a frame to stdout as it is and flushes at once.
The canvas builds the frame, so the port adds no line end.

## `BConsoleSizeRead`

Answers the console's width and height in cells.

## `BConsoleKeyRead`

Waits for one key and returns it unechoed.
It blocks, so only the loop's key reader thread calls it.

## `BConsoleWindowApply`

Enters the alternate screen, hides the cursor and makes Ctrl+C a key.
The old Ctrl+C setting is remembered for the restore.

## `BConsoleWindowRestore`

Shows the cursor, leaves the alternate screen and puts back the Ctrl+C setting.
It acts once, so a second call and a call without an apply do nothing.
The exchange keeps the Ctrl+C thread and the main thread from restoring twice.

## `BConsoleModeApply`

Sets UTF-8 for both streams and remembers the code pages it found.
It takes the interruption the Ctrl+C handler calls first.

## `BConsoleModeRestore`

Puts back the remembered code pages once, so a second call does nothing.

## `BConsoleStream`

The portable port over `System.Console`, with no operating-system check.
`System.Console` sets the Windows console code page itself when the encoding changes.
UTF-8 is set without a byte-order mark, so a redirected stream receives none.

## `_bConsoleStreamInterruption`

The Ctrl+C handler, which runs the caller's interruption, restores the window and the code pages, and exits with 130.
Ctrl+C ends the process at once, and no `finally` of the caller runs.
The interruption lets the caller print held notices before the exit.
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
A call may bring its own fault handler, which then replaces the loop's for that run.
So a box that fails to draw reports to the handler of its own run.
A run that starts with an empty queue runs the batch first.
So a nested run draws its first frame.

## `BLoopBatchAttach`

Sets the work that runs whenever the queue has just emptied.
The window draws its frame there, so a burst of keys gives one frame.
A batch that throws goes to the fault handler like any work item.

## `BLoopKeyAttach`

Starts a background thread that reads keys from the port and defers each to the observer.
A failure of the read is thrown again on the loop, so the fault handler sees it.

## `BLoopSizeAttach`

Starts a background thread that asks the loop to read the size every 100 milliseconds.
The next ask waits until the loop has run the last one, so asks never pile up.

## `BLoopDefer`

Queues one work item from any thread.
`Post` defers through it, so every continuation lands on the loop.

## `Send`

Runs at once on the loop's own thread, and otherwise waits for the loop to run it.
An exception from the work is thrown again to the waiting caller, as the dispatcher does.

## `CreateCopy`

Returns the loop itself, since a copy would break the identity checks of `BObserver`.

## Stamp

Commit: `9a85a3ab82db`
- `BConsole.cs` `b4a6d1d52d852278`
- `BConsoleStream.cs` `0bc001d457003413`
- `BLoop.cs` `8eca3f514cac409a`
