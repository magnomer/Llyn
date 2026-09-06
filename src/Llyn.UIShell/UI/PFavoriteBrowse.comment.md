# PFavoriteBrowse.cs

## `public partial class PFavorite`

Browsing the marked entries: the search, the ordering, the roster, and the switch between reading and editing.
The panel shell lives in the file beside this one.

## `private async void PFavoriteHandle(object sender, DependencyPropertyChangedEventArgs e)`

Fills the roster the first time the tab is shown, and again on every later showing.
The flags are loaded before the rows are built, so a row never renders a flag it could have had.

## `private void PRecallHandle(object sender, TextChangedEventArgs e)`

Narrows the roster to the marked entries whose headword carries the typed text.
Typing selects nothing and marks nothing.

## `private void PSeriesHandle(object sender, RoutedEventArgs e)`

Takes the chosen ordering from the clicked row and closes the dropdown.
The roster is read again rather than reordered in place.

## `private IEnumerable<LFavorite> PSeriesSort(IReadOnlyList<LFavorite> favorites)`

Orders the marked entries by the current choice.
Headword order is what the engine already returns, so that choice reorders nothing.
`Marked` orders by when the mark was made, never by when the entry was written.

## `private void PRosterFind(string query)`

Reads the marked entries carrying the query and rebuilds the rows in the chosen order.
An unreadable workspace leaves the roster empty and tells the user why.
The empty line is shown only while no row stands.

## `private void PRosterUpdate()`

Reads the roster again after a mark or an unmark.
An unmarked entry loses its row at once, while the display keeps showing it.

## `private void PRosterHandle(object sender, RoutedEventArgs e)`

Selects the clicked entry, after the editor has been given the chance to keep unsaved changes.

## `internal void PRosterEntryShow(string id)`

Loads the entry and shows it in the display.
An entry that has left the workspace clears the panel and re-reads the roster.
An open editor is moved onto the same entry, so the two never stand on different records.

## `private void PRosterEntryUpdate(string id)`

Re-reads the roster and the display after the editor stored the entry.
A headword the store changed must reach the row that carries it.

## `private void PEditorEntryRestore()`

Puts the editor back on the selected entry after a discard.
Nothing selected leaves the editor empty.

## `private void PFavoriteScribeHandle(object sender, RoutedEventArgs e)`

Swaps the read view for the editor, and back.
Leaving the editor asks about unsaved changes first, then reloads the entry as stored.

## `private void PFavoriteScribeShow(bool editing)`

Shows one of the two views and relabels the button with what the next click does.

## `internal bool PFavoriteLeaveConfirm()`

Asks the window whether the unsaved changes may be dropped.
An editor holding nothing answers yes without asking.

## `private void PFavoriteEntryShow(string id, LEntryDraft draft)`

Fills the display and enables the mode toggle, which is dead while nothing is selected.

## `private void PFavoriteClear()`

Returns the panel to nothing selected, reading, and no editor open.
