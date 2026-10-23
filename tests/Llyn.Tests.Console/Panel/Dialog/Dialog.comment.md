# Dialog

Tests of the bodies that carry a panel's content inside a sublevel box.

## `TCustoms`

Drives the import dialog and the omission list through a fake port and the real envoy.
Picking a mode and a stored target, then pressing F2, answers true.
An append row with no target leaves Import disabled, so F2 does nothing and Escape answers false.
Shift with F2 does not press Import, since a key matches its modifiers exactly.
Escape declines at once, and the box shows its title and the position `1 of 1`.
A replace row draws its loss text, the lit mode cell and the target cell.
A drop opened on the last row stays inside the body and shows 5 labels.
A lit label past the page scrolls the drop.
The dialog bodies draw with a width measure the test passes in.
An open import dialog shows `1Help`, `2Import` and `10Close` in the legend.
In Korean the legend labels cut by cells.
A dialog with a row not ready draws `Import` in the `disabled` role.
The omission list draws the heading, each line number right-aligned and its text, and a Close button.

## Stamp

Commit: `c1396871c007`
- `TCustoms.cs` `ac8885984dfb4078`
