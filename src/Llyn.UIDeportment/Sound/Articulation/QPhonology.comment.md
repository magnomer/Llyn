# QPhonology.cs

## `internal sealed class QPhonology`

Drives the phonology panel: what it is made of, and what it forwards.
Every branch it once carried lives in `LPhonology` and the `LPanel` it holds.
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

## `internal void QPhonologyAttach(PWindow host)`

Puts the panel to work on the window deportment, which builds its deportment over the engine's ports.
It news the deportment with the seams the panel answers through and subscribes to its notices.
The window fills it when it restores the stored ordering, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.

## `internal async void QPhonologyVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Hands the vista to the deportment and attaches the observers that carry each announcement onto the dispatcher.
The flags are loaded before the first rows are built, because a row reads its flag at construction.

## `internal bool QPhonologyDraftFinish(bool store)`

Carries the window's exit answer down to the editor this panel owns.
The panel holds no draft of its own, so it only passes the answer along.
What the editor answers is passed back up, because a store the engine refused must not close the window.

## `internal bool QPhonologyChangeCheck()`

Whether the editor holds modifications that have not been stored, as the deportment reads it.
That is what the window asks before the workspace changes or the program closes.

## `private bool QPhonologyShownCheck()`

The shown seam: whether this tab is the one in front, which only the page knows.

## `internal bool QPhonologyLeaveConfirm()`

The panel's question before its unsaved work goes out of sight, asked by the window.

## `private bool QPhonologyDiscardConfirm()`

The leave seam: the window's leave dialog over the editor's finish, asked only when the deportment found changes.

## `internal void QPhonologyClose()`

Stops the panel: the editor is shut down and the shared display releases its playback.

## `private void QInventoryUpdate()`

Refills the inventory from the rows the deportment reads, spliced so the list keeps its scroll position.

## `private void QPhonologyModeUpdate()`

Writes the mode and the enablement the deportment holds into the eight controls that show them.
The trail pair shows while reading and the chronicle pair while writing.

## `private void QPhonologyPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
The deportment answers, so no control state is read.
The binding can be asked before the deportment is built, and then answers false.

## `private async void QPhonologyPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The window asks for the ticket and the deportment names the vista, so nothing is read back from the screen.

## `private async void QPhonologyPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.

## Inline notes

### `_qArticulation.QArticulationAttach(QProbe, QPhonologyEditor.PPronunciationField);`

The aid is given both fields a phonetic character is typed into.
The search comes first, because a reader who opens the charts with nothing focused is looking a pronunciation up.
The editor's field takes over as soon as the reader focuses it.
It is named once here rather than looked up whenever a character is chosen.

## `internal void QPhonologyVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void QPhonologyRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void QPhonologyAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `private void QPhonologyUndoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void QPhonologyRedoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void QPhonologyChronicleUpdate()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `internal long QPhonologyVoyageRead()`

The Entry the panel shows, read off the phonology panel as the station of this panel.

## `internal void QInventoryEntryShow(long id)`

Shows one Entry by id, for a jump the window makes from another panel.
It asks nothing, because the window asks before it jumps.

## `private void QArticulationFoldHandle(object sender, RoutedEventArgs e)`

Shows the aid and its seam while the fold toggle is checked, and hides both otherwise.
The toggle is the only state, so the handler reads it and keeps nothing.
