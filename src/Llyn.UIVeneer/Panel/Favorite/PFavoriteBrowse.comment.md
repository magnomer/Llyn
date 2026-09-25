# PFavoriteBrowse.cs

## `public partial class PFavorite`

Browsing the marked entries: the search, the ordering, the roster, and the switch between reading and editing.
The panel shell lives in the file beside this one.

## `private async void PFavoriteWorkspaceUpdate()`

A workspace that moved empties the panel and loads its flags before the rows are built.

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

## `private void PRosterHandle(object sender, RoutedEventArgs e)`

Selects the clicked entry, after the editor has been given the chance to keep unsaved changes.

## `internal void PRosterEntryShow(long id)`

Shows the entry on the shared panel, which loads it and drives the display and any open editor from there.
It asks nothing, because the window asks before it jumps.

## `private void PFavoriteScribeHandle(object sender, RoutedEventArgs e)`

Swaps the read view for the editor, and back, through the shared panel.

## `internal bool PFavoriteLeaveConfirm()`

Asks the shared panel whether the unsaved changes may be dropped.

## `private void PSeriesGraspUpdate()`

Reads the roster again when a grasp changed under the grasp ordering, and does nothing otherwise.
Under any other ordering the grasp is not shown, so the rows stay where they are.

## `internal async void PFavoriteVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Takes the vista the window started for this tab and puts the panel on it.
The panel re-reads the roster through the vista whenever the engine announces a change, wherever it was made.
Each subject the panel cares about is attached once, so no handler sorts announcements by subject.
A vista announcement re-lists the roster, since order, filter or query moved.
A mark set from another tab puts its row here without the tab being opened.
A reflex fill or a flipped setting rewrites the epithet beside a headword, so each re-lists too.
A changed grasp re-lists only under the grasp ordering, because only then does the row move.
A draft edit or a fetched frequency, paradigm, script or fanqie row changes no listed row, and is not attached.
The dropdown mark and the filter mark are drawn from it first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The roster is then listed from the vista.
The same vista is handed to the display, which reads its chosen entry from it.

## `private void PStrainerRestore()`

Shows the filter mark while the vista hides any language.

## `internal void PFavoriteScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on, through the shared panel.

## `internal long PFavoriteVoyageRead()`

The Entry the panel shows, as the station the window's trail records.
Zero says the panel shows none, so there is no place to come back to.
