# TInterfaceConductPortrait.cs
Hash: `c0b5da50cc1382f2`

## `internal static class TInterfaceConductPortrait`

The relays for Conduct's portrait maps and its shared export and print cores.
They are apart from `TInterfaceEnginePortrait`, which relays the engine's side of a likeness.
Each relay is transparent and carries no test logic of its own.
The print openers alone compose a workspace, so Conduct facts reach a real print session.

## `internal static LPortraitMedium TPortraitMediumRead(CPortraitMedium medium)`

Relays the internal export format map.

## `internal static LPortraitLabel TPortraitLabelRead(LSettingsPort settings)`

Relays the label wording, so a fact reads each key through a fake settings port.

## `internal static LPortraitLegend TPortraitLegendRead(LSettingsPort settings, string realm)`

Relays the legend wording of `realm`.

## `internal static IReadOnlyList<CPortraitChoice> TPortraitChoiceRead()`

Relays the export formats the file question offers.

## `internal static IReadOnlyList<CPortraitChoice> TPortraitChoiceRead(IReadOnlyList<(LPortraitMedium, string, bool)> rows)`

Relays the export format map over given engine rows, so a fact feeds hostile rows.

## `internal static Task TPortraitFileExport(CEnvoy envoy, string file, Func<string, LPortraitMedium, Task> export)`

Relays the shared export core with its question and failure policy.
The fixed `file` is handed on as the core's name read.
Its settings port is the shared fake from `TInterfaceConduct`.

## `internal static Task TPortraitFileExport(CEnvoy envoy, LSettingsPort settings, Func<string> fileRead, Func<string, LPortraitMedium, Task> export)`

Relays the shared export core with a given settings port and name read.
A fact can thus fail the name read or pass no settings.

## `internal static Task TPortraitPrint(CEnvoy envoy, Func<Task<LGalleyRelay?>> open)`

Relays the shared print core with its failure policy.
Its settings port is the shared fake from `TInterfaceConduct`.

## `internal static async Task TPortraitPrint(CEnvoy envoy, LPressBoard board, Exception? failure = null, TPressVault? vault = null)`

Opens a print session through the route a panel's print gate takes.
A fresh workspace engine saves the entry `water` and chooses it in the library vista.
Its fake press answers `board`, and its fake queue answers the two fixed printers.
A `failure` makes the fake queue raise it instead.
A `vault` replaces the workspace's own setting store, so two sessions can share one.
The print core then opens the session through `LPortraitPort.LEnginePressOpen` with the settings label.
So a Conduct fact never builds the engine's print session itself.
The workspace and engine live until the window closes and the core disposes the board.
The suggested file name is `water`, the entry's headword.

## `internal static Task TPressWindowOpen(LPressBoard board, Func<CSPress, Task> check, string? path = "press.pdf", List<string>? asked = null, Exception? failure = null, TPressVault? vault = null)`

Opens a print session over `board` and runs `check` against its gate.
The board may be a fault double, so the fault sweep reaches a gate through this route.
The envoy callback first sets the galley's edit and sight delays to `Timeout.Infinite`.
So no deferred draw fires on its own, and a fact that tests the deferral sets the delays to zero.
The callback then restores the kept setting, as the driver does when the window shows.
That restore lists the printers, so `check` sees them as a reader would.
A `failure` makes the listing raise it, so `asked` records the ledger key `Print.Failed`.
A `vault` handed to two calls carries a setting from the first window to the second.
The envoy callback runs `check` on the thread pool and waits for it.
So a fact can hold a board task open and release it without a deadlock.
A null `path` declines the Save as PDF file question.
Every question the envoy hears lands in `asked`.
