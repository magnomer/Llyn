# Shell

The console program's start and the pieces no single console panel owns.
It mirrors Deportment's `Shell`, so each start step has the same name and seam on both families.

## `BBootstrap`

The start of the console program, with every notice a start can raise and the fault handlers.
The host builds the engine and hands each step in as a seam, so no engine type is named.
The engine is a type parameter for the same reason, as in `QBootstrap`.
The console port comes through the constructor, so no member sees the bootstrap without it.
The bootstrap owns the loop and the catalog, and later drivers receive them from it.

## `BBootstrapNoticeHead`

The `llyn: ` head every diagnostic line starts with, as strict mode prescribes.
It is the command's name, the same in every language, so no catalog carries it.
The strict envoy prints its lines under the same head, so the constant is internal.

## `_bBootstrapInvocation`

The parsed command line, kept from the start until the run seam reaches `BBootstrapAtelierRun`.

## `BBootstrapIntroduce`

Installs the loop as the thread's context first, then applies the console modes.
It loads the default catalog and chooses the mode before the workspace opens.
No arguments with stdin and stdout on a terminal is the communicative mode.
That mode is not built yet, so it prints `Window.Unbuilt` on stderr and returns 2.
Any other start is strict mode, parsed by `BInvocationParse` before the workspace opens.
A usage error prints its sentence on stderr and returns 2, and the workspace never opens.
A valid command line builds the engine, then hands it to the run seam.
Every valid verb runs with the workspace open, so its text speaks the stored language.
A run seam that throws is a fault: the fault notice, then exit 4.
A catalog that fails to load prints a fixed English line and returns 1, as `LHost` exits 1.
The modes are restored on every return, and Ctrl+C restores them through the port.

## `BBootstrapBuild`

Reads the workspace and builds the engine, as `QBootstrapBuild` does for the GUI.
A failure prints the sentence `CAtelierRefusalRead` chooses, the workspace and the reason, and the start exits 3.

## `BBootstrapCatalogApply`

Loads the stored language when it is not the default, as `QBootstrapCatalogApply` does.
A failed load keeps the default catalog and prints a fixed English warning.

## `BBootstrapRescueConsult`

Prints the rescue notice, the backup path and the reason, when the start rescued a database.

## `BBootstrapFaultIntroduce`

Takes the audit seam and observes unobserved task faults, as `QBootstrapFaultIntroduce` does.

## `BBootstrapAtelierRun`

Runs the loop for one atelier and returns the exit code.
The strict invocation is the loop's first work item, and the loop ends when it completes.
A fault in that item prints the fault notice and ends the run with 4.
The atelier closes on every return, as the GUI window closes it on exit.
The atelier is never opened, since each verb calls only the gates of its own action.
The window lands here later as another task the loop runs.

## `BBootstrapFaultObserve`

Hears a work item's fault from the loop, records it and prints the fault notice with the log path.
The loop then runs on, as the dispatcher does after a handled fault.
Before `BBootstrapFaultIntroduce` the audit seam records nothing, yet the notice still prints.

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

Commit: `ed7d18d90760`
- `BBootstrap.cs` `6971d6359ccc4aef`
- `BLocalizationCatalog.cs` `6b3081293256d764`
