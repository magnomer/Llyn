# Table

The console's rows and cells that a user moves through and picks from.

## `BTable`

A page of rows over a count, with a lit row, a lit cell and an open drop.
Each cell has a builder for its drop and a callback for the choice.
A table with no cells only scrolls, and the arrows move its top row.
The count may not be negative and the page must be positive.

## `BTableObserve`

Returns true when the table took the key.
Up and Down move the lit row, and Left and Right move the lit cell, both clamped to their range.
Enter or Space opens the drop of the lit cell, if its drop is enabled.
The top row follows the lit row, so the lit row stays in the page.
With a drop open, the drop sees the key, and Enter or Space first reports the choice.
Every key is taken while a drop is open, except F10.
F10 closes the drop and is not taken, so the box can answer.

## `BDrop`

A list of labels with a lit one, a top and a page.
It is built with the label that starts lit, and the page must be positive.
A drop with no labels, or built disabled, is not enabled and never opens.

## `BDropObserve`

Up and Down move the lit label, clamped, and keep it inside the page.
Enter, Space, Escape and F10 return false, which tells the table to close the drop.
Every other key returns true and does nothing, so a drop holds the keyboard while open.

## Stamp

Commit: `9a85a3ab82db`
- `BDrop.cs` `bab88970ad068492`
- `BTable.cs` `8fdf9d20e2ea8f4c`
