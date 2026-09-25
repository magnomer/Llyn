# PAnchorMenu.cs

## `public partial class PEditor`

The editor's anchor dropdown: one popup the panel owns, retargeted at the reflex row that asked for it.
It lists the character's stored placements with a tick each, and a tick sends one request for that pair.
A placement whose tone class the row's own tone may descend from ends with the estimate mark.

## `private const string PAnchorMenuStyle`

The tick-row style, the one the language filter of the catalog uses.

## `private const string PAnchorMenuBrush`

The accent brush the estimate mark is drawn in.

## `private const string PAnchorMenuMark`

The mark appended after the label of an estimated placement, a space and `≈`.
It stands at the end so the label reads as stored and the estimate reads as a comment on it.

## `private LReflexItem? _pAnchorRow`

The reflex row the open popup edits, `null` while it is closed.

## `internal void PReflexAnchorHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the popup under the label that was pressed, filled for the row the command carries.

## `private void PAnchorBuild(LReflexItem row)`

Fills the list with one tick row per stored placement, ticked where `row` is anchored.
The engine marks each placement, reading the classes the row's tone may descend from under the entry language's tone rows.
The empty notice shows when the character has no stored placement yet.

## `private TextBlock PAnchorLabelBuild(PAnchorItem item)`

The label of one tick row, with the estimate mark in the accent colour after an estimated placement.

## `private void PAnchorTickHandle(object sender, RoutedEventArgs e)`

Sends the tick to the engine as an anchor request, and the row comes back rebuilt with its label.

## `private void PAnchorClosedHandle(object? sender, EventArgs e)`

Forgets the row when the popup closes.
