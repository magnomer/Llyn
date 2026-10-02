# QPhonology.cs

## `internal sealed class QPhonology`

Drives the phonology panel: what it is made of, and what it forwards.
Every branch it once carried lives in Conduct's `CPhonology` and the `CPanel` it holds.
The search, the ordering, the inventory, the read-only display and the editor are all wired here.
The panel itself is the veneer's `QPhonology` page, which the window places.
It names no Core type, since its rows arrive as `CCatalogPronunciation`.

## `internal QPhonology(UserControl surface)`

Takes the page the window pulled under the contract ID `QPhonology`.
It builds the articulation driver over the nested `QArticulation` page, as a parent driver builds a nested one.
It adds the print and export command bindings to the page and points the two buttons at those commands.
It ties the droppers to their popups, sets every icon, and attaches the row fills.
Row clicks are taken on the inventory, and every button and search field is subscribed here.

## `private Rectangle QArticulationSeam`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QPhonologyIntroduce(QWindow host)`

Puts the panel to work through the Conduct phonology panel it builds, which builds its own editor.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The view wraps that editor for its editor page.
The lectern follows the panel, whose loads and clears reach the display's area, never the veneer.
The sequence menu is built once from the orders Conduct offers.
The inventory and the filter mark both repaint whenever the panel raises its rows.
The window fills it when it restores the stored ordering, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.

## `internal async void QPhonologyVistaRefine()`

Answers the workspace opening, after the Conduct panel restored its vista and attached its own observers.
It marks the chosen order and the filter, then awaits the flags.
The flags are loaded before the first rows are built, because a row reads its flag at construction.
The filter menu lists the languages the flag load answers.
Its one request is `CPhonologyRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QLensListRefine(IReadOnlyList<string> languages)`

Fills the filter menu with the languages the flag load answered, each ticked by the panel's filter.

## `private async void QPhonologyWorkspaceRefine()`

Answers `CPhonologyWorkspaceChanged` by drawing the flags of the new workspace's languages.
Once the flags are in, it repaints the inventory, whose rows carry a flag.
A failed load throws before the repaint, as the old load before the entry close did.
Its one request is `CPhonologyRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private bool QPhonologyShownCheck()`

The shown seam: whether this tab is the one in front, which only the page knows.

## `internal void QPhonologyExitRefine()`

Releases the editor's recording player when the window closes.
The editor and the playback stop in Conduct, through the close the panel registered with the workspace.

## `private void QInventoryRefine()`

Refills the inventory from the rows the deportment reads, spliced so the list keeps its scroll position.

## `private void QInventoryRefine(IReadOnlyList<CCatalogPronunciation> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QPhonologyModeRefine()`

Writes the mode and the enablement the deportment holds into the eight controls that show them.
The trail pair shows while reading and the chronicle pair while writing.

## `private void QPhonologyPressRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
The deportment answers, so no control state is read.
The binding can be asked before the deportment is built, and then answers false.

## `private async void QPhonologyPressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The gate asks for the ticket through the envoy and names the vista.
Nothing is read back from the screen.

## `private async void QPhonologyPortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The gate asks for the file and the format through the envoy.
The engine writes the document from stored rows.

## Inline notes

### `_qArticulation.QArticulationIntroduce(`

The aid is given both fields a phonetic character is typed into.
The search comes first, because a reader who opens the charts with nothing focused is looking a pronunciation up.
The editor's field takes over as soon as the reader focuses it.
The editor's field is pulled from the editor's Veneer by its contract ID `PPronunciationField`.
It is named once here rather than looked up whenever a character is chosen.

## `internal void QPhonologyVoyageRefine(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the panel only shows what it is told.

## `private void QPhonologyRetreatObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station.

## `private void QPhonologyAdvanceObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station.

## `private void QPhonologyUndoObserve(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void QPhonologyRedoObserve(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void QPhonologyChronicleRefine()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private void QPhonologyViewerObserve(object sender, RoutedEventArgs e)`

Hands the reading mode to the scribe gate, so the viewer button needs no comparison with its sender.

## `private void QPhonologyScribeObserve(object sender, RoutedEventArgs e)`

Hands the writing mode to the scribe gate.

## `private void QSequenceObserve(object sender, RoutedEventArgs e)`

Hands the order row the reader picked to the order gate, then folds the menu away.

## `private void QSequenceRefine()`

Folds the sequence menu away.

## `private void QLensObserve(object sender, RoutedEventArgs e)`

Hands the filter the reader ticked to the filter gate.
The mark repaints when the panel raises its rows after the change.

## `private void QArticulationFoldRefine(object sender, RoutedEventArgs e)`

Shows the aid and its seam while the fold toggle is checked, and hides both otherwise.
The toggle is the only state, so the handler reads it and keeps nothing.
