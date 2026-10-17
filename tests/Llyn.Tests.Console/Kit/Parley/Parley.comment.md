# Parley

Tests of the console sublevel box, mirroring `src/Llyn.UIDemeanor/Kit/Parley/`.

## `TParley`

The box draws centred at 80 by 24, with its shadow on the right column and the row below.
At 40 columns it shrinks to the console less 2 cells.
An anchor puts the box at that row and column.
A long path wraps and draws every row.
A disabled button draws in `disabled`, and the lit button in `lit`.
Right skips a disabled button, so Enter answers the next enabled one.
Escape answers the safe index.

## Stamp

Commit: `9a85a3ab82db`
- `TParley.cs` `0bea1ac82378c9a7`
