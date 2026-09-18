# PGuildAutograph.cs

## `public partial class PGuild`

The editing half of the authors panel: naming one Author, saving it, folding it into another, and deleting it.
The name is held by a tenure, so the engine owns the draft, the dirty check and the commit.
The panel keeps only the tenure and the union list, and reads the chosen Author from the vista.

## `private LTenure? _pAutographTenure;`

The engine's hold on the Author being named, or null while the edit area is closed.

## `private void PAutographOpen(long? id)`

Opens the edit area on one Author, or on a new one when no id is given.
A tenure is started on the Author and the name it holds is shown, so no copy is kept here.
The union section shows only for a stored Author, because nothing can be folded into an unstored row.

## `private void PAutographCancel()`

Ends the tenure without saving and clears the edit area.

## `private void PAutographTallyShow(long id)`

Writes the two count chips from the catalog row, and zero for an Author not yet stored.

## `private bool PAutographChangeCheck()`

Whether the tenure reports the held name changed from the stored one.

## `private void PAutographChangeUpdate()`

Lights the save button while the edit area is open and the typed name is neither blank nor stored.

## `private void PAutographNameHandle(object sender, TextChangedEventArgs e)`

Defers the typed name to the tenure and relights the save button.

## `private void PAutographDraftUpdate(LBulletin bulletin)`

Relights the save button when the engine wrote the held draft, and only for this tenure.

## `private void PAutographTenureUpdate(LBulletin bulletin)`

Relights the save button when the tenure's state moved, and only for this tenure.

## `private bool PAutographStoreRun()`

Finishes the tenure with a store: a rename for a stored Author, a creation for a new one.
A blank name is refused with a notice before the engine is asked, because an Author is its name.
A created Author becomes the chosen one, and the edit area reopens on it for a fold.

## `private void PAutographUnionHandle(object sender, TextChangedEventArgs e)`

Lists the Authors the typed name matches, the chosen one left out, up to eight.

## `private void PAutographUnionSelect(object sender, RoutedEventArgs e)`

Folds the chosen Author into the clicked one after the window has asked, then reads the kept Author.
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
