# Shell

The console program's start and the pieces no single console panel owns.
It mirrors Deportment's `Shell`, so each start step has the same name and seam on both families.

## `BBootstrap`

The start of the console program, with every notice a start can raise and the fault handlers.
The host builds the engine and hands each step in as a seam, so no engine type is named.
The engine is a type parameter for the same reason, as in `QBootstrap`.
The console port comes through the constructor, so no member sees the bootstrap without it.
The bootstrap owns the loop and the catalog, and later drivers receive them from it.
In the communicative mode it also owns the window's envoy, and every notice waits there.

## `BBootstrapNoticeHead`

The `llyn: ` head every diagnostic line starts with, as strict mode prescribes.
It is the command's name, the same in every language, so no catalog carries it.
The strict envoy prints its lines under the same head, so the constant is internal.

## `_bBootstrapInvocation`

The parsed command line, kept from the start until the run seam reaches `BBootstrapAtelierRun`.
It stays null in the communicative mode.

## `_bBootstrapEnvoy`

The window's envoy, set only in the communicative mode and null in strict mode.
Its presence tells every later step that the window mode is running.

## `BBootstrapIntroduce`

Installs the loop as the thread's context first, then applies the console modes.
It loads the default catalog and chooses the mode before the workspace opens.
No arguments with stdin and stdout on a terminal is the communicative mode.
That mode builds the envoy and applies the window, then builds the engine as a leased start.
Any other start is strict mode, parsed by `BInvocationParse` before the workspace opens.
A usage error prints its sentence on stderr and returns 2, and the workspace never opens.
A valid command line builds the engine, then hands it to the run seam.
The build seam is told whether the command line is leased, which `BInvocationLeased` answers.
Every valid verb runs with the workspace open, so its text speaks the stored language.
A run seam that throws is a fault: the fault notice, then exit 4.
A catalog that fails to load prints a fixed English line and returns 1, as `LHost` exits 1.
The console is restored on every return through `BBootstrapConsoleRestore`.
Ctrl+C runs the same restore through the port's interruption.

## `BBootstrapConsoleRestore`

Leaves the window, prints the held notices, then restores the modes, in that order.
The notices print after the window is left, so the alternate screen does not swallow them.
Without an envoy no notice is held, and the restore only puts the modes back.

## `BBootstrapBuild`

Reads the workspace and builds the engine, as `QBootstrapBuild` does for the GUI.
It passes the lease flag on to the build seam with the workspace.
A failure shows the sentence `CAtelierRefusalRead` chooses, the workspace and the reason, and the start exits 3.
The three lines go through `BBootstrapNoticePrint`, so a window start holds them.

## `BBootstrapCatalogApply`

Loads the stored language when it is not the default, as `QBootstrapCatalogApply` does.
A failed load keeps the default catalog and shows a fixed English warning.

## `BBootstrapRescueConsult`

Shows the rescue notice, the backup path and the reason, when the start rescued a database.

## `BBootstrapFaultIntroduce`

Takes the audit seam and observes unobserved task faults, as `QBootstrapFaultIntroduce` does.

## `BBootstrapAtelierRun`

Runs the loop for one atelier and returns the exit code.
It takes a seam and asks it for a detached atelier, so a command line never touches a stored layout.
The seam is called after the invocation check, so a missing invocation throws before any atelier exists.
With the window's envoy it opens a `BWindow` on an attached atelier instead.
The window opens as the loop's first work item.
The window's batch, key and size observers attach to the loop before it runs.
The loop ends when the window's exit task completes, and the run returns 0.
The atelier closes on every return, as in the strict run.
The presence seam answers whether a path names an existing file.
The strict run hands it to the invocation.
The strict invocation is the loop's first work item, and the loop ends when it completes.
The item is awaited, since the import and export verbs finish asynchronously.
A fault in that item prints the fault notice and ends the run with 4.
The atelier closes on every return, as the GUI window closes it on exit.
The atelier is never opened by a strict verb, since each verb calls only the gates of its own action.

## `BBootstrapFaultObserve`

Hears a work item's fault from the loop, records it and shows the fault notice with the log path.
The loop then runs on, as the dispatcher does after a handled fault.
Before `BBootstrapFaultIntroduce` the audit seam records nothing, yet the notice still shows.

## `BBootstrapNoticePrint`

Sends one notice line to the envoy to hold in the window mode, and otherwise prints it on stderr.
Holding the line keeps the alternate screen from hiding it.

## `BEnvoy`

The window's answer to Conduct's `CEnvoy`, with no panel to ask yet.
Every question is answered with its refusing default and every read returns null or false.
The envoy holds notice lines under a lock, since a fault may come from another thread.

## `BEnvoyNoticeAdd`

Holds one line until the console is restored.

## `BEnvoyNoticePrint`

Prints the held lines on stderr in order, then clears them.

## `CEnvoyFailureShow`

Holds the failure sentence after the `llyn: ` head, as strict mode prints it.
With a ledger notice it holds the notice's sentence too, and its label and path when both exist.

## `CEnvoyOmissionShow`

Holds one line per omission, with its line number and text, after the same head.

## `BWindow`

The full-screen window, owning a canvas that follows the console size.
It draws a frame of two bordered halves, an opening line and the quit legend.
Below 80 by 24 cells it draws only the small-console notice, wrapped by cells.
The window ends the run through `BWindowExit` and holds the atelier and the envoy it was given.

## `BWindowOpen`

Reads the size, observes it and then opens the atelier with the envoy.
The marshal it hands over comes from `BObserverCreate`, so it runs work at once on the loop.

## `BWindowSizeObserve`

Builds a new canvas when the size differs from the last one, and marks the frame changed.
A negative size counts as 0, and the first call always builds a canvas.
The new canvas starts blank, so the next sync prints the whole frame.

## `BWindowBatchObserve`

Draws and syncs the frame once when the size changed since the last batch.
Keys that change nothing print no frame, so a burst of them costs nothing.

## `BWindowKeyObserve`

Treats F10, Ctrl+C and the character U+0003 as a request to quit.
The atelier's quit confirmation decides, and only a yes completes the exit task.
Every other key does nothing yet.

## `BWindowFrameBuild`

Places the two halves, the opening line and the legend for the current size.
The left half is half the width, and the right half takes the rest.
The legend puts `10` and `Legend.Quit` at column 72, nine slots of 8 cells from the left edge.

## `BWindowSideBuild`

Places one bordered half, with a rule two rows above the bottom border.
The top border sits on row 1, so row 0 stays blank.

## `BWindowNoticeBuild`

Fills `Window.Small` with the minimum and the actual size, then wraps it by cells.
Each line is placed from the left edge on its own row.

## `BBootstrapStrayObserve`

Records an unobserved task fault and marks it observed.

## `BLocalizationCatalog`

The one place a console key resolves, as `QLocalizationCatalog` is for the GUI.
It is an instance the bootstrap owns, since a driver may hold no mutable static state.

## `BLocalizationCatalogApply`

Merges a catalog over the one in place and replaces the whole dictionary.
A key the new catalog lacks keeps its earlier text, as the GUI merge does.

## `BLocalizationTextRead`

Returns the text of a key, or the key itself when no catalog carries it, as the GUI does.

## Stamp

Commit: `66a4aa567097`
- `BBootstrap.cs` `8b05b604324177cd`
- `BEnvoy.cs` `15ac0daeb21b5ed5`
- `BLocalizationCatalog.cs` `6b3081293256d764`
- `BWindow.cs` `0bd68f8d4e67edb1`
