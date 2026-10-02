# QAnchor.cs
Hash: `942ece4b06597d3d`

## `internal sealed class QAnchor`

The editor's anchor dropdown is one popup the panel owns.
It is retargeted at the reflex row that asked for it.
It lists the character's stored placements with a tick each, and a tick hands that pair to one gate.
A placement whose tone class the row's own tone may descend from ends with the estimate mark.
Conduct holds the row the open popup edits, so the driver keeps no row between open and tick.

## `private const string QAnchorMenuStyle`

The tick-row style, the one the language filter of the catalog uses.

## `private const string QAnchorMenuBrush`

The accent brush the estimate mark is drawn in.

## `private const string QAnchorMenuMark`

The mark appended after the label of an estimated placement, a space and `≈`.
It stands at the end so the label reads as stored and the estimate reads as a comment on it.

## `internal QAnchor(FrameworkElement surface)`

Holds the editor scope and subscribes the anchor menu's close and the anchor command's preview on the sound panel.
It takes the plain scope, since it calls no member of the editor.

## `internal void QAnchorIntroduce(CSoundingAnchor anchor)`

Holds the anchor area the editor's driver built over the same desk.

## `private void QAnchorShutRefine(object sender, ExecutedRoutedEventArgs e)`

Shuts an open popup before the anchor command reaches `QAnchorReflexObserve`, so it reopens under the new label.
It only changes the look, so no gate is involved.
Shutting it after the open gate could let the popup's close clear the row that gate just chose.

## `internal void QAnchorReflexObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the id of the row the command carries to the open gate.
Its Refine paints the answer under the pressed label.

## `private void QAnchorRefine(CAnchor menu, UIElement anchor)`

Fills the list with one tick row per offered placement, ticked where the row is anchored.
Shows the empty notice when Conduct says so, then opens the popup under `anchor`.

## `private TextBlock QAnchorLabelBuild(CAnchorRow item)`

The label of one tick row, with the estimate mark in the accent colour after an estimated placement.

## `private void QAnchorTickObserve(object sender, RoutedEventArgs e)`

Hands the ticked placement and its new state to the anchor gate.
The row comes back from the draft change with its new label.

## `private void QAnchorClosedObserve(object? sender, EventArgs e)`

Tells Conduct the popup closed, so the row is no longer selected.
