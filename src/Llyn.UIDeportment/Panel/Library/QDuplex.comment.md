# QDuplex.cs

## `internal sealed class QDuplex`

Drives the duplex panel: two wings and when they start and stop.
Searching, picking and reading live in the wing, so this file only forwards to both.
The panel itself is the veneer's `PDuplex` page, which the window places.

## `internal QDuplex(UserControl surface)`

Takes the page the window pulled under the contract ID `PDuplex`.
The page places the veneer's `PWing` twice, and each place gets its own wing driver.

## `internal void QDuplexAttach(PWindow host)`

Puts both wings to work on the window `host`.
Each wing subscribes for itself, so the panel subscribes to nothing.

## `internal void QDuplexRestore(CWorkspaceState state)`

Puts both wings back on the workspace open now, each standing on the Entry `state` names for it.
Each wing gets a vista started now under `left` or `right`, so a switched workspace's layout is read.
Both vistas are blank, so a wing with nothing typed offers no rows.

## `internal void QDuplexClose()`

Releases what both wings hold open.
