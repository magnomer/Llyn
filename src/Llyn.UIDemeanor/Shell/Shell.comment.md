# Shell

The console program's start and the pieces no single console panel owns.
It mirrors Deportment's `Shell`, so each start step has the same name and seam on both families.

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
Only the reason is kept, the window shows it in a box once it runs, and the start goes on.
Strict mode builds no theme.
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
In the window mode it first opens the window.
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
It takes a seam and asks it for a detached atelier, so a command line never touches a stored layout.
The seam is called after the invocation check, so a missing invocation throws before any atelier exists.
With the window's envoy it opens a `BWindow` on an attached atelier instead.
`BBootstrapWindowAttach` builds the window with the loop, the blueprint and the theme, and gives it to the envoy.
It opens as the loop's first work item, and its batch, key and size observers attach to the loop.
The held rescue and theme reason are shown as the next item.
The loop ends when the window's exit task completes, and the run returns 0.
The presence seam answers whether a path names an existing file.
The strict run hands it to the invocation.
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

## `BEnvoy`

The window's answer to Conduct's `CEnvoy`, which asks through sublevel boxes drawn over the window.
It answers confirms, failures, the leave question, a coinage name, the import dialog and the omission list.
The other reads, such as file, workspace, folder, media and markup, still return null, and the press read false.
It finds `ASParley` in the blueprint, and takes the arrow from that blueprint's glyphs.
A box needs the window, which `BEnvoyWindowAttach` gives it.
Without a window every show falls back to a held notice, and every question gets no answer.
The envoy holds notice lines under a lock, since a fault may come from another thread.

## `BEnvoyParleyConsult`

Builds one box and asks the window to run it.
A question has Yes and No buttons with No as the safe answer, and any other box has OK.
The leave, coinage and import boxes name their own buttons, and the last is the safe one.
It answers null when no window is attached or the window has exited.

## `BEnvoyFaultShow`

Shows a fault box in the `alert` role with the message, and the log path as a path line.
The fault handler of the box's own run is the one passed in.
A box that cannot show holds the same lines as notices.

## `BEnvoyRefusalShow`

Shows the start refusal, the workspace and the reason as an alert box with OK.

## `BEnvoyRescueShow`

Shows the rescue sentence, the backup path and the reason in a `dialog` box.
A box that cannot show holds the same lines as notices.

## `BEnvoyThemeShow`

Shows a fixed English theme sentence and the reason in a `dialog` box.
The sentence is English because the catalog holds no key for it.
A box that cannot show holds the head, the sentence and the reason as one notice.

## `CEnvoyConfirm`

Asks in an alert box, or in a dialog box when a tally follows the question.
Only Yes answers true, and Escape answers No.

## `CEnvoyUnionConfirm`

Asks a dialog box with the dropped and the kept name around the arrow.

## `CEnvoyLeaveConfirm`

Asks with Save, Discard and Cancel, and Cancel is the safe answer.
Save answers true, Discard false and anything else null.

## `CEnvoyCoinageRead`

Asks for a name in a field, and the Create button is enabled only while the wording is valid.
It answers the typed text when Create is pressed, and otherwise null.

## `CEnvoyCustomsRead`

Shows the import dialog with an Import button on F2 and a Cancel button.
Import starts enabled only if every row is ready.
It answers true only when Import is pressed.

## `BEnvoyWindowAttach`

Gives the envoy the window it draws its boxes on.

## `BEnvoyNoticeAdd`

Holds one line until the console is restored.

## `BEnvoyNoticePrint`

Prints the held lines on stderr in order, then clears them.

## `CEnvoyFailureShow`

Shows the failure sentence in an alert box with OK.
With a ledger notice the box adds the notice's sentence, and its label and path when both exist.
A box that cannot show holds the same sentences after the `llyn: ` head, as strict mode prints them.

## `CEnvoyOmissionShow`

