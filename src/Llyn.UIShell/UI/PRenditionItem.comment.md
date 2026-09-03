# PRenditionItem.cs

## `internal sealed class PRenditionItem`

One rendition row, in the editor that writes them and in the display that reads them back.
It is mutable where `PAnthologyItem` is not, because the editor binds to it and the user types into it.
A row already carrying an id keeps it, so an id anything holds stays valid across a save.
A row written here carries no id until the store assigns one.
Position is not held: the order is the order of the list, and reordering is moving the row.
