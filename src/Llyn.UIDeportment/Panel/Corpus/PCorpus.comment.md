# PCorpus.cs

## `public partial class PCorpus : UserControl`

The Corpus panel: the view of the shared stock of sentences itself.
An Example is independent data owned by nothing, so this panel is not a view of one Entry's sentences.
It holds the host and the engine the browsing side calls, and nothing else.
The browsing behavior lives in `PCorpusBrowse.cs` and the editing in `PCorpusEditor.cs`, one file per responsibility.
The held draft the editor writes into lives in `PCorpusHold.cs`, apart from the controls it reads.
The linking gesture over the transcript lives in `PCorpusMention.cs`.

## `private LCorpus _lCorpus`

The panel's deportment, holding the anthology, the quotation and the desk the window restored.
The anthology's vista carries the order, the query, and the languages hidden from the entry column.
The panel keeps no copy of any of them and asks the deportment for each where it needs it.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

## `public PCorpus()`

Loads the panel's markup from the Veneer and wears it as its content.
The markup's name scope is copied onto the panel, so its named parts answer `FindName`.
It builds the transcript dictionary, merges it, and registers the local styles of both dictionaries with the look.
It adds the print, export and four Mention command bindings, and points the two buttons at their commands.
It ties the three droppers to their popups, sets every icon, and subscribes every click and field event.
It attaches the row fills of the catalog, the quotations, the citation drawer and the speaker list.

## `private Border PRank`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal void PCorpusAttach(PWindow host)`

Builds the panel's deportment and its editor over the engine's ports, and wires the desk's notices.
Binds the panel to the window it asks for confirmations and panel switches through.
It binds its lists and subscribes to the engine, and reads nothing yet.
The chip line and the two Gloss lists are attached to their fills, since their templates carry no bindings.
The transcript Gloss list takes the panel's own fill, which wires the dictionary's forwarding handlers.
It attaches the entry display and editor to the same host and engine, so an Entry is read and written.
The engine's change notices drive the mode, and its row notices drive the two lists.
Its transcript and example notices paint the sheet, and its failures reach the window.
A dropped search empties the search box and the language menu.
The print and export commands are bound in the constructor, and their checks answer false until the engine exists.
The window fills the example catalog when it restores the stored ordering.
Every change after that arrives as an announcement.
The panel is current whether or not its tab is in front.

## `private bool PCorpusShownCheck()`

Whether the panel is on screen, so the engine knows when a notice needs painting.

## `private bool PCorpusDiscardConfirm(Func<bool, bool> finish)`

Asks the window whether unsaved work may be dropped before the engine moves on.
A save runs the finish the deportment handed in, so the deportment decides what follows it.

## `private bool PCorpusRemovalConfirm(int usage)`

Asks the window whether an Example cited this many times may be removed.

## `private void PCorpusModeUpdate()`

Paints the mode the engine decides: which page shows, which toggle is checked, which button is live.
The transcript is live only while its desk runs.
It ends by refreshing the rail's undo and redo.

## `internal void PCorpusReset()`

Drops both selections through the deportment and reads the catalog again, for when the workspace underneath changed.

## `internal bool PCorpusChangeCheck()`

Whether the editor holds work nothing has saved yet.
The engine answers it against the held draft rather than the panel against a copy of a stored record.
The window asks before anything can leave the panel.
The deportment asks the quotation side and the anthology side, and each answers only while it edits.

## `internal bool PCorpusDraftFinish(bool store)`

Ends whichever draft is in front, committing it or discarding it.
The deportment finishes the entry editor's draft while it is in front, and the held sentence otherwise.

## `internal void PCorpusClose()`

Closes the popups the panel owns, so none outlives the window.

## `private void PCorpusPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: an entry is read, or an example is read.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void PCorpusPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, or else the example being read, as the engine portrays it.
The deportment picks the page from the side it shows, and the engine builds it from stored rows.
Nothing is read back from the screen.

## `private void PCorpusPortraitCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the export button is live: exactly when an entry is read in the display.
Print may also act on the other page this panel reads, but export acts on entries alone.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void PCorpusPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.
Nothing is read back from the screen.
