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
The box is drawn with a width measure the test passes in, as the window does.

## Stamp

Commit: `c1396871c007`
- `TParley.cs` `9ed1224b37cf73e6`
