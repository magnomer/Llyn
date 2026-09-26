# PTenor.cs

## `public partial class PTenor : UserControl`

The tenor panel: the workspace browsed by the Registers its cards carry.
It is the taxonomy panel's shape read through a different question, so it holds the same three columns.
It owns a reader and an editor over one Entry, and answers the engine rather than its own visibility.

## `public PTenor()`

Loads the panel's markup from the Veneer and wears it as its content.
The markup's name scope is copied onto the panel, so its named parts answer `FindName`.
It adds the print and export command bindings and points the two buttons at those commands.
It ties both droppers to their popups, sets every icon, and subscribes every click and search field.

## `private Border PDegree`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal void PTenorAttach(PWindow host)`

Binds the panel to the window, builds its deportment and its editor over the engine's ports, and wires their notices.
Both catalogs get their row fills here, where their sources are set.

## `internal void PTenorReset()`

Empties the panel and rebuilds the catalog, for a workspace that has just moved.

## `internal bool PTenorDraftFinish(bool store)`

Stores or discards a standing draft on the way out of the application.

## `internal bool PTenorChangeCheck()`

Whether the editor is the shown side and is holding a change, so leaving would lose work.

## `internal void PTenorClose()`

Stops listening and closes the reader and the editor.

## `private void PTenorPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print and export buttons are live: exactly when an entry is read in the display.
An editor on screen prints nothing, because what is printed is what is read.
The buttons follow this answer on their own, so no panel state has to switch them.

## `private async void PTenorPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The panel names only the id it is showing, and the engine builds the page from stored rows.
Nothing is read back from the screen.

## `private async void PTenorPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.
Nothing is read back from the screen.

## `internal void PTenorVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void PTenorRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void PTenorAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `private void PTenorUndoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void PTenorRedoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void PTenorChronicleUpdate()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private void PTenorModeUpdate()`

Paints the mode from the shared panel state.
No control's visibility stands in for the mode any more.
The trail pair shows with the reader and the chronicle pair with the editor.
