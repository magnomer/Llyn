# Kit

The console framework Demeanor builds for itself.
It holds the loop, the port to the terminal, the canvas and the cell width of text.
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

## `BObserver`

Builds the `marshal` delegate the atelier takes, the console's counterpart of `QObserver`.

## `BObserverCreate`

Captures the loop that is the current context when the delegate is built.
A call on the loop's thread runs at once, and a call from another thread is deferred onto the loop.
It refuses a thread with no loop, since the marshal would have nowhere to go.

## `BCanvasWidth`

The width side of the canvas: how many cells a text takes on the console.
It is its own object, so the canvas holds one and stays whole in its file.

## `_bCanvasWidthAmbiguous`

Whether ambiguous characters such as `ǐ`, `★` and box drawing take two cells.
The constructor takes it.
The window builds its widths narrow.
The setting holds for the object's life, so it has one writer, the constructor.

## `BCanvasWidthResolve`

Sums the cells of each grapheme cluster `StringInfo` cuts from the text, so one cluster gives its own width.
A cluster takes the widest of its characters, so a combining mark joins its base at no cost.
A lone combining mark, enclosing mark, format or control character takes 0.
A cluster holding U+FE0F asks for emoji presentation and takes 2.
Every other character takes its cells from `BCanvasRange`.
Categories come from the runtime that cuts the clusters, so both read one Unicode version.

## `BCanvasWidthDivide`

Cuts a text into lines of at most the given cells, never splitting a grapheme cluster.
A line breaks at a space, and spaces at a line start are dropped.
Two neighbouring wide clusters break between them, unless they are Hangul, which breaks only at spaces.
A span of 0 or less gives no lines.
A line with no break point is cut where the next cluster would overflow.
A cluster wider than the span still takes a line of its own.

## `BCanvasCell`

One cell of the grid holds its text, its width and its role.
Width 1 is a normal cell and width 2 is the head of a wide cluster.
Width 0 is the empty tail after a head.
The role names the style the cell belongs to.

## `BCanvasText`

One placed text: its row, column, span in cells, value and role.
The frame is a list of these, so a driver never writes a cell itself.

## `BCanvas`

The grid of cells the window draws on, one drawn grid and one grid the terminal shows.
The grid is as wide and high as the console, and a negative size is refused.
The drawn grid starts blank and the shown grid starts unknown, so the first sync writes every cell.

## `BCanvasFrameDraw`

Rebuilds the drawn grid from blanks and the placed texts, in order, so a later text covers an earlier one.
A text is clipped to its span and the row, and a row or column off the grid is skipped.
A cluster of zero cells is skipped.
A wide cluster that does not fit takes a blank cell and ends its text.
Writing over half of a wide cluster blanks the other half, so no half character stays.

## `BCanvasSync`

Prints only the runs that differ from the shown grid, each after a cursor move to its row and column.
A run grows to cover a wide tail and any changed cell after it.
A change in a tail starts the run at its head.
A sync with nothing changed prints nothing.
The drawn grid becomes the shown one afterwards.

## `BCanvasRange`

The cell-width table, written by `scripts/UnicodeWidth.ps1` and never edited by hand.
Its Unicode version is pinned to the runtime's, 16.0.0 for .NET 10.
`BCanvasRangeFirst` partitions every code point into runs, each starting where its class changes.
`BCanvasRangeNarrow` and `BCanvasRangeWide` give each run's cells under each ambiguous setting.
The table holds the answer for both settings, so no class code needs decoding.
Wide and fullwidth characters and emoji presentation take 2, ambiguous ones 1 or 2, and the rest 1.
Reserved code points in the wide blocks take 2, as the Unicode data lists them.

## Stamp

Commit: `66a4aa567097`
- `BCanvas.cs` `0f5a31467f0acc03`
- `BCanvasCell.cs` `cb0d077c14c60990`
- `BCanvasRange.cs` `07ec2bb6aa90b979`
- `BCanvasText.cs` `025bd90e380a2df3`
- `BCanvasWidth.cs` `ae7c1dcb36517e5a`
- `BConsole.cs` `b4a6d1d52d852278`
- `BConsoleStream.cs` `0bc001d457003413`
- `BLoop.cs` `5afa5b048abe1562`
- `BObserver.cs` `55a41481c90c90c6`
