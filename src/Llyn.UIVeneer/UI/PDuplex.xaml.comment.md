# PDuplex.xaml.cs

## `public partial class PDuplex : UserControl`

The duplex panel as a control: two wings and when they start and stop.
Searching, picking and reading live in the wing, so this file only forwards to both.

## `private LPosture _lPosture = null!;`

The window's posture the vistas are started through at each restore.

## `internal void PDuplexAttach(PWindow host)`

Puts both wings to work on the window deportment and takes the posture from `host`.
Each wing subscribes for itself, so the panel subscribes to nothing.

## `internal void PDuplexRestore(LWorkspaceState state)`

Puts both wings back on the workspace open now, each standing on the Entry `state` names for it.
Each wing gets a vista started now under `left` or `right`, so a switched workspace's layout is read.
Both vistas are blank, so a wing with nothing typed offers no rows.

## `internal void PDuplexClose()`

Releases what both wings hold open.
