# PRepertoire.xaml.cs

## `public partial class PRepertoire : UserControl`

The Repertoire panel: the view of the shared stock of usage contexts itself.
A Situation is independent data owned by nothing, so this panel is not a view of one Entry's contexts.
It holds the host and the engine the browsing side calls, and nothing else.
The browsing behavior lives in `PRepertoireBrowse.cs` and the editing in `PRepertoireEditor.cs`, one file per responsibility.
The held draft the editor writes into lives in `PRepertoireHold.cs`, apart from the controls it reads.
It merges the card's picture and video row templates, answering their clicks as `PImageHost` and `PVideoHost`.
Those answers live in `PRepertoireDialog.cs`.

## `internal void PRepertoireAttach(PWindow host)`

Builds the panel's deportment and its editor over the engine's ports, and wires the desk's notices.
Binds the panel to the window it asks for confirmations and panel switches through.
It binds its lists, the editor's media rows among them, and subscribes to the engine, and reads nothing yet.
It attaches the entry display and editor to the same host and engine, so an Entry is read and written.
The engine's change notices drive the mode, and its row notices drive the two lists.
Its scenario and situation notices paint the sheet, and its failures reach the window.
A dropped inquest empties the inquest box and the language menu.
The print and export commands are bound in code, so their checks never run before the engine exists.
The window fills the situation catalog when it restores the stored ordering.
Every change after that arrives as an announcement.
The panel is current whether or not its tab is in front.

## `private bool PRepertoireShownCheck()`

Whether the panel is on screen, so the engine knows when a notice needs painting.

## `private bool PRepertoireDiscardConfirm(Func<bool, bool> finish)`

Asks the window whether unsaved work may be dropped before the engine moves on.
A save runs the finish the deportment handed in, so the deportment decides what follows it.

## `private bool PRepertoireRemovalConfirm(int usage)`

Asks the window whether a Situation used this many times may be removed.

## `private void POccurrenceDraftShow(LDraft draft)`

Shows the occurrence's held entry in the display.

## `private void PRepertoireModeUpdate()`

Paints the mode the engine decides: which page shows, which toggle is checked, which button is live.
The scenario is live only while its desk runs.
It ends by refreshing the rail's undo and redo.

## `internal void PRepertoireReset()`

Drops both selections through the deportment and reads the catalog again, for when the workspace underneath changed.

## `internal bool PRepertoireChangeCheck()`

Whether the editor holds work nothing has saved yet.
The engine answers it against the held draft rather than the panel against a copy of a stored record.
The window asks before anything can leave the panel.
The deportment asks the occurrence side and the atlas side, and each answers only while it edits.

## `internal bool PRepertoireDraftFinish(bool store)`

Ends whichever draft is in front, committing it or discarding it.
The deportment finishes the entry editor's draft while it is in front, and the held situation otherwise.

## `internal void PRepertoireClose()`

Closes the popups the panel owns, so neither outlives the window.

## `private void PRepertoirePressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: an entry is read, or a situation is read.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void PRepertoirePressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, or else the situation being read, as the engine portrays it.
The deportment picks the page from the side it shows, and the engine builds it from stored rows.
Nothing is read back from the screen.

## `private void PRepertoirePortraitCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the export button is live: exactly when an entry is read in the display.
Print may also act on the other page this panel reads, but export acts on entries alone.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void PRepertoirePortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.
Nothing is read back from the screen.
