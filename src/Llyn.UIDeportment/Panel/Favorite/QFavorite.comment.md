# QFavorite.cs
Hash: `92aefbea141e7b3f`

## `internal sealed partial class QFavorite`

Drives the favorites panel: what it is made of, and when it starts and stops.
Browsing lives in the file beside this one.
That is the search, the ordering, the roster and the switch between reading and editing.
The panel itself is the veneer's `QFavorite` page, which the window places.

## `internal QFavorite(UserControl surface)`

Takes the page the window pulled under the contract ID `QFavorite`.
It adds the print and export command bindings to the page and points the two buttons at those commands.
It ties both droppers to their popups, sets every icon, and subscribes every click and search field.

## `private Border QSeries`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QFavoriteIntroduce(QWindow host)`

Puts the panel to work through its Conduct `CFavorite`, which it builds over the atelier.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The editor view and the lectern wrap the entry editor that `CFavorite` owns.
The lectern follows the panel, whose loads and clears reach the display's area, never the veneer.
It builds the ordering menu once, from the orderings `CFavorite` offers.
It binds its lists, subscribes the area's events, attaches their row fills, and reads nothing yet.
`QFavoriteVistaRefine` fills it when the workspace opens, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.

Every mark and unmark is announced by the engine, wherever the star was clicked.
That is how a row leaves the roster the moment the entry it stands on is unmarked.

## `internal void QFavoriteExitRefine()`

Releases the editor's recording player when the window closes.
The editor and the display stop through `CFavorite`'s closure, which the atelier's exit gate runs.

## `private void QFavoritePressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
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

## `internal void QFavoriteVoyageShow(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the panel only shows what it is told.

## `private void QFavoriteRetreatObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station.

## `private void QFavoriteAdvanceObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station.

## `private void QFavoriteUndoObserve(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void QFavoriteRedoObserve(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void QFavoriteChronicleUpdate()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private void QFavoriteModeUpdate()`

Paints the mode from the shared panel state.
No control's visibility stands in for the mode any more.
The trail pair shows with the reader and the chronicle pair with the editor.
