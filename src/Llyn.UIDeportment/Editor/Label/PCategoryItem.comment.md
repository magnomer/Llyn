# PCategoryItem.cs

## `internal sealed class PCategoryItem`

Presentation item for one preset row in the `PCategory` menu.
It is the display name the chosen language declares for one part of speech.
That is also what the row writes into the field when it is clicked.
It also carries whether the entry already wears that name as a chip.
The row draws a check on that, so the menu and the chips read as one state.
It carries the id of the value row behind the name, so a picked row is linked rather than re-resolved.

The item is immutable, because the menu is rebuilt rather than edited.
A chip added or dropped rebuilds the rows, so nothing here has to announce a change.
