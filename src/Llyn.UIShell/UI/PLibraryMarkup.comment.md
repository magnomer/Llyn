# PLibraryMarkup.cs

## `public partial class PLibrary`

The library panel's import action.
The path of a markup file chosen by the reader is handed to the engine.
The index is re-read from what the workspace then holds.
The panel does no parsing, opens no file, and writes nothing itself.
It picks a file, passes its path on, and shows what came back.

## Inline notes

### `internal async void PLibraryMarkupHandle(object sender, RoutedEventArgs e)`

The button's whole behavior.
Import belongs on this panel rather than the editor.
What arrives is any number of entries, none of them the one being written.
So the catalog is what changes.

### `Filter = "Llyn Markup|*.llx|All files|*.*",`

The format's own extension is offered first, with everything else still reachable.
Markup is plain text, so a file that carries it under another name is still importable.

### `if (dialog.ShowDialog(_pLibraryHost) != true)`

A cancelled pick is not a failure and leaves the workspace exactly as it was.

### `string path = dialog.FileName;`

Only the path crosses to the engine, which opens the file itself.
The workspace is the engine's to read, and this panel holds no part of it.

### `await Task.Run(() => _lEngine.LEngineMarkupImport(path));`

Opening the file and importing it stand inside the same attempt.
An unreadable file and an unusable one are the same thing to the reader.
The import did not happen.
A long file is read and written away from the panel's own thread.
So the window keeps drawing while the work runs.
What follows the await is back where the controls live, which is where the index may be touched.

### `_pLibraryHost.PWindowFailureShow("List.ImportFailed", exception);`

Nothing is added on failure, and nothing needs undoing here.
The engine imports every entry or none.
So the index still shows what it showed before.

### `PIndexFind(PInquiry.Text ?? string.Empty);`

The index is filled again through the search that is typed.
So the imported entries appear under the reader's current query rather than replacing it.
