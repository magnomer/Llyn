# PAnchorMenu.cs

## `public partial class PEditor`

The editor's anchor dropdown: one popup the panel owns, retargeted at the reflex row that asked for it.
It lists the character's stored placements with a tick each, and a tick sends one request for that pair.

## `private const string PAnchorMenuStyle`

The tick-row style, the one the language filter of the catalog uses.

## `private PReflexItem? _pAnchorRow`

The reflex row the open popup edits, `null` while it is closed.

## `internal void PReflexAnchorHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the popup under the label that was pressed, filled for the row the command carries.

## `private void PAnchorBuild(PReflexItem row)`

Fills the list with one tick row per stored placement, ticked where `row` is anchored.
The empty notice shows when the character has no stored placement yet.

## `private void PAnchorTickHandle(object sender, RoutedEventArgs e)`

Applies the tick to the row at once, so its label follows, then sends the anchor request to the engine.

## `private void PAnchorClosedHandle(object? sender, EventArgs e)`

Forgets the row when the popup closes.
