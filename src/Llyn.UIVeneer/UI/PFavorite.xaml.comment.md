# PFavorite.xaml.cs

## `public partial class PFavorite : UserControl`

The favorites panel as a control: what it is made of, and when it starts and stops.
Browsing itself lives in the file beside this one.
That is the search, the ordering, the roster, the read-only display and the editor beside it.

## `internal void PFavoriteAttach(PWindow host)`

Puts the panel to work through its deportment and its editor.
The window deportment builds both over the engine's ports.
It binds its lists and reads nothing yet.
The window fills it when it restores the stored ordering, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.

Every mark and unmark is announced by the engine, wherever the star was clicked.
That is how a row leaves the roster the moment the entry it stands on is unmarked.

## `internal void PFavoriteReset()`

Puts the panel back on the workspace open now.
Nothing is selected, the editor is closed, and the roster is re-read.
A different workspace keeps its own marks.

## `internal bool PFavoriteDraftFinish(bool store)`

Carries the window's exit answer down to the editor this panel owns.
The panel holds no draft of its own, so it only passes the answer along.
A store the engine refused must not close the window, so the editor's answer is passed back up.

## `internal bool PFavoriteChangeCheck()`

Reports whether the editor is open and holding unsaved changes.
A closed editor answers no, whatever it still carries.

## `internal void PFavoriteClose()`

Stops the editor and the display when the window closes.

## `private void PFavoritePressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void PFavoritePressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The panel names only the id it is showing, and the engine builds the page from stored rows.
Nothing is read back from the screen.

## `private async void PFavoritePortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The window asks for the file and the format, and the engine writes the document from stored rows.
Nothing is read back from the screen.
