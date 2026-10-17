# Kit

The console framework Demeanor builds for itself: the loop, the port to the terminal and the cell width of text.
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

## `BCanvasWidth`

The width side of the canvas: how many cells a text takes on the console.
It is its own object, so the later canvas grids hold one and stay whole in their file.

## `_bCanvasWidthAmbiguous`

Whether ambiguous characters such as `ǐ`, `★` and box drawing take two cells.
The constructor takes it.
Only `-find` builds one yet, and it passes narrow.
The setting holds for the object's life, so it has one writer, the constructor.

## `BCanvasWidthResolve`

Sums the cells of each grapheme cluster `StringInfo` cuts from the text, so one cluster gives its own width.
A cluster takes the widest of its characters, so a combining mark joins its base at no cost.
A lone combining mark, enclosing mark, format or control character takes 0.
A cluster holding U+FE0F asks for emoji presentation and takes 2.
Every other character takes its cells from `BCanvasRange`.
Categories come from the runtime that cuts the clusters, so both read one Unicode version.

## `BCanvasRange`

The cell-width table, written by `scripts/UnicodeWidth.ps1` and never edited by hand.
Its Unicode version is pinned to the runtime's, 16.0.0 for .NET 10.
`BCanvasRangeFirst` partitions every code point into runs, each starting where its class changes.
`BCanvasRangeNarrow` and `BCanvasRangeWide` give each run's cells under each ambiguous setting.
The table holds the answer for both settings, so no class code needs decoding.
Wide and fullwidth characters and emoji presentation take 2, ambiguous ones 1 or 2, and the rest 1.
Reserved code points in the wide blocks take 2, as the Unicode data lists them.

## Stamp

Commit: `52c21d68b4c4`
- `BCanvasRange.cs` `07ec2bb6aa90b979`
- `BCanvasWidth.cs` `cd29cad80ec2fd92`
- `BConsole.cs` `d3f3566823aa415c`
- `BConsoleStream.cs` `15ce67e32788d41a`
- `BLoop.cs` `19a29a372875f2a4`
- `BObserver.cs` `55a41481c90c90c6`
