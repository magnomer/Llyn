# PCorpus.xaml.cs

## `public partial class PCorpus : UserControl`

The Corpus panel: the view of the shared stock of sentences itself.
An Example is independent data owned by nothing, so this panel is not a view of one Entry's sentences.
It holds the host and the engine the browsing side calls, and nothing else.
The browsing behavior lives in `PCorpusBrowse.cs` and the editing in `PCorpusEditor.cs`, one file per responsibility.
The held draft the editor writes into lives in `PCorpusHold.cs`, apart from the controls it reads.
The linking gesture over the transcript lives in `PCorpusMention.cs`.

## `internal void PCorpusAttach(PWindow host, LEngine engine)`

Binds the panel to the window it asks for confirmations and panel switches through.
It binds its lists and subscribes to the engine, and reads nothing yet.
It attaches the entry display and editor to the same host and engine, so an Entry is read and written.
The editor's change notice drives `PCorpusStore`, as it drives the save button of the tenor panel.
The editor is lent `PCorpusBackward` and `PCorpusForward` too, so the rail's undo and redo follow an open Entry.
The window fills the example catalog when it restores the stored ordering.
Every change after that arrives as an announcement.
The panel is current whether or not its tab is in front.

## `internal void PCorpusReset()`

Drops the selection and reads the catalog again, for when the workspace underneath changed.

## `internal bool PCorpusChangeCheck()`

Whether the editor holds work nothing has saved yet.
The engine answers it against the held draft rather than the panel against a copy of a stored record.
The window asks before anything can leave the panel.
The entry editor answers while it is in front, and the Example editor otherwise.

## `internal bool PCorpusDraftFinish(bool store)`

Ends whichever draft is in front, committing it or discarding it.
The entry editor finishes its own draft, and the Example editor finishes its own.

## `internal void PCorpusClose()`

Closes the popups the panel owns, so none outlives the window.

## `private void PCorpusPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: an entry is read, or a example is read.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void PCorpusPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, or else the example being read, as the engine portrays it.
The panel names only the id it is showing, and the engine builds the page from stored rows.
Nothing is read back from the screen.

## `private void PCorpusPortraitCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the export button is live: exactly when an entry is read in the display.
Print may also act on the other page this panel reads, but export acts on entries alone.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void PCorpusPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.
Nothing is read back from the screen.