Shows the omissions in a box with a Close button.
A box that cannot show holds one line per omission, with its line number and text, after the same head.

## `BWindow`

The full-screen window, owning a canvas that follows the console size.
It draws a frame of two bordered halves, an opening line and the quit legend.
The layout comes from the `AWindow` blueprint, and the window only fills the parts it names.
The constructor finds `AEstablishment` and `ALegend` in `AWindow` and `ANoticeText` in `ANotice`.
A missing blueprint or part throws at construction, not while drawing.
Below 80 by 24 cells it draws only the small-console notice, wrapped by cells.
The window ends the run through `BWindowExit` and holds the atelier and the envoy it was given.
The atelier may be null, when the start was refused and only a box is shown.
It also holds the loop and the stack of open boxes.

## `BWindowOpen`

Reads the size, observes it and then opens the atelier with the envoy, when there is one.
The marshal it hands over comes from `BObserverCreate`.
Work from the loop's thread runs at once, and other work is deferred onto the loop.

## `BWindowSizeObserve`

Builds a new canvas with the theme when the size differs from the last one, and marks the frame changed.
A negative size counts as 0, and the first call always builds a canvas.
The new canvas starts blank, so the next sync prints the whole frame.

## `BWindowBatchObserve`

Draws and syncs the frame once when a size or a box key marked it changed since the last batch.
Without a mark no frame is built, so a burst of keys costs one frame.

## `BWindowKeyObserve`

With a box open and the console large enough, every key except Ctrl+C goes to the top box.
A box that has answered leaves the stack, and the frame is marked changed.
Otherwise F10, Ctrl+C and the character U+0003 are a request to quit.
The atelier's quit confirmation decides, and only a yes completes the exit task.
Without an atelier the request quits at once.
Every other key does nothing.

## `BWindowParleyConsult`

Puts a box on the stack and runs the loop nested until the box answers or the window exits.
Queued work keeps running in the nested loop, and a fault there goes to the handler given.
When the window exits, the box gets Escape, so it answers its safe index.
It answers null at once if the window has already exited.

## `BWindowFrameBuild`

Places the `AWindow` blueprint for the current size and draws its frames with the glyph set each names.
The open boxes draw last, in stack order, so the top box covers the rest.
The opening line fills `AEstablishment`, one cell in from the left.
The legend puts `10` and its label nine slots of 8 cells right of `ALegend`, so at column 72.
The label is `Legend.Quit`, or `Legend.Close` while a box is open.
The left half is half the width, and the right half takes the rest.
Each half has a rule above its last inner row.
Row 0 is the roof and stays blank.

## `BWindowLegendBuild`

Places one legend key, the number and then the label of the key it is given.
The two roles are the legend's theme with `-number` and `-label` added, so the theme file styles them.

## `BWindowNoticeBuild`

Fills `Window.Small` with the minimum and the actual size, then wraps it by cells.
A console narrower than 44 or lower than 8 cells gets the bare text in the `text` role.
Each line starts at the left edge on its own row.
A larger one gets a framed `ANotice` box 42 cells wide, centred, with ` Llyn ` on its top border.
The box height follows the wrapped lines, and lines past the box are cut.
The quit legend sits on the last row at column 0.

## `BBootstrapStrayObserve`

Records an unobserved task fault and marks it observed.

## `BObserver`

Builds the `marshal` delegate the atelier takes, the console's counterpart of `QObserver`.

## `BObserverCreate`

Captures the loop that is the current context when the delegate is built.
A call on the loop's thread runs at once, and a call from another thread is deferred onto the loop.
It refuses a thread with no loop, since the marshal would have nowhere to go.

## Stamp

Commit: `9a85a3ab82db`
- `BBootstrap.cs` `4ef31cb830fd2059`
- `BEnvoy.cs` `0d329ee34b22daeb`
- `BObserver.cs` `55a41481c90c90c6`
- `BWindow.cs` `954b2630ad2b98f3`
