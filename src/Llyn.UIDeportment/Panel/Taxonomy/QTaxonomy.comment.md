# QTaxonomy.cs

## `internal sealed partial class QTaxonomy`

Drives the taxonomy panel: what it is made of, and when it starts and stops.
Browsing itself lives in the file beside this one.
That is the tag search, the tag ordering, the tag catalog and the entries under a tag.
The reader and editor stand beside them.
The panel itself is the veneer's `PTaxonomy` page, which the window places.

## `internal QTaxonomy(UserControl surface)`

Takes the page the window pulled under the contract ID `PTaxonomy`.
It adds the print and export command bindings to the page and points the two buttons at those commands.
It ties both droppers to their popups, sets every icon, and subscribes every click and search field.

## `private Border QFunnel`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QTaxonomyAttach(PWindow host)`

Puts the panel to work through the Conduct taxonomy, which the forge builds with its own editor.
The view wraps that editor for its editor page.
The lectern follows the panel, whose loads and clears reach the display's area, never the veneer.
It binds its lists, attaches their row fills, and reads nothing yet.
The window fills it when it restores the stored ordering, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.

## `internal void QTaxonomyReset()`

Puts the panel back on the workspace open now.
No tag is chosen, nothing is selected, the editor is closed, and the tag catalog is re-read.
A different workspace has its own tags.
So the tag this panel stood on may not exist in the one now open.

## `internal bool QTaxonomyDraftFinish(bool store)`

Carries the window's exit answer down to the editor this panel owns.
The panel holds no draft of its own, so it only passes the answer along.
What the editor answers is passed back up, because a store the engine refused must not close the window.

## `internal bool QTaxonomyChangeCheck()`

Whether the editor holds modifications that have not been stored.
That is what the window asks before the workspace changes or the program closes.

## `internal void QTaxonomyClose()`

Stops the panel: the editor is shut down and the reader releases its playback.

## `private void QTaxonomyPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void QTaxonomyPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The panel names only the id it is showing, and the engine builds the page from stored rows.
Nothing is read back from the screen.

## `private async void QTaxonomyPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.
Nothing is read back from the screen.

## Inline notes

### `private PWindow _qTaxonomyHost = null!;`

The window this panel sits in.
It is who reports a read that failed.
It also asks the question put before unsaved work would be lost.

## `internal void QTaxonomyVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void QTaxonomyRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void QTaxonomyAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `private void QTaxonomyUndoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void QTaxonomyRedoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void QTaxonomyChronicleUpdate()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private void QTaxonomyModeUpdate()`

Paints the mode from the shared panel state.
No control's visibility stands in for the mode any more.
The trail pair shows with the reader and the chronicle pair with the editor.
