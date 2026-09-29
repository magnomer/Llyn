# PAnchorMenu.cs

## `public partial class PEditor`

The editor's anchor dropdown: one popup the panel owns, retargeted at the reflex row that asked for it.
It lists the character's stored placements with a tick each, and a tick hands that pair to one gate.
A placement whose tone class the row's own tone may descend from ends with the estimate mark.
Conduct holds the row the open popup edits, so the driver keeps no row between open and tick.

## `private const string PAnchorMenuStyle`

The tick-row style, the one the language filter of the catalog uses.

## `private const string PAnchorMenuBrush`

The accent brush the estimate mark is drawn in.

## `private const string PAnchorMenuMark`

The mark appended after the label of an estimated placement, a space and `≈`.
It stands at the end so the label reads as stored and the estimate reads as a comment on it.

## `private void PAnchorAttach()`

Subscribes the anchor menu's close, which the markup named, and the anchor command's preview.

## `private void PAnchorShutRefine(object sender, ExecutedRoutedEventArgs e)`

Shuts an open popup before the anchor command reaches `PReflexAnchorObserve`, so it reopens under the new label.
It only changes the look, so no gate is involved.
Shutting it after the open gate could let the popup's close clear the row that gate just chose.

## `internal void PReflexAnchorObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the id of the row the command carries to the open gate.
Its Refine paints the answer under the pressed label.

## `private void PAnchorRefine(CAnchor menu, UIElement anchor)`

Fills the list with one tick row per offered placement, ticked where the row is anchored.
Shows the empty notice when Conduct says so, then opens the popup under `anchor`.

## `private TextBlock PAnchorLabelBuild(CAnchorRow item)`

The label of one tick row, with the estimate mark in the accent colour after an estimated placement.

## `private void PAnchorTickObserve(object sender, RoutedEventArgs e)`

Hands the ticked placement and its new state to the anchor gate.
The row comes back from the draft change with its new label.

## `private void PAnchorClosedObserve(object? sender, EventArgs e)`

Tells Conduct the popup closed, so the row is no longer selected.
