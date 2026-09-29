# PCategoryItem.cs

## `internal sealed class PCategoryItem`

Presentation item for one row of the `PCategory` menu.
It is the name Conduct's menu row carries for one part of speech.
That is also the name the row hands the speech gate when it is clicked.
It also carries the engine's verdict that the entry already wears that name as a chip.
The row draws a check on that, so the menu and the chips read as one state.

The item is immutable, because the menu is rebuilt rather than edited.
Every repaint of the menu rebuilds the rows, so nothing here has to announce a change.
