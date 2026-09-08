# PReferenceAuthor.cs

## `public partial class PReference`

The credited-author region of the Source edit area.
An Author is shared data credited on any number of Sources, so editing one here reaches every one of them.
Crediting and dropping write association rows only, and never the Author itself.
The order belongs to the Source alone, because another Source may order the same authors differently.

## Inline notes

### `private LState _pAuthorState = LState.LStateUnspecified;`

Whether the authorship is unknown as against never having been filled in.
It is a column on the `source` row.
It is saved with the five fields rather than when a credit is made.

## `private void PAuthorFind()`

Reads every Author the workspace holds for the crediting menu.
A workspace holding none shows that state rather than an empty list.

## `private void PAuthorCreditFind(string? stored)`

Fills the credits from the map the shelf already holds, in the Source's own order.
A Source not yet stored can carry no credit, because a credit is a row against a stored id.
The stored id comes from the held draft, which is the only thing that knows whether one exists.

## `private void PAuthorUpdate()`

Reads the credits again after a write and refills both the region and the shelf.
The shelf shows credits too, so one write is one refill for both.

## `private void PAuthorAttach(string id)`

Credits an existing Author at the end of the order.
An Author already credited is not credited twice.
Crediting anyone moves the author state to Specified.

## `private void PAuthorFreshHandle(object sender, RoutedEventArgs e)`

Creates an Author from the typed name and credits it at once.

## `private void PAuthorRemoveHandle(object sender, RoutedEventArgs e)`

Drops one credit and never the Author.
The Author stays available to every other Source.
Dropping the last credit does not move the author state back on its own.

## `private void PAuthorMove(object sender, int step)`

Moves one credit through the Source's order.
Attaching at a position renumbers the whole set, so a reorder never collides mid-write.

## `private void PAuthorRenameHandle(object sender, RoutedEventArgs e)`

Renames an Author to the name typed in the menu's box.
The rename is an explicit action rather than a side effect of typing in the credit list.
The name is rewritten and never the id, so every Source keeps pointing at the same Author.

## `private bool PAuthorRenameConfirm(PAuthorItem item)`

Says how many Sources the rename reaches before it is applied.
The figure comes from the credit map the panel already holds.

## `private void PAuthorUnknownHandle(object sender, RoutedEventArgs e)`

Records that the authorship is unknown, or takes that record back.
`Anonymous` is a credited Author and not a substitute for either state.
The state is a field of the Source.
Changing it pushes to the held draft as any typed field does.
