# PLibrary.xaml.cs

## `public partial class PLibrary : UserControl`

The library panel as a control: what it is made of, and when it starts and stops.
Browsing itself lives in the file beside this one.
That is the search, the ordering, the index, the read-only display and the editor beside it.

## `internal void PLibraryAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine`, the workspace the window opened.
It binds its lists and subscribes to the engine, and reads nothing yet.
The window fills it when it restores the stored ordering, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.
The command buttons are derived once here and again whenever the panel is shown.

## `internal void PLibraryReset()`

Puts the panel back on the workspace open now.
Nothing is selected, the editor is closed, and the index is re-read.
A different workspace has its own database.
So what the panel was showing came from one that is no longer open.

## `internal bool PLibraryDraftFinish(bool store)`

Carries the window's exit answer down to the editor this panel owns.
The panel holds no draft of its own, so it only passes the answer along.
What the editor answers is passed back up, because a store the engine refused must not close the window.

## `internal bool PLibraryChangeCheck()`

Whether the editor holds modifications that have not been stored.
That is what the window asks before the workspace changes or the program closes.

## `internal void PLibraryClose()`

Stops the panel: the editor is shut down and the shared display releases its playback.

## `private void PLibraryPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void PLibraryPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The panel names only the id it is showing, and the engine builds the page from stored rows.
Nothing is read back from the screen.

## Inline notes

### `private PWindow _pLibraryHost = null!;`

The window this panel sits in.
It is who reports a load that failed.
It also asks the question put before unsaved work would be lost.

### `PDisplay.PDisplayAttach(host, engine);`

The shared display is given the workspace it draws a flag from.
It is given the window as well, because an incoming row asks for another entry to be opened.
It is told nothing about which entry to show, because that is this panel's decision.

### `PEditor.PEditorAttach(host, engine, "Library", null);`

The editor opens on no entry: this panel puts it on one when the reader asks to write.
The origin names this panel, so its held work is told apart from the input panel's.

### `_pLibraryObserver = new PObserver(this, PLibraryBulletinHandle);`

A store may have changed the headword the index lists and the text the display shows.
The engine announces it, so both are read again from what was written rather than left as they were.
It is the same announcement whether the store happened in this panel's editor or in another tab.
The editor is told nothing about this panel, because a discard falls back on the entry its own draft names.
