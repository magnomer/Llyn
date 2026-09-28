# QFavoriteBrowse.cs

## `internal sealed partial class QFavorite`

Browsing the marked entries: the search, the ordering, the roster, and the switch between reading and editing.
The panel shell lives in the file beside this one.

## `private async void QFavoriteWorkspaceUpdate()`

A workspace that moved empties the panel and loads its flags before the rows are built.

## `private void QRecallHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement re-lists the roster.
Typing selects nothing and marks nothing.

## `private void QSeriesHandle(object sender, RoutedEventArgs e)`

Takes the chosen ordering from the clicked row, closes the dropdown and hands the ordering to the vista.
The vista saves it and announces it, and the announcement re-lists the roster.

## `private void QStrainerHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the clicked box's menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

## `private void QRosterFind()`

Reads the marked entries the engine returns for the vista, already filtered and sorted.
Before a vista is handed over the roster is emptied and nothing is asked.
Ordering by the mark reads when the mark was made, never when the entry was written.
An unknown workspace leaves the roster empty and tells the user why.
The empty line is shown only while no row stands.
Rows arrive as shapes, numbered and with their epithets, so the panel only copies them.

## `private void QRosterRowShow(QRosterItem? item)`

Selects the clicked entry, after the editor has been given the chance to keep unsaved changes.
The click hands over the row's item, so no control decides the request.

## `private void QRosterApply(FrameworkElement container, object item, string? _)`

Fills one roster row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The epithet leads with an en space, as its string format did.

## `internal void QRosterEntryShow(long id)`

Shows the entry on the shared panel, which loads it and drives the display and any open editor from there.
It asks nothing, because the window asks before it jumps.

## `private void QFavoriteScribeHandle(object sender, RoutedEventArgs e)`

Swaps the read view for the editor, and back, through the shared panel.

## `internal bool QFavoriteLeaveConfirm()`

Asks the shared panel whether the unsaved changes may be dropped.

## `internal async void QFavoriteVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Takes the vista the window started for this tab and puts the panel on it.
The panel re-reads the roster through the vista whenever the engine announces a change, wherever it was made.
Each subject the panel cares about is attached once, so no handler sorts announcements by subject.
A vista announcement re-lists the roster, since order, filter or query moved.
A mark set from another tab puts its row here without the tab being opened.
A reflex fill or a flipped setting rewrites the epithet beside a headword, so each re-lists too.
A changed grasp goes to `CFavoriteGraspUpdate`, which re-lists only under the grasp ordering.
A draft edit or a fetched frequency, paradigm, script or fanqie row changes no listed row, and is not attached.
The dropdown mark and the filter mark are drawn from it first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The roster is then listed from the vista.
The same vista is handed to the display, which reads its chosen entry from it.

## `private void QStrainerRestore()`

Shows the filter mark while the vista hides any language.

## `internal void QFavoriteScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on, through the shared panel.

## `internal long QFavoriteVoyageRead()`

The Entry the panel shows, as the station the window's trail records.
Zero says the panel shows none, so there is no place to come back to.
