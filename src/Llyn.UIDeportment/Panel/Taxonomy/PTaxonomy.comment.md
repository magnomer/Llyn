# PTaxonomy.cs

## `public partial class PTaxonomy : UserControl`

The taxonomy panel as a control: what it is made of, and when it starts and stops.
Browsing itself lives in the file beside this one.
That is the tag search, the tag ordering, the tag catalog and the entries under a tag.
The reader and editor stand beside them.

## `public PTaxonomy()`

Loads the panel's markup from the Veneer and wears it as its content.
The markup's name scope is copied onto the panel, so its named parts answer `FindName`.
It adds the print and export command bindings and points the two buttons at those commands.
It ties both droppers to their popups, sets every icon, and subscribes every click and search field.

## `private Border PFunnel`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal void PTaxonomyAttach(PWindow host)`

Puts the panel to work through its deportment and its editor.
The window deportment builds both over the engine's ports.
It binds its lists, attaches their row fills, and reads nothing yet.
The window fills it when it restores the stored ordering, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.

## `internal void PTaxonomyReset()`

Puts the panel back on the workspace open now.
No tag is chosen, nothing is selected, the editor is closed, and the tag catalog is re-read.
A different workspace has its own tags.
So the tag this panel stood on may not exist in the one now open.

## `internal bool PTaxonomyDraftFinish(bool store)`

Carries the window's exit answer down to the editor this panel owns.
The panel holds no draft of its own, so it only passes the answer along.
What the editor answers is passed back up, because a store the engine refused must not close the window.

## `internal bool PTaxonomyChangeCheck()`

Whether the editor holds modifications that have not been stored.
That is what the window asks before the workspace changes or the program closes.

## `internal void PTaxonomyClose()`

Stops the panel: the editor is shut down and the reader releases its playback.

## `private void PTaxonomyPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void PTaxonomyPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The panel names only the id it is showing, and the engine builds the page from stored rows.
Nothing is read back from the screen.

## `private async void PTaxonomyPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.
Nothing is read back from the screen.

## Inline notes

### `private PWindow _pTaxonomyHost = null!;`

The window this panel sits in.
It is who reports a read that failed.
It also asks the question put before unsaved work would be lost.

## `internal void PTaxonomyVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void PTaxonomyRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void PTaxonomyAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `private void PTaxonomyUndoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void PTaxonomyRedoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void PTaxonomyChronicleUpdate()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private void PTaxonomyModeUpdate()`

Paints the mode from the shared panel state.
No control's visibility stands in for the mode any more.
The trail pair shows with the reader and the chronicle pair with the editor.
