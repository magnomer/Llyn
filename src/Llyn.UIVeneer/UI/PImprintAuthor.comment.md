# PImprintAuthor.cs

## `public partial class PImprint`

The credited-author region of the Source edit area.
An Author is shared data credited on any number of Sources.
The credits themselves are a list the held draft carries, and every credit change is a request.
So a source nothing has stored yet can already be credited, and nothing is written until it is saved.
The order belongs to the Source alone, because another Source may order the same authors differently.

Each credit is a row holding one field that names the author it credits.
Typing in the field never renames the Author, which is shared, but changes which Author the row credits.
Renaming and deleting an Author belong to an author tab of their own.

## Inline notes

### `private LStateMark _pAuthorState = LStateMark.LStateMarkUnspecified;`

The authorship mark as the held draft has it, unknown as against never having been filled in.
It is carried whole rather than read apart, so the area resolves no state.
It is a column on the `source` row.
It is saved with the five fields rather than when a credit is made.

### `private IReadOnlyList<LAuthor> _pAuthorCredited = [];`

The credits last shown, kept so the blank row can move without a draft to re-read.

### `private int _pAuthorBlankAt = -1;`

Where a row not yet crediting anyone stands, as the place its credit would take, or none.
The draft carries no such row, because an empty credit is nothing to hold.
So the area remembers it across the redraws the bulletins bring, until it is filled or dropped.

## `internal void PAuthorFind()`

Reads every Author the workspace holds, so a credit can show a name renamed since the draft took it.

## `private void PAuthorOpen(LDraft? draft)`

Shows a newly held draft, or none, and forgets any blank row the last one left behind.

## `private void PAuthorShow(LDraft? draft)`

Shows the credits and the author state of the held draft, or the notice when there is no draft.

## `private void PAuthorCreditShow(IReadOnlyList<LAuthor> credits)`

Refills the credit rows only when what they show differs from the list given.
A bulletin that changed nothing here then costs no redraw, and keeps the field being typed into.
The blank row, when one is kept, is put back at its place among the credits.
A draft crediting nobody always shows one blank row, as an entry's card always shows one sentence line.
Otherwise there would be no row to add from.

## `private IReadOnlyList<LAuthor> PAuthorCreditRead(LDraft draft)`

The draft's credits with each stored Author's name read from the catalog.
The draft carries the name it was given, and a rename made since is the catalog's to know.
A minted credit is in no catalog and keeps its typed name.

## `private bool PAuthorCreditMatch(IReadOnlyList<LAuthor> credits)`

Whether the rows shown are the credits given, with the blank row at its kept place or absent.

## `private void PAuthorAdd(PAuthorItem item)`

Puts a blank row under the row whose add was pressed, as an entry's sentence add does.
The caret goes into it.
Only one blank row stands at a time, so a second press moves the caret to the one standing.

## `private void PAuthorSelect(PAuthorItem item)`

Focuses the field of one row once the list has drawn its container.

## `private void PAuthorCreditHandle(object sender, RoutedEventArgs e)`

The one handler over every credit row, because the row template is a shared resource.
A template kept in a dictionary can name no handler of its own.
The button carrying the click says in its `Tag` which of the four actions it is.
Dropping a credit never drops the Author, which stays available to every other Source.

## `private void PAuthorMove(PAuthorItem item, int step)`

Moves one credit through the Source's order.

## `private void PAuthorRemove(PAuthorItem item)`

Drops one row.
A blank row is only the area's, so it is dropped without a request.
The last row of a draft crediting nobody comes straight back, since one always stands.

## `private void PAuthorTextHandle(object sender, TextChangedEventArgs e)`

Offers the authors the typed text names as it is typed.
A field refilled by a redraw is not being typed into, and opens nothing.

## `private void PAuthorKeyHandle(object sender, KeyEventArgs e)`

Enter takes the offered row the arrows reached, else credits what was typed.
Escape puts the credited name back.
The dropdown never takes focus, so the field's key handler drives it.

## `private void PAuthorLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)`

A field left with typing not entered shows its credited name again.
Only enter or a chosen row changes a credit, so a glance elsewhere credits nobody by accident.

## `private void PAuthorCommit(PAuthorItem row, TextBox box)`

Credits the typed name in the row's place.
The engine credits the stored Author of that name when one exists, and mints one otherwise.
A name the row already credits is left as it is.

## `private void PAuthorAttach(PAuthorItem row, TextBox box, long id)`

Credits the chosen stored Author in the row's place.

## `private void PAuthorChange(PAuthorItem row, LRequest request)`

Puts the new credit before the old one, then drops the old one.
In that order a request that fails leaves the row crediting what it did.
A blank row has nothing to drop, and is forgotten once its credit is made.

