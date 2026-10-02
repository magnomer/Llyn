# QFavoriteBrowse.cs
Hash: `cda66d0212d2518f`

## `internal sealed partial class QFavorite`

Browsing the marked entries: the search, the ordering, the roster, and the switch between reading and editing.
The panel shell lives in the file beside this one.

## `private async void QFavoriteWorkspaceRefine()`

Answers the area's workspace event, which has already closed the chosen entry.
It loads the flags of the workspace that moved, so the rows built next carry the right ones.

## `private void QRecallObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement re-lists the roster.
Typing selects nothing and marks nothing.

## `private void QSeriesObserve(object sender, RoutedEventArgs e)`

Takes the chosen ordering from the clicked row and hands it to the vista, then closes the dropdown.
The vista saves it and announces it, and the announcement re-lists the roster.

## `private void QSeriesDropperRefine()`

Unticks the ordering button, which closes its dropdown.

## `private void QStrainerObserve(object sender, RoutedEventArgs e)`

The ticked languages are read off the clicked box's menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

## `internal async void QFavoriteVistaRefine()`

Answers the workspace opening, after `CFavorite` has restored its vista and attached its observers.
The dropdown mark and the filter mark are drawn from the vista first.
The flags are loaded before any row is built.
The language menu is then built from the languages that load answers.
The roster is then listed once, so a row keeps the flag it was built with.
The search text stands in the box already, since the restore carried it into the fresh vista.
Its one request is `CFavoriteRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QStrainerBuild(IReadOnlyList<string> languages)`

Builds the language menu from the languages the flag load answered, ticking the vista's hidden ones.

## `private void QStrainerRefine()`

Shows the filter mark while the vista hides any language.

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

## `private void QFavoriteViewerObserve(object sender, RoutedEventArgs e)`

Swaps the editor for the read view through the shared panel.

## `private void QFavoriteScribeObserve(object sender, RoutedEventArgs e)`

Swaps the read view for the editor through the shared panel.

## `private void QFavoriteStoreObserve(object sender, RoutedEventArgs e)`

Hands the store click to the editor's save gate.

## `private void QFavoriteBinObserve(object sender, RoutedEventArgs e)`

Hands the delete click to the panel's delete gate.
