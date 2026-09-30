# QLibrary.cs

## `internal sealed class QLibrary`

Drives the library panel: what it is made of, and what it forwards.
Every branch it once carried lives in the Conduct `CLibrary` and the `CPanel` it holds.
The search, the ordering, the index, the read-only display, the editor and the import are all wired here.
The panel itself is the veneer's `PLibrary` page, which the window places.

## `internal QLibrary(UserControl surface)`

Takes the page the window pulled under the contract ID `PLibrary`.
It adds the print and export command bindings to the page and points the two buttons at those commands.
It ties both droppers to their popups, sets every icon, and subscribes every click and the search field.

## `private Border QOrder`

Each named part is pulled from the page by its contract ID on every read.
`QLibraryLectern` is internal, because the window scrolls the lectern's card to a sense.

## `internal void QLibraryIntroduce(PWindow host)`

Puts the panel to work through its Conduct `CLibrary`, which it builds over the atelier.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The Conduct builds its own entry editor, which this panel wraps for the editor view and the lectern.
The lectern follows the panel, and the panel's notices reach its own controls.
It builds the ordering menu once, from the orderings `CLibrary` offers.
It subscribes the area's events and reads nothing yet.
`QLibraryVistaRefine` fills it when the workspace opens, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.
The print and portrait command bindings live in the constructor, and their check answers false until the controller exists.

## `private void QLibraryIndexIntroduce(ItemsControl view, FrameworkElement empty)`

Builds the entry list over the page's controls and refills it whenever the panel announces new rows.
The list is medium, so it moved here from the controller when the controller was sealed.

## `private void QLibraryIndexRefine()`

Reads the rows and hands them to the list with the library's empty verdict.
Conduct counts the rows, so the empty notice needs no count kept here.

## `private void QLibrarySieveRefine()`

Shows the mark on the sieve button while the vista hides any language.

## `internal async void QLibraryVistaRefine()`

Answers the opening of a workspace, after the atelier has started the tab's vistas.
It paints the order and the filter mark, then waits for the flags.
The flags are loaded before the first rows are built, because a row reads its flag at construction.
It builds the filter menu from the languages the load answers, then reads the rows once.
The observers that carry each announcement live in `CLibrary`, so this driver holds none.

## `private void QSieveBuild(IReadOnlyList<string> languages)`

Builds the filter menu from the languages the flag load answered.

## `private async void QLibraryWorkspaceRefine()`

Answers the area's workspace notice by loading the flags of the new workspace.
The entry was already closed in Conduct.

## `private bool QLibraryShownCheck()`

The shown seam: whether this tab is the one in front, which only the page knows.

## `internal void QLibraryExitRefine()`

Releases the editor's recording player when the window closes.
The editor and the display stop through `CLibrary`'s closure, which the atelier's exit gate runs.

## `private void QLibraryModeRefine()`

Writes the mode and the enablement the controller holds into the eight controls that show them.

## `private void QOrderObserve(object sender, RoutedEventArgs e)`

A clicked order row sets the ordering its tag carries, then closes the dropdown.
The row's enum is read here, so no ordering is spelled or parsed in the veneer.

## `private void QOrderRefine()`

Unchecks the order button, which closes its dropdown.

## `private void QSieveObserve(object sender, RoutedEventArgs e)`

A clicked language row sets the filter its list now stands for, then redraws the mark at once.

## `private void QLibraryIndexObserve(object sender, RoutedEventArgs e)`

Puts the panel on the clicked row's entry, and the gate records the station first.
The click hands over the row's item id, so no control decides the request.
A sender without a row selects none, which leaves the panel on no entry.

## `private void QLibraryRowRefine(FrameworkElement container, object item, string? change)`

Fills one index row through the shared index fill, then subscribes its click once.
The click is removed first, so a refill never doubles it.

## `private void QInquiryObserve(object sender, TextChangedEventArgs e)`

The search field hands its text to the query gate.

## `private void QLibraryFreshObserve(object sender, RoutedEventArgs e)`

The new button asks the panel for a fresh entry.

## `private void QLibraryViewerObserve(object sender, RoutedEventArgs e)`

The viewer button asks the panel to leave writing.

## `private void QLibraryScribeObserve(object sender, RoutedEventArgs e)`

The scribe button asks the panel to start writing.

## `private void QLibraryStoreObserve(object sender, RoutedEventArgs e)`

The store button asks the editor to save the entry.

## `private void QLibraryBinObserve(object sender, RoutedEventArgs e)`

The bin button asks the panel to delete the chosen entry.

## `private async void QLibraryMarkupObserve(object sender, RoutedEventArgs e)`

The import button calls `CLibraryMarkupImport`, which asks every question itself.
The gate asks for the file, then the customs question, and shows the report through the envoy.
Import belongs on this panel rather than the editor.
What arrives is any number of entries, none of them the one being written.

## `private void QLibraryPressRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
The controller answers, so no control state is read.

## `private async void QLibraryPressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The gate asks for the ticket through the envoy and names the vista.
Nothing is read back from the screen.

## `private async void QLibraryPortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The gate asks for the file and the format through the envoy.
The engine writes the document from stored rows.

## Inline notes

### `QLibraryEditor.PEditorIntroduce(host, new QEditor(_cLibrary.CLibraryEditor));`

The editor opens on no entry, and this panel puts it on one when the reader asks to write.
The editor deportment was made for this panel, so its held work is told apart from the input panel's.
A store may have changed the headword the index lists and the text the display shows.
The engine announces it through the vista, so both are read again from what was written.
It is the same announcement whether the store happened in this panel's editor or in another tab.

## `internal void QLibraryVoyageRefine(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the panel only shows what it is told.

## `private void QLibraryRetreatObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station.

## `private void QLibraryAdvanceObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station.

## `private void QLibraryUndoObserve(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void QLibraryRedoObserve(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void QLibraryChronicleRefine()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.
