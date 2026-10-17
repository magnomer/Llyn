# Dialog

Tests of the bodies that carry a panel's content inside a sublevel box.

## `TCustoms`

Drives the import dialog and the omission list through a fake port and the real envoy.
Picking a mode and a stored target, then pressing F2, answers true.
An append row with no target leaves Import disabled, so F2 does nothing and Escape answers false.
Escape declines at once, and the box shows its title and the position `1 of 1`.
A replace row draws its loss text, the lit mode cell and the target cell.
A drop opened on the last row stays inside the body and shows 5 labels.
A lit label past the page scrolls the drop.
The omission list draws the heading, each line number right-aligned and its text, and a Close button.

## Stamp

Commit: `9a85a3ab82db`
- `TCustoms.cs` `dbcabf49a89a89a8`
