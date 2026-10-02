# QTaxonomy.cs
Hash: `882d8199eea33655`

## `internal sealed partial class QTaxonomy`

Drives the taxonomy panel.
It owns what the panel is made of, and when it starts and stops.
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

## `internal void QTaxonomyIntroduce(QWindow host)`

Puts the panel to work through the Conduct taxonomy it builds, which builds its own editor.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The view wraps that editor for its editor page.
The lectern follows the panel, whose loads and clears reach the display's area, never the veneer.
It binds its lists, attaches their row fills, and reads nothing yet.
The Tag list answers the area's rows event, and the entry list its own panel's.
The window fills it when it restores the stored ordering, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.

## `internal void QTaxonomyExitRefine()`

Releases the editor's player, so none outlives the window.
It calls no gate.
The window's exit gate `CAtelierClose` closes the editor and stops its playback in Conduct.

## `private void QTaxonomyPressRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void QTaxonomyPressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The panel names only the id it is showing, and the engine builds the page from stored rows.
Nothing is read back from the screen.

## `private async void QTaxonomyPortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The gate asks for the file and the format through the envoy.
The engine writes the document from stored rows.
Nothing is read back from the screen.

## Inline notes

### `private QWindow _qTaxonomyHost = null!;`

The window this panel sits in.
It is who reports a read that failed.
It also asks the question put before unsaved work would be lost.

## `internal void QTaxonomyVoyageRefine(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the panel only shows what it is told.

## `private void QTaxonomyRetreatObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station.

## `private void QTaxonomyAdvanceObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station.

## `private void QTaxonomyUndoObserve(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void QTaxonomyRedoObserve(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void QTaxonomyChronicleRefine()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private void QTaxonomyModeRefine()`

Paints the mode from the shared panel state.
No control's visibility stands in for the mode any more.
The trail pair shows with the reader and the chronicle pair with the editor.
