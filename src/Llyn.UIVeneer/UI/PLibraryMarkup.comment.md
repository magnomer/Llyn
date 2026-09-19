# PLibraryMarkup.cs

## `public partial class PLibrary`

The library panel's import action.
The path of a markup file chosen by the reader is handed to the engine.
The entries the file declares are shown in the customs window before any of them enters.
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

### `LMarkupCargo cargo = await _lEngine.LEngineMarkupStart(path);`

The file is read once so the customs window can list what it declares.
A long file is parsed on an engine thread, so the window keeps drawing and the panel starts no task.

### `IReadOnlyList<LMarkupIntake>? intakes = PSCustoms.PSCustomsShow(_pLibraryHost, _lEngine, entries);`

The reader chooses how each entry enters before anything is written.
A cancelled window answers null, and the workspace is left untouched.

### `LMarkupOutcome outcome = await _lEngine.LEngineMarkupStart(cargo, intakes);`

The engine opens the file again and imports it under the declared intakes.
An unknown file and an unusable one are the same thing to the reader.
The import did not happen.
What follows the await is back where the controls live, which is where the index may be touched.

### `PSCustoms.PSCustomsOmissionShow(_pLibraryHost, outcome.LMarkupOutcomeOmission);`

What the import could not place is shown after the index already holds the entries.
An import that dropped nothing shows nothing.

### `_pLibraryHost.PWindowFailureShow("List.ImportFailed", exception);`

Nothing is added on failure, and nothing needs undoing here.
The engine imports every entry or none.
So the index still shows what it showed before.

### `PIndexFind(PInquiry.Text ?? string.Empty);`

The index is filled again through the search that is typed.
So the imported entries appear under the reader's current query rather than replacing it.
