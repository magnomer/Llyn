# PLibraryBrowse.cs

## `public partial class PLibrary`

Browsing behavior of the library panel.
The search box and the ordering dropdown refill the entry index.
A chosen index row is loaded back from the workspace.
It is rendered read-only in the display area, which the mode toggle swaps for the editor.
This is the read half of the entry round trip.
The input panel writes an entry, and this reads it back and corrects it.

## Inline notes

### `private string? _pDisplayEntry;`

The entry the right-hand side stands on, or null when none is selected.
The display may show it and the editor may be correcting it.
Either way it is the one entry this panel is on.

### `private LCatalogOrder _pOrderChoice;`

Which ordering the index is listed in, held as one of the orderings the engine supports.
It starts as whatever the workspace stored, which the window applies before the panel is first shown.
The dropdown tag is read into that set, so the panel offers nothing the engine cannot do.
The ordering is the same one whatever language names it.

### `private async void PLibraryBulletinHandle(LBulletin bulletin)`

The panel answers the engine rather than its own visibility.
So a save made in the input panel lands here at once, with no tab switch to trigger it.
A workspace that moved is the one announcement that empties the panel first, and its flags are reloaded before any row is built.
Every other announcement re-lists the index and refreshes the entry this panel stands on.

### `private void POrderHandle(object sender, RoutedEventArgs e)`

A chosen ordering renames the button and re-lists the index.
Nothing about the selected entry changes.
An ordering says in which order the entries are offered, never which one is shown.

### `private void PIndexFind(string query)`

Refills the index with the entries the engine returns for the query and the ordering.
An empty box lists everything.
The display is not touched: the search says which entries are offered, never which one is shown.

### `if (!PLibraryLeaveConfirm())`

Selecting another entry leaves whatever is being written behind, so it is asked about first.
The row that was clicked is not worth the correction that was typed.

### `private void PIndexEntryShow(string id)`

Puts the whole right-hand side on one entry.
The display is filled from the store.
An editor that is open moves onto the same entry, since the entry being edited is selected.

### `PLibraryClear();`

The entry went away between the search and the click.
The row is stale.
So the list is re-read rather than left offering a row that no longer loads.

### `private void PIndexEntryUpdate(string id)`

After a store, the headword the index lists may have changed.
So may the text the display shows.
Each is read again from what was written.
The editor keeps the entry it is on — the user corrected it, they did not leave it.

### `return;`

The write went through.
A display that cannot be filled again is not worth reporting as a failure that did not happen.

### `private void PEditorEntryRestore()`

The discard the editor hands over: the selected entry comes back as it is stored.
With no entry selected there is nothing stored to come back to, so the form is emptied instead.

### `private void PLibraryFreshHandle(object sender, RoutedEventArgs e)`

The new-entry command: the selection is dropped and the editor opens on a blank form.
Nothing selected means the discard falls back to an empty form, so the writer stays in the new entry.
The blank form starts a draft of its own under the `Library` origin, so a new word here and a new word in the input panel are two tentative entries and not one.
Each carries its own id and its own file, so neither answer to the exit question can reach the other.
The origin is what a leftover reports itself by, which is how recovery can say where the work was being typed.

### `private void PLibraryScribeHandle(object sender, RoutedEventArgs e)`

The mode toggle: reading becomes writing on the selected entry, and writing goes back to reading what is actually stored.
Leaving the editor is what the unsaved question stands in front of.

### `if (_pDisplayEntry is not null)`

A correction that was given up leaves the editor holding text the store never took.
So the display is filled from the store again rather than from what was on screen.

### `private void PLibraryScribeShow(bool editing)`

Which of the two halves the broader side shows, and what the toggle then offers.
The half chosen is pushed downstream as it changes, so the panel opens on it next time.

### `internal bool PLibraryLeaveConfirm()`

The question put before the editing state is left.
That is selecting another entry, or toggling back to the display.
It is anything else that would leave typed corrections behind.
The window asks it too, for a jump that lands on the library the user is already writing in.
Nothing unsaved means nothing to ask about.

### `private void PLibraryEntryShow(string id, LEntryDraft draft)`

The shared display draws the entry, and this panel decides what the toggle may then do.
An entry is selected now, so it can be written as well as read.

### `private void PLibraryClear()`

The panel must not assume an entry is selected.
With none chosen the display shows only its prompt.
There is nothing to write, so the editor is closed and the toggle offers nothing.

### `internal void POrderRestore(LCatalogOrder order)`

Puts the panel back on the ordering the workspace stored, and moves the dropdown mark onto it.
The window calls it once on attach, so the panel never reads the stored state for itself.

### `internal void PLibraryScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored, because an empty editor is the state a new record is written in.
