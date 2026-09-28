# QLibrary.cs

## `internal sealed class QLibrary`

Drives the library panel: what it is made of, and what it forwards.
Every branch it once carried lives in `LLibrary` and the `CPanel` it holds.
The search, the ordering, the index, the read-only display, the editor and the import are all wired here.
The panel itself is the veneer's `PLibrary` page, which the window places.

## `internal QLibrary(UserControl surface)`

Takes the page the window pulled under the contract ID `PLibrary`.
It adds the print and export command bindings to the page and points the two buttons at those commands.
It ties both droppers to their popups, sets every icon, and subscribes every click and the search field.

## `private Border QOrder`

Each named part is pulled from the page by its contract ID on every read.
`QLibraryDisplay` is internal, because the window scrolls the display to a sense.

## `internal void QLibraryAttach(PWindow host)`

Puts the panel to work on the panel factory, which builds its controller over the engine's ports.
It builds the controller with the seams the panel answers through and subscribes to its notices.
The window fills it when it restores the stored ordering, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.
The print and portrait command bindings live in the constructor, and their check answers false until the controller exists.

## `private void QLibraryIndexAttach(ItemsControl view, FrameworkElement empty)`

Builds the entry list over the page's controls and refills it whenever the panel announces new rows.
The list is medium, so it moved here from the controller when the controller was sealed.

## `private void QLibraryIndexShow()`

Reads the rows and hands them to the list as an answered request.
The list counts what it holds, so the empty notice needs no count kept here.

## `private void QLibrarySieveShow(FrameworkElement mark)`

Shows the mark on the sieve button while the vista hides any language.

## `internal async void QLibraryVistaRestore()`

The atelier starts the tab's vistas from the stored posture, so no vista crosses the veneer.
Attaches the observers that carry each announcement onto the dispatcher.
The dropdown lists the shared entry orderings, and the mark is drawn from the controller's verdict.
The flags are loaded before the first rows are built, because a row reads its flag at construction.

## `internal bool QLibraryDraftFinish(bool store)`

Carries the window's exit answer down to the editor this panel owns.
The panel holds no draft of its own, so it only passes the answer along.
What the editor answers is passed back up, because a store the engine refused must not close the window.

## `internal bool QLibraryChangeCheck()`

Whether the editor holds modifications that have not been stored, as the controller reads it.
That is what the window asks before the workspace changes or the program closes.

## `internal bool QLibraryLeaveConfirm()`

The question the window puts before a jump that lands on the library the user is already writing in.
The panel asks it through the envoy only when it found changes.

## `internal long QLibraryVoyageRead()`

The entry the panel shows, as the station the window records before a jump away.

## `internal void QIndexEntryShow(long id)`

Puts the whole right-hand side on one entry, for a jump the window makes from another panel.
It asks nothing, because the window asks before it jumps.

## `private bool QLibraryShownCheck()`

The shown seam: whether this tab is the one in front, which only the page knows.

## `internal void QLibraryClose()`

Stops the panel: the editor is shut down and the shared display releases its playback.

## `private void QLibraryModeUpdate()`

Writes the mode and the enablement the controller holds into the eight controls that show them.

## `private string? QLibraryMarkupOpen()`

The path seam of the import: the file the reader picked, or null for a cancelled pick.

## `private IReadOnlyList<CSCustomsRow>? QLibraryCustomsShow(IReadOnlyList<CMarkupEntry> entries)`

The customs seam of the import: the reader chooses how each entry enters before anything is written.
A cancelled window answers null.

## `private void QLibraryOmissionShow(IReadOnlyList<CMarkupOmission> omissions)`

The omission seam of the import: what the import could not place, shown after the index holds the entries.
An import that dropped nothing shows nothing.

## `private void QOrderHandle(object sender, RoutedEventArgs e)`

A clicked order row closes the dropdown and sets the ordering its tag carries.
The row's enum is read here, so no ordering is spelled or parsed in the veneer.

## `private void QSieveHandle(object sender, RoutedEventArgs e)`

A clicked language row sets the filter its list now stands for, then redraws the mark at once.

## `private void QLibraryRowSelect(QIndexItem? item)`

Records the station, then puts the panel on the clicked row's entry.
The click hands over the row's item, so no control decides the request.
A sender without a row selects none, which leaves the panel on no entry.

## `private void QIndexApply(FrameworkElement container, object item, string? change)`

Fills one index row through the shared index fill, then subscribes its click once.
The click is removed first, so a refill never doubles it.

## `private async void QLibraryMarkupHandle(object sender, RoutedEventArgs e)`

The import button: the controller runs the whole import and asks each question through the three seams.
Import belongs on this panel rather than the editor.
What arrives is any number of entries, none of them the one being written.

## `private void QLibraryPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
The controller answers, so no control state is read.

## `private async void QLibraryPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The window asks for the ticket and the controller names the vista, so nothing is read back from the screen.

## `private async void QLibraryPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.

## Inline notes

### `QLibraryEditor.PEditorAttach(host, _lLibrary.LLibraryEditor, lectern);`

The editor opens on no entry, and this panel puts it on one when the reader asks to write.
The editor deportment was made for this panel, so its held work is told apart from the input panel's.
A store may have changed the headword the index lists and the text the display shows.
The engine announces it through the vista, so both are read again from what was written.
It is the same announcement whether the store happened in this panel's editor or in another tab.

## `internal void QLibraryVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void QLibraryRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void QLibraryAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `private void QLibraryUndoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void QLibraryRedoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void QLibraryChronicleUpdate()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.
