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
A box with no buttons and no body is refused with `ArgumentOutOfRangeException`.
A box with a body and no buttons draws the body where the button row would be.
It draws no bracketed button, and its shadow still falls below the box.
With no buttons, Tab and Left are not live keys, and the body takes Right, Down and Enter.
Escape then answers the safe index, and the body's choice is still reported once.
With a button beside the body, Tab hands the keys to the button.
Enter then answers the button and chooses nothing.
The box is drawn with a width measure the test passes in, as the window does.

## Stamp

Commit: `c37ca1837ded`
- `TParley.cs` `175f40653c84cccd`
