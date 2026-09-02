# PMarkupImport.cs

## `public partial class PList`

The list panel's import action: a markup file chosen by the reader is handed to the engine whole, and the index is re-read from what the workspace then holds. The panel does no parsing and writes nothing itself — it picks a file, passes its text on, and shows what came back.

## Inline notes

### `internal async void PMarkupImportHandle(object sender, RoutedEventArgs e)`

The button's whole behavior. Import belongs on this panel rather than the editor: what arrives is any number of entries, none of them the one being written, so the catalog is what changes.

### `Filter = "Llyn Markup|*.llx|All files|*.*",`

The format's own extension is offered first, with everything else still reachable: markup is plain text, so a file that carries it under another name is still importable.

### `if (dialog.ShowDialog(_pListHost) != true)`

A cancelled pick is not a failure and leaves the workspace exactly as it was.

### `string text = await File.ReadAllTextAsync(dialog.FileName);`

The file goes across as one string. Reading it and importing it stand inside the same attempt because an unreadable file and an unusable one are the same thing to the reader: the import did not happen.

### `await Task.Run(() => _lEngine.LEngineMarkupImport(text));`

A long file is scanned and written away from the panel's own thread, so the window keeps drawing while the work runs. What follows the await is back where the controls live, which is where the index may be touched.

### `_pListHost.PWindowFailureShow("List.ImportFailed", exception);`

Nothing is added on failure, and nothing needs undoing here: the engine imports every entry or none, so the index still shows what it showed before.

### `PIndexFind(PInquiry.Text ?? string.Empty);`

The index is filled again through the search that is typed, so the imported entries appear under the reader's current query rather than replacing it.
