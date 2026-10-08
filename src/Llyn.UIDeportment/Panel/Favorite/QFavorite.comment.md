# QFavorite.cs
Hash: `2c99312f1184aed4`

## `internal sealed class QFavorite`

Drives the favorites panel: what it is made of, when it starts and stops, and how it is browsed.
Browsing is the search, the ordering, the roster and the switch between reading and editing.
The panel itself is the veneer's `QFavorite` page, which the window places.

## `internal QFavorite(UserControl surface)`

Takes the page the window pulled under the contract ID `QFavorite`.
It adds the print and export command bindings to the page.
The rail's print and export buttons sit inside the page, so their commands reach these bindings.
It hands the rail `PFavoriteRail` to a `QPanelRail`, with the bin, no new-record button and the export button.
It hands `PFavoriteOrder` to a `QChoiceOrder`, whose menu hangs under the whole search bar.
It hands `PFavoriteFilter` to a `QChoiceFilter`.
It subscribes the search field and the rail's notices.

## `private Border QSeries`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QFavoriteIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)`

Puts the panel to work through its Conduct `CFavorite`, which it builds over the atelier.
It keeps the atelier and envoy for the catalog load, and hands the volume and mention menu on.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The editor view and the lectern wrap the entry editor that `CFavorite` owns.
The display view builds the lectern over the editor's display, which Conduct attached to the panel.
It hands the picker and the filter the panel's aperture, titled `Series` and `Strainer`.
The ordering menu is built once, from the orderings `CFavorite` offers.
It binds its lists, subscribes the area's events, attaches their row fills, and reads nothing yet.
`QFavoriteVistaRefine` fills it when the workspace opens, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.
The rail is introduced last, with the atelier's navigation for its trail and the editor for its chronicle.

Every mark and unmark is announced by the engine, wherever the star was clicked.
That is how a row leaves the roster the moment the entry it stands on is unmarked.

## `private void QFavoriteStoreUpdate()`

Lights the rail's store button while the editor's draft can be stored.
It runs whenever the editor's desk reports its state again.

## `internal void QFavoriteExitRefine()`

Releases the editor's recording player when the window closes.
The editor and the display stop through `CFavorite`'s closure, which the atelier's exit gate runs.

## `private void QFavoritePressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: when an entry is chosen and the panel is not editing.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void QFavoritePressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The panel names nothing, and the engine builds the page from stored rows.
Nothing is read back from the screen.

## `private async void QFavoritePortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The gate asks for the file and the format through the envoy.
The engine writes the document from stored rows.
Nothing is read back from the screen.

## `private bool QFavoriteShownCheck()`

Tells `CFavorite` whether the page is on screen.

## `private void QFavoriteChronicleUpdate()`

Lights the rail's two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private void QFavoriteModeUpdate()`

Paints the mode from the shared panel state.
No control's visibility stands in for the mode any more.
The rail folds its own button pairs from the scribe flag it is handed.

## `private async void QFavoriteWorkspaceRefine()`

Answers the area's workspace event, which has already closed the chosen entry.
It loads the flags of the workspace that moved, so the rows built next carry the right ones.
The window's envoy goes with the load, so the catalog reports a failed load and answers no languages.

## `private void QRecallObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement re-lists the roster.
Typing selects nothing and marks nothing.

## `internal async void QFavoriteVistaRefine()`

Answers the workspace opening, after `CFavorite` has restored its vista and attached its observers.
The picker's checked row and the filter mark are drawn from the vista first.
The flags are loaded before any row is built.
The filter's language menu is then built from the languages that load answers.
The roster is then listed once, so a row keeps the flag it was built with.
The search text stands in the box already, since the restore carried it into the fresh vista.
Its one request is `CFavoriteRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QRosterRefine()`

Paints the rows `CFavorite` reads, already filtered and sorted, and splices them into the list.
Before a vista is handed over, or when the read fails, the roster is empty.
A failed read is shown to the user by `CFavorite`.
Ordering by the mark reads when the mark was made, never when the entry was written.
The empty line is shown only while no row stands.
Rows arrive as shapes, numbered and with their epithets, so the panel only copies them.

## `private void QRosterRefine(IReadOnlyList<CVistaRow> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QRosterObserve(object sender, RoutedEventArgs e)`

Hands the clicked row's id to the panel's row gate, which asks about unsaved changes and records the station.
The click hands over the row's item, so no control decides the request.

## `private void QRosterApply(FrameworkElement container, object item, string? _)`

Fills one roster row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The epithet leads with an en space, as its string format did.

## `private void QFavoriteScribeObserve(bool scribe)`

Hears the rail's mode toggle and swaps the read view and the editor through the shared panel.
`scribe` is true for the editor and false for the read view.

## `private void QFavoriteStoreObserve()`

Hands the rail's store notice to the editor's save gate.

## `private void QFavoriteBinObserve()`

Hands the rail's delete notice to the panel's delete gate.
