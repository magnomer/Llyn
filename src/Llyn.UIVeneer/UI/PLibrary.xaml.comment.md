# PLibrary.xaml.cs

## `public partial class PLibrary : UserControl`

The library panel as a control: what it is made of, and what it forwards.
Every branch it once carried lives in `LLibrary` and the `LPanel` it holds.
The panel news its deportment, forwards each handler to it, and writes its controls when a notice arrives.
The search, the ordering, the index, the read-only display, the editor and the import are all wired here.

## `internal void PLibraryAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine`, the workspace the window opened.
It news the deportment with the seams the panel answers through and subscribes to its notices.
The window fills it when it restores the stored ordering, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.
The print and portrait command bindings are added last, so no can-execute query ever meets a deportment not yet built.

## `internal async void PLibraryVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Hands the vista to the deportment and attaches the observers that carry each announcement onto the dispatcher.
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

## `private bool PLibraryShownCheck()`

The shown seam: whether this tab is the one in front, which only the control knows.

## `private bool PLibraryDiscardConfirm()`

The discard seam: the window's leave dialog over the editor's finish, asked only when the deportment found changes.

## `internal void PLibraryClose()`

Stops the panel: the editor is shut down and the shared display releases its playback.

## `private void PIndexUpdate()`

Refills the index from the rows the deportment reads, spliced so the list keeps its scroll position.

## `private void PLibraryModeUpdate()`

Writes the mode and the enablement the deportment holds into the six controls that show them.

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

### `PEditor.PEditorAttach(host, engine, "Library", null);`

The editor opens on no entry: this panel puts it on one when the reader asks to write.
The origin names this panel, so its held work is told apart from the input panel's.
A store may have changed the headword the index lists and the text the display shows.
The engine announces it through the vista, so both are read again from what was written.
It is the same announcement whether the store happened in this panel's editor or in another tab.
