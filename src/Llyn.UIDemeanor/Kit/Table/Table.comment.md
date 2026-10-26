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
The labels shown run from the top for at most one page.
The lit label is clamped into the list, so a start past the end lights the last.

## `BDropObserve`

Up and Down move the lit label, clamped, and keep it inside the page.
Enter, Space, Escape and F10 return false, which tells the table to close the drop.
Every other key returns true and does nothing, so a drop holds the keyboard while open.

## `BDropLitMove`

Moves the lit label by a step, clamped, and keeps it inside the page.
Up and Down call it with one step.
An owner that picks the next label itself calls it with the gap.
A move clears the letters typed so far, so a seek never continues across a move.

## `BDropSeek`

The editor's lists let the user type a letter to reach a label, which `BDropObserve` does not do.
So the owner hands a printable character to `BDropSeek`, and it returns true only when a label matched.
Letters typed in a row extend one prefix, and the search starts at the lit label itself.
A single letter starts after the lit label, so repeating it walks through the labels with that initial.
When the longer prefix matches nothing, the letter alone is tried before the typed text is dropped.
The match ignores case under the current culture, since the labels are in the interface language.
A control character or an empty list is not taken, so the key stays with its other binding.

## `BDropDraw`

The editor draws a drop as a box under its field, where `BTable` draws its own drops elsewhere.
The box sits under the anchor row.
It flips above only when it would run past the bounds and the room above fits it.
It is then clamped into the bounds, so it never leaves the part that hosts it.
A label is cut to leave room for its edge mark, such as a check.
The owner supplies the mark per row.
The frame glyphs come from the glyph set, with ASCII ones when the set lacks the six marks.
A one-cell shadow runs down the right side and under the bottom, so the box reads as raised.
The caller passes no focus to draw the box with no lit row.
So a list that opens before the user has chosen lights nothing, and a disabled one draws muted.

## Stamp

Commit: `f3a687d70b47`
- `BDrop.cs` `f8922ce6a8c78481`
- `BTable.cs` `8fdf9d20e2ea8f4c`
