# PLibrary.cs

## `public class PLibrary : UserControl`

The library panel as a control: what it is made of, and what it forwards.
Every branch it once carried lives in `LLibrary` and the `LPanel` it holds.
The panel news its deportment, forwards each handler to it, and writes its controls when a notice arrives.
The search, the ordering, the index, the read-only display, the editor and the import are all wired here.

## `public PLibrary()`

Loads the panel's markup from the Veneer and wears it as its content.
The markup's name scope is copied onto the panel, so its named parts answer `FindName`.
It adds the print and export command bindings and points the two buttons at those commands.
It ties both droppers to their popups, sets every icon, and subscribes every click and the search field.
It attaches the index row fill.

## `private Border POrder`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.
`PDisplay` is internal, because the window scrolls the display to a sense.

## `internal void PLibraryAttach(PWindow host)`

Puts the panel to work on the window deportment, which builds its deportment over the engine's ports.
It news the deportment with the seams the panel answers through and subscribes to its notices.
The window fills it when it restores the stored ordering, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.
The print and portrait command bindings live in the constructor, and their check answers false until the deportment exists.

## `internal async void PLibraryVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Hands the vista to the deportment and attaches the observers that carry each announcement onto the dispatcher.
The dropdown lists the shared entry orderings, and the deportment draws the filter mark from the vista.
The flags are loaded before the first rows are built, because a row reads its flag at construction.

## `internal bool PLibraryDraftFinish(bool store)`

Carries the window's exit answer down to the editor this panel owns.
The panel holds no draft of its own, so it only passes the answer along.
What the editor answers is passed back up, because a store the engine refused must not close the window.

## `internal bool PLibraryChangeCheck()`

Whether the editor holds modifications that have not been stored, as the deportment reads it.
That is what the window asks before the workspace changes or the program closes.

## `internal bool PLibraryLeaveConfirm()`

The question the window puts before a jump that lands on the library the user is already writing in.
The deportment asks it through the discard seam only when it found changes.

## `internal long PLibraryVoyageRead()`

The entry the panel shows, as the station the window records before a jump away.

## `internal void PIndexEntryShow(long id)`

Puts the whole right-hand side on one entry, for a jump the window makes from another panel.
It asks nothing, because the window asks before it jumps.

## `private bool PLibraryShownCheck()`

The shown seam: whether this tab is the one in front, which only the control knows.

## `private bool PLibraryDiscardConfirm()`

The discard seam: the window's leave dialog over the editor's finish, asked only when the deportment found changes.

## `internal void PLibraryClose()`

Stops the panel: the editor is shut down and the shared display releases its playback.

## `private void PLibraryModeUpdate()`

Writes the mode and the enablement the deportment holds into the eight controls that show them.

## `private string? PLibraryMarkupOpen()`

The path seam of the import: the file the reader picked, or null for a cancelled pick.

## `private IReadOnlyList<LMarkupIntake>? PLibraryCustomsShow(LMarkupCargo cargo)`

The customs seam of the import: the reader chooses how each entry enters before anything is written.
A cancelled window answers null.

## `private void PLibraryOmissionShow(IReadOnlyList<LMarkupOmission> omissions)`

The omission seam of the import: what the import could not place, shown after the index holds the entries.
An import that dropped nothing shows nothing.

## `private async void PLibraryMarkupHandle(object sender, RoutedEventArgs e)`

The import button: the deportment runs the whole import and asks each question through the three seams.
Import belongs on this panel rather than the editor.
What arrives is any number of entries, none of them the one being written.

## `private void PLibraryPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
The deportment answers, so no control state is read.

## `private async void PLibraryPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The window asks for the ticket and the deportment names the vista, so nothing is read back from the screen.

## `private async void PLibraryPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.

## Inline notes

### `PEditor.PEditorAttach(host, _lLibrary.LLibraryEditor);`

The editor opens on no entry, and this panel puts it on one when the reader asks to write.
The editor deportment was made for this panel, so its held work is told apart from the input panel's.
A store may have changed the headword the index lists and the text the display shows.
The engine announces it through the vista, so both are read again from what was written.
It is the same announcement whether the store happened in this panel's editor or in another tab.

## `internal void PLibraryVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void PLibraryRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void PLibraryAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `private void PLibraryUndoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void PLibraryRedoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void PLibraryChronicleUpdate()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private void PIndexApply(FrameworkElement container, object item, string? change)`

Fills one index row through the shared index fill, then subscribes its click once.
The click is removed first, so a refill never doubles it.
