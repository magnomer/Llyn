# QTenor.cs

## `internal sealed partial class QTenor`

The tenor panel: the workspace browsed by the Registers its cards carry.
It is the taxonomy panel's shape read through a different question, so it holds the same three columns.
It owns a reader and an editor over one Entry, and answers the engine rather than its own visibility.
The panel itself is the veneer's `PTenor` page, which the window places.

## `internal QTenor(UserControl surface)`

Takes the page the window pulled under the contract ID `PTenor`.
It adds the print and export command bindings to the page and points the two buttons at those commands.
It ties both droppers to their popups, sets every icon, and subscribes every click and search field.

## `private Border QDegree`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QTenorAttach(PWindow host)`

Puts the panel to work through the Conduct tenor panel, which the forge builds with its own editor.
The view wraps that editor for its editor page.
The lectern follows the panel, whose loads and clears reach the display's area, never the veneer.
Both catalogs get their row fills here, where their sources are set.

## `internal void QTenorReset()`

Empties the panel and rebuilds the catalog, for a workspace that has just moved.

## `internal bool QTenorDraftFinish(bool store)`

Stores or discards a standing draft on the way out of the application.

## `internal bool QTenorChangeCheck()`

Whether the editor is the shown side and is holding a change, so leaving would lose work.

## `internal void QTenorClose()`

Stops listening and closes the reader and the editor.

## `private void QTenorPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print and export buttons are live: exactly when an entry is read in the display.
An editor on screen prints nothing, because what is printed is what is read.
The buttons follow this answer on their own, so no panel state has to switch them.

## `private async void QTenorPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The panel names only the id it is showing, and the engine builds the page from stored rows.
Nothing is read back from the screen.

## `private async void QTenorPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.
Nothing is read back from the screen.

## `internal void QTenorVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void QTenorRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void QTenorAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `private void QTenorUndoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void QTenorRedoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void QTenorChronicleUpdate()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private void QTenorModeUpdate()`

Paints the mode from the shared panel state.
No control's visibility stands in for the mode any more.
The trail pair shows with the reader and the chronicle pair with the editor.
