# Shell

The console program's start and the pieces no single console panel owns.
It mirrors Deportment's `Shell`, so each start step has the same name and seam on both families.
The main window and what it owns sit in `Chamber`, and the roof menu bar in `Rafter`.

## `BBootstrap`

The start of the console program, with every notice a start can raise and the fault handlers.
The host builds the engine and hands each step in as a seam, so no engine type is named.
The engine is a type parameter for the same reason, as in `QBootstrap`.
The console port and the blueprint come through the constructor, so no member sees the bootstrap without them.
The blueprint is the UI family's own `BBlueprint`, so Demeanor names no concrete screen file.
The bootstrap owns the loop and the catalog, and later drivers receive them from it.
In the communicative mode it also owns the window's envoy, and the notices of `BBootstrapNoticePrint` wait there.
The same mode keeps a rescue and a theme reason until the window can show them in a box.

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
That mode builds the envoy with the blueprint, then the console theme, and then applies the window.
It then builds the engine as a leased start.
The theme seam answers a named theme as a role-to-colour function, and the start asks for `console-blue`.
A theme that fails to load or parse falls back to the built-in spare palette.
The theme seam is kept for the run, so the `Options` menu can load another theme by name later.
Of the failure only the reason is kept.
The window shows it in a box once it runs, and the start goes on.
Strict mode builds no theme.
Any other start is strict mode, parsed by `BInvocationParse` before the workspace opens.
A usage error prints its sentence on stderr and returns 2, and the workspace never opens.
A valid command line builds the engine, then hands it to the run seam.
The build seam is told whether the command line is leased, which `BInvocationLeased` answers.
The engine is built from the workspace before the run seam, so a verb's text can speak the stored language.
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
In the window mode it first attaches a window that holds no atelier.
It shows the refusal in an alert box, with the workspace as a path line.
Then the three lines go through `BBootstrapNoticePrint`, so a window start holds them for after the restore.

## `BBootstrapCatalogApply`

Loads the stored language when it is not the default, as `QBootstrapCatalogApply` does.
A failed load keeps the default catalog and shows a fixed English warning.

## `BBootstrapRescueConsult`

Shows the rescue notice, the backup path and the reason, when the start rescued a database.
In the window mode it only keeps them, and `BBootstrapNoticeShow` draws the box when the run starts.

## `BBootstrapFaultIntroduce`

Takes the audit seam and observes unobserved task faults, as `QBootstrapFaultIntroduce` does.

## `BBootstrapAtelierRun`

Runs the loop for one atelier and returns the exit code.
Strict mode asks the atelier seam for a detached atelier, so a command line never touches a stored layout.
The seam is called after the invocation check, so a missing invocation throws before any atelier exists.
With the window's envoy it opens a `BWindow` on an attached atelier instead.
`BBootstrapWindowAttach` builds the window with the loop, the blueprint and the theme, and gives it to the envoy.
It builds one console Capsule and hands it to both the window and the envoy.
So the wardrobe and the path prompt write one file.
It also hands the window a theme loader over the kept theme seam.
A theme that fails to load throws, and the wardrobe decides whether the theme box shows it.
The window gets the envoy's theme box as the wardrobe's report.
The window starts with the default theme's name only when the first load worked.
So the menu does not mark the spare palette as a named choice.
It opens as the loop's first work item and opens the atelier as the second.
Its batch, key and size observers attach to the loop.
The held rescue and theme reason are shown as the next item.
The loop ends when the window's exit task completes, and the run returns 0.
The presence seam answers whether a path names an existing file.
The strict run hands it to the invocation, with the blueprint.
The blueprint is there for the `show` verb, which draws the reading view the way the window does.
The folder, file and drive seams list a folder's folders, list its files and name the logical drives.
Only the window mode uses them, and `BBootstrapAtelierRun` joins the first two into one listing for the path prompt.
The window mode also hands the presence seam on to that prompt.
The strict invocation is the loop's first work item, and the loop ends when it completes.
The item is awaited, since the import and export verbs finish asynchronously.
A fault in that item prints the fault notice and ends the run with 4.
The atelier closes on every return, as the GUI window closes it on exit.
The atelier is never opened by a strict verb, since each verb calls only the gates of its own action.

## `BBootstrapNoticeShow`

Shows the kept rescue first, then the kept theme reason, each in its own box.
Each box waits for its answer before the next draws.

## `BBootstrapFaultObserve`

Hears a work item's fault from the loop and records it through the audit seam.
In the window mode the envoy shows a fault box with the log path and the message.
Without a window it prints the fault notice through `BBootstrapFaultPrint`.
The loop then runs on, as the dispatcher does after a handled fault.
Before `BBootstrapFaultIntroduce` the audit seam records nothing, yet the notice still shows.

## `BBootstrapFaultPrint`

Records the fault and sends the head, the log path and the message to `BBootstrapNoticePrint`.
The envoy hands it to the fault box as the handler of the box's own run.
So a box that fails to draw still leaves a notice.

## `BBootstrapNoticePrint`

Sends one notice line to the envoy to hold in the window mode, and otherwise prints it on stderr.
Holding the line keeps the alternate screen from hiding it.

## `BBootstrapStrayObserve`

Records an unobserved task fault and marks it observed.

## `BObserver`

Builds the `marshal` delegate the atelier takes, the console's counterpart of `QObserver`.

## `BObserverCreate`

Captures the loop that is the current context when the delegate is built.
A call on the loop's thread runs at once, and a call from another thread is deferred onto the loop.
It refuses a thread with no loop, since the marshal would have nowhere to go.

## Stamp

Commit: `36e31eb71c98`
- `BBootstrap.cs` `3ab8b832f4245916`
- `BObserver.cs` `55a41481c90c90c6`
