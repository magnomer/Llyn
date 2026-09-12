# PImprintAuthor.cs

## `public partial class PImprint`

The credited-author region of the Source edit area.
An Author is shared data credited on any number of Sources, so renaming one here reaches every one of them.
The credits themselves are a list the held draft carries, and every credit change is a request.
So a source nothing has stored yet can already be credited, and nothing is written until it is saved.
The order belongs to the Source alone, because another Source may order the same authors differently.

## Inline notes

### `private LState _pAuthorState = LState.LStateUnspecified;`

Whether the authorship is unknown as against never having been filled in.
It is a column on the `source` row.
It is saved with the five fields rather than when a credit is made.

## `internal void PAuthorFind()`

Reads every Author the workspace holds for the crediting menu.
A workspace holding none shows that state rather than an empty list.

## `private void PAuthorShow(LDraft? draft)`

Shows the credits and the author state of the held draft, or nothing when there is no draft.
Crediting is offered whenever a draft is held, since the credits live on the draft now.

## `private void PAuthorCreditShow(IReadOnlyList<LAuthor> credits)`

Refills the credit rows only when what they show differs from the list given.
A bulletin that changed nothing here then costs no redraw.

## `private IReadOnlyList<LAuthor> PAuthorCreditRead(LDraft draft)`

The draft's credits with each stored Author's name read from the catalog.
The draft carries the name it was given, and a rename made since is the catalog's to know.
A minted credit is in no catalog and keeps its typed name.

## `private void PAuthorAttach(long id)`

Credits an existing Author at the end of the order, unless already credited.

## `private void PAuthorFreshHandle(object sender, RoutedEventArgs e)`

Credits a new Author from the typed name at the end of the order.
The Author row is created only when the Source is saved.

## `private void PAuthorCreditHandle(object sender, RoutedEventArgs e)`

The one handler over every credit row, because the row template is a shared resource.
A template kept in a dictionary can name no handler of its own.
The button carrying the click says in its `Tag` which of the four actions it is.
Dropping a credit never drops the Author, which stays available to every other Source.

## `private void PAuthorMove(PAuthorItem item, int step)`

Moves one credit through the Source's order.

## `private void PAuthorNameUpdate(PAuthorItem item)`

Renames an Author to the name typed in the menu's box.
The rename is an explicit action rather than a side effect of typing in the credit list.
A stored Author is renamed in the store, and never its id, so every Source keeps pointing at it.
A credit not stored yet is only a name on the draft.
So it is dropped and credited again under the new name.

## `private void PAuthorRequestSend(LRequest request)`

Sends one credit request and redraws from what the engine answered.
Typing still waiting in the five fields is written first, so the requests reach the engine in order.

## `private bool PAuthorRenameConfirm(PAuthorItem item)`

Says how many Sources the rename reaches before it is applied.
The figure is counted by the panel, which is where the credit map is held.

## `private void PAuthorUnknownHandle(object sender, RoutedEventArgs e)`

Records that the authorship is unknown, or takes that record back.
`Anonymous` is a credited Author and not a substitute for either state.
The state is a field of the Source.
Changing it pushes to the held draft as any typed field does.
