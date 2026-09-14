# PGuildAutograph.cs

## `public partial class PGuild`

The editing half of the authors panel: naming one Author, saving it, folding it into another, and deleting it.

## `private void PAutographOpen(long? id)`

Opens the edit area on one Author, or on a new one when no id is given.
The stored name is remembered apart from the typed one, so a change is a difference and never a flag.
The union section shows only for a stored Author, because nothing can be folded into an unstored row.

## `private void PAutographCancel()`

Drops whatever the edit area held, typed or not.

## `private void PAutographTallyShow(long id)`

Writes the two count chips from the catalog row, and zero for an Author not yet stored.

## `private bool PAutographChangeCheck()`

Whether the typed name differs from the stored one, blanks trimmed.

## `private void PAutographChangeUpdate()`

Lights the save button while the edit area is open and the typed name is neither blank nor stored.

## `private void PAutographNameHandle(object sender, TextChangedEventArgs e)`

Relights the save button as the name is typed.

## `private bool PAutographStoreRun()`

Saves the typed name: a rename for a stored Author, a creation for a new one.
A blank name is refused with a notice, because an Author is its name.
A created Author becomes the chosen one and stays open for editing, so another can be folded in at once.

## `private void PAutographUnionHandle(object sender, TextChangedEventArgs e)`

Lists the Authors the typed name matches, the edited one left out, up to eight.

## `private void PAutographUnionSelect(object sender, RoutedEventArgs e)`

Folds the edited Author into the clicked one after the window has asked, then reads the kept Author.
The kept Author is read rather than edited, because the fold is the last thing done to the dropped one.

## `private void PGuildFreshHandle(object sender, RoutedEventArgs e)`

Opens the edit area on a new Author, after asking about an unsaved name.
The toggle goes live so the reading side can be returned to, which clears the unsaved Author.

## `private void PGuildStoreHandle(object sender, RoutedEventArgs e)`

Saves the held Author.

## `private void PGuildBinHandle(object sender, RoutedEventArgs e)`

Deletes the chosen Author, from the reading side or the editing side alike.
An uncredited Author is simply deleted.
A credited one is named to the user with its source count, then detached and deleted in one store call.
