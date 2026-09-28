# QCorpus.cs

## `internal sealed partial class QCorpus : QChronicleHost`

The Corpus panel's driver: the view of the shared stock of sentences itself.
An Example is independent data owned by nothing, so this panel is not a view of one Entry's sentences.
It holds the surface, the host, the drawer and the corpus Conduct the browsing side calls, and nothing else.
The browsing behavior lives in `QCorpusBrowse.cs` and the editing in `QCorpusEditor.cs`, one file per responsibility.
The held draft the editor writes into lives in `QCorpusHold.cs`, apart from the controls it reads.
The linking gesture over the transcript lives in `QCorpusMention.cs`.
It answers the window's undo and redo keys as `QChronicleHost`, attached to its surface since the surface is no driver.

## `private CCorpus _cCorpus`

The corpus Conduct, holding the anthology, the quotation list, the desk, the session and the panel's mode.
The anthology's vista carries the order, the query, and the languages hidden from the entry column.
The driver keeps no copy of any of them and asks the Conduct for each where it needs it.
It is null until the window hands one over, so the command checks answer false before that.

## `private readonly QDrawer _qDrawer`

The citation drawer, which holds the offered Sources and the row the arrow keys stand on.
It is built over the drawer's parts here, with the press on a row answered by this driver.

## `internal QCorpus(UserControl surface)`

Takes the veneer's page as its surface, which the window pulls by contract ID.
The page merges the excerpt and transcript dictionaries itself, so their local styles reach the look sheet here.
The driver attaches itself to the page as the host of the undo and redo keys.
It adds the print, export and four Mention command bindings, and points the two buttons at their commands.
It ties the three droppers to their popups, sets every icon, and subscribes every click and field event.
It attaches the row fills of the catalog, the quotations and the speaker list.

## `private Border QRank`

Each named part of the page is pulled through `QContract.QContractFind` by its contract ID.

## `internal void QCorpusAttach(PWindow host)`

Takes the corpus Conduct and its editor from the forge, and wires the desk's notices.
The lectern follows the quotation panel, whose loads and clears reach the display's area, never the veneer.
Binds the panel to the window it asks for confirmations and panel switches through.
It binds its lists and subscribes to the engine, and reads nothing yet.
The chip line and the two Gloss lists are attached to their fills, since their templates carry no bindings.
The transcript Gloss list takes the driver's own fill, which wires the row's handlers around the shared fill.
It attaches the entry display and editor to the same host and engine, so an Entry is read and written.
The engine's change notices drive the mode, and its row notices drive the two lists.
Its transcript and example notices paint the sheet.
Its failures reach the window through the envoy, which the Conduct asks directly.
A dropped search empties the search box and the language menu.
The window fills the example catalog when it restores the stored ordering.
Every change after that arrives as an announcement.
The panel is current whether or not its tab is in front.

## `private bool QCorpusShownCheck()`

Whether the page is on screen, so the engine knows when a notice needs painting.

## `private void QCorpusModeUpdate()`

Paints the mode the engine decides: which page shows, which toggle is checked, which button is live.
The transcript is live only while its desk runs.
It ends by refreshing the rail's undo and redo.

## `internal void QCorpusReset()`

Drops both selections through `CCorpusExampleClose`, for when the workspace underneath changed.

## `internal bool QCorpusChangeCheck()`

Whether the editor holds work nothing has saved yet.
The engine answers it against the held draft rather than the driver against a copy of a stored record.
The window asks before anything can leave the panel.

## `internal bool QCorpusDraftFinish(bool store)`

Ends whichever draft is in front, committing it or discarding it.
The deportment finishes the entry editor's draft while it is in front, and the held sentence otherwise.

## `internal void QCorpusClose()`

Closes the popups the panel owns, the citation drawer among them, so none outlives the window.

## `private void QCorpusPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: an entry is read, or an example is read.
An editor on screen prints nothing, because what is printed is what is read.

## `private async void QCorpusPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, or else the example being read, as the engine portrays it.
The deportment picks the page from the side it shows, and the engine builds it from stored rows.

## `private void QCorpusPortraitCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the export button is live: exactly when an entry is read in the display.
Print may also act on the other page this panel reads, but export acts on entries alone.

## `private async void QCorpusPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.
