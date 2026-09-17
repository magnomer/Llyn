# PFavoriteBrowse.cs

## `public partial class PFavorite`

Browsing the marked entries: the search, the ordering, the roster, and the switch between reading and editing.
The panel shell lives in the file beside this one.

## `private LVista? _pFavoriteVista;`

The engine's view state for the favorite tab: order, filter and query.
The panel keeps no copy of any of the three and reads each from the vista where it needs it.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

## `private async void PFavoriteBulletinHandle(LBulletin bulletin)`

Re-reads the roster whenever the engine announces a change, wherever it was made.
A vista announcement carrying this panel's vista id re-lists the roster, since order, filter or query moved.
Another panel's vista is not this panel's business and is skipped.
A mark set from another tab puts its row here without the tab being opened.
A workspace that moved empties the panel first, and its flags are loaded before the rows are built.
A draft edit or a fetched frequency, paradigm, script, fanqie or reflex row changes no listed row.
Those announcements are skipped.
A changed grasp is the exception when the roster is ordered by grasp, because the row moves.

## `private void PRecallHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement re-lists the roster.
Typing selects nothing and marks nothing.

## `private void PSeriesHandle(object sender, RoutedEventArgs e)`

Takes the chosen ordering from the clicked row, closes the dropdown and hands the ordering to the vista.
The vista saves it and announces it, and the announcement re-lists the roster.

## `private void PStrainerHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

## `private void PRosterFind()`

Reads the marked entries the engine returns for the vista, already filtered and sorted.
Before a vista is handed over the roster is emptied and nothing is asked.
Ordering by the mark reads when the mark was made, never when the entry was written.
An unknown workspace leaves the roster empty and tells the user why.
The empty line is shown only while no row stands.
Rows arrive numbered and with their epithets, so the panel only copies them.

## `private void PRosterUpdate()`

Reads the roster again after a mark or an unmark.
An unmarked entry loses its row at once, while the display keeps showing it.

## `private void PRosterHandle(object sender, RoutedEventArgs e)`

Selects the clicked entry, after the editor has been given the chance to keep unsaved changes.

## `internal void PRosterEntryShow(long id)`

Loads the entry and shows it in the display.
An entry that has left the workspace clears the panel and re-reads the roster.
An open editor is moved onto the same entry, so the two never stand on different records.

## `private void PRosterEntryUpdate(long id)`

Re-reads the roster and the display after the editor stored the entry.
A headword the store changed must reach the row that carries it.

## `private void PEditorEntryRestore()`

Puts the editor back on the selected entry after a discard.
Nothing selected leaves the editor empty.

## `private void PFavoriteScribeHandle(object sender, RoutedEventArgs e)`

Swaps the read view for the editor, and back.
Leaving the editor asks about unsaved changes first, then reloads the entry as stored.


## `if (editing == (PEditor.Visibility == Visibility.Visible))`

Both segments answer here, so the click is read off which one was pressed.
The segment already standing for what is on screen changes nothing.
## `private void PFavoriteScribeShow(bool editing)`

Shows one of the two views and relabels the button with what the next click does.
The side chosen is pushed downstream as it changes, so the panel opens on it next time.

## `internal bool PFavoriteLeaveConfirm()`

Asks the window whether the unsaved changes may be dropped.
An editor holding nothing answers yes without asking.

## `private void PFavoriteEntryShow(long id, LEntryDraft draft)`

Fills the display and enables the mode toggle, which is dead while nothing is selected.

## `private void PFavoriteClear()`

Returns the panel to nothing selected, reading, and no editor open.

## `private void PSeriesGraspUpdate()`

Reads the roster again when a grasp changed under the grasp ordering, and does nothing otherwise.
Under any other ordering the grasp is not shown, so the rows stay where they are.

## `internal async void PFavoriteVistaRestore(LVista vista)`

Takes the vista the window started for this tab and puts the panel on it.
The dropdown mark and the filter mark are drawn from it first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The roster is then listed from the vista.

## `private void PSeriesRestore()`

Moves the dropdown mark onto the ordering the vista holds.

## `private void PStrainerRestore()`

Shows the filter mark while the vista hides any language.

## `internal void PFavoriteScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored.
An empty editor is the state a new record is written in.

A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.

### `private void PRosterSelect(string? id)`

Marks the catalog row the panel stands on and clears the mark from every other row.
A null id leaves no row marked, which is what a cleared panel shows.
It is called wherever the shown entry changes, so the mark and the right-hand side never disagree.
