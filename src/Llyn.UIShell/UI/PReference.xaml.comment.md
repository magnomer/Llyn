# PReference.xaml.cs

## `public partial class PReference : UserControl`

The Source panel: the view of the shelf of bibliographic Sources itself.
A Source is independent data owned by nothing, so this panel is not a view of one citation of it.
The visible label is Source and the internal base is Reference, because Source already names a pronunciation source.
It holds the host and the engine the browsing side calls, and nothing else.
The browsing lives in `PReferenceBrowse.cs`, and the editing in the `PImprint` control the panel holds.

## `internal void PReferenceAttach(PWindow host, LEngine engine)`

Binds the panel to the window it asks for confirmations and panel switches through.
It binds its lists and subscribes to the engine, and reads nothing yet.
It attaches the edit area to the same host and engine, and to itself.
The area reads the shelf's answers through it.
It attaches the entry display and editor to the same host and engine, so an Entry is read and written.
The editor's change notice drives `PReferenceStore`, as it drives the save button of the tenor panel.
The window fills the source shelf when it restores the stored ordering.
Every change after that arrives as an announcement.
The panel is current whether or not its tab is in front.

## `internal void PReferenceReset()`

Drops the selection and reads the shelf again, for when the workspace underneath changed.

## `internal bool PReferenceChangeCheck()`

Whether the panel holds work nothing has saved yet.
The window asks before anything can leave the panel.
The engine answers it, so the panel decides nothing about what counts as a change.
Credits are not compared, because they are written when they are made rather than on save.
The entry editor answers while it is in front, and the edit area otherwise.

## `internal bool PReferenceDraftFinish(bool store)`

Ends whichever draft is in front, committing it or discarding it.
The entry editor finishes its own draft, and the edit area finishes the held Source.
The window asks the panel, because the panel is what the window knows.

## `internal void PReferenceClose()`

Detaches the panel from the engine, so a closed panel is never announced to.
Closes the popups the panel owns, so none outlives the window.

## `private void PReferencePressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: an entry is read, or a source is read.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void PReferencePressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, or else the source being read, as the engine portrays it.
The panel names only the id it is showing, and the engine builds the page from stored rows.
Nothing is read back from the screen.
