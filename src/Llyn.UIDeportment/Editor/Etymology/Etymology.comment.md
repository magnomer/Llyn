# Etymology

This folder holds the editor's etymology map drivers.
They draw the boxes Conduct reads and pass every change to the etymology gate.
No driver here decides whether a change is accepted.

## `QEtymology`

The driver repaints from one read of the gate, whenever the gate or the entry draft announces a change.
The entry draft notice also refreshes the headword at the foot of the map.
The map is a tree, so each box carries its own list of sources, and the template nests them.
Box parts are found by name inside each container, so the template owns the layout.
Handlers are removed before they are added, since a container is reused for another box.
A text box stops listening while its text is set, so a repaint never writes back what it just drew.

### Fork

Each trunk rule is bound to `HasItems` of the sources list above it, so a box without sources draws none.
The map's own trunk above the headword is bound the same way to the top list.
A box hides its left half-bar when it is the first of its siblings.
It hides its right half-bar when it is the last.
So the half-bars of a sibling row join into one fork, and a lone source draws no bar.

### Box states

A linked box shows its word in the accent colour with a mark.
Its tongue choice gives way to the tongue's name.
A borrowed box is read only, with no tools and no frame, and shows the dashed trace.
The insert button hides on a linked box, since the engine refuses an insert above one.
The restore button shows only for the box the gate shelved.

### Choices

The tongue and relation lists are set with their change handler removed, so filling them selects nothing.
A custom choice carries a context menu to delete it.
Any other choice clears the menu a reused row had.
The row keeps a choice only by name for a tongue and by id for a relation.

### Picking

A source word is picked through the shared prospect driver.
A key press and a click both reach the gate through the mention pick command on the text box.
The driver sets the link only when the pick carries an entry id.
The gloss writes only while its box has keyboard focus, so a repaint never echoes into the gate.

## `QEtymologyChoice`

One option of a tongue or relation list, wrapping the Conduct choice for the combo box.
The text is the choice's name when it has no key, and the localized key otherwise.
So a built-in relation follows the language, and a custom one shows what the user typed.

## `QEtymologyItem`

One box of the map as a row of the tree.
`QEtymologyItemShow` rebuilds the tree from the flat rows by their depth.
A box's sources are the rows after it, down to the next row of its own depth.
`QEtymologyItemFirst` and `QEtymologyItemLast` mark a box as the first or the last of its siblings.
Rows are matched by the key, so a box that survives a repaint keeps its container.
Every kept row is refined and raises one change notice, so its container repaints in place.
The relation and tongue lists are shared by every row of one read.

## Stamp

Commit: `2f9a06b8eb95`
- `QEtymology.cs` `4d4d758bca7e3a63`
- `QEtymologyChoice.cs` `81ca0bacab667286`
- `QEtymologyItem.cs` `25e81f18626dce6f`
