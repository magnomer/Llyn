# PYunjing.xaml.cs

## `public partial class PYunjing : UserControl`

The yunjing panel: the workspace browsed as a rime table, by onset and rime of the user's reconstruction.
It is shown only while a loaded language pack carries rime books, since without them there is no table.
It reads one language, the first pack with rime books, and owns a reader and an editor over one Entry.

## `internal void PYunjingAttach(PWindow host, LEngine engine)`

Binds the panel to the window and the engine, starts listening for what the engine announces, and loads.

## `internal bool PYunjingCheck()`

Whether any loaded language pack carries rime books, so the window knows to show the tab.

## `internal void PYunjingReset()`

Finds the language again, drops the choices, empties the reader and reloads, for a workspace that has just moved.

## `internal bool PYunjingDraftFinish(bool store)`

Stores or discards a standing draft on the way out of the application.

## `internal bool PYunjingChangeCheck()`

Whether the editor is the shown side and is holding a change, so leaving would lose work.

## `internal void PYunjingClose()`

Stops listening and closes the reader and the editor.

## `private string? PYunjingLanguageFind()`

The first loaded language whose pack names rime books, or `null`.

## `private void PYunjingPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the print and export buttons are live: exactly when an entry is read in the display.

## `private async void PYunjingPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.

## `private async void PYunjingPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
