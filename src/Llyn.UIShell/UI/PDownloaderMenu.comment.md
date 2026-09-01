# PDownloaderMenu.cs

## `public partial class PEditor : LListener`

Audio download as the editor shows it: opening the menu starts a search for recordings of the headword, each found recording can be previewed, and taking one downloads it into the workspace and attaches it to the form. This is the shell side of `LListener` — the engine calls back on a worker thread, so every arrival is marshalled onto the dispatcher here.

## Inline notes

### `await _lEngine.LEngineRecordingFind(word, _pLangcodeChoice, this, _pDownloaderCancellation.Token);`

The panel asks and then listens: the search is over when the listener is told it is, never when this call returns. The engine reports the end through LListenerFinish, and reading completion off the awaited task as well gave the menu a second opinion about a search it does not run.

### `catch (Exception)`

Superseded by a newer discovery or the window closed; ignore.

### `_pDownloaderSearching = false;`

A discovery that could not be started reports no end of its own, so the menu is taken out of its searching state here rather than left running under a search that never began.

### `private void PDownloaderMenuUpdate()`

What the menu shows, from the two things it knows: whether the search is still running, and what has arrived so far. They are independent — a source that has already answered does not end the search — which is why the running line follows the search alone. Reading it off "nothing found yet" instead is what left it running under a menu that was plainly finished.

### `if (recordings)`

The notice is the one line the menu says while it has no rows to show: what it is doing, or that there was nothing to find. With rows on screen it says nothing — a row reports its own download itself.

### `string path = await _lEngine.LEngineRecordingPrepare(recording.PDownloaderRecordingModel, CancellationToken.None);`

Streaming the remote, token-bearing URL through the media stack is unreliable; fetch it to a local temp file first, then play that.

### `internal async void PDownloaderMenuHandle(object sender, RoutedEventArgs e)`

Preview is best-effort; a failed fetch leaves the menu untouched.

### `recording.PDownloaderRecordingAction = _pEditorHost.PLocalizationTextRead("Downloader.Saving");`

The download is reported on the row that was taken, not on the status card: that card belongs to the search, and a recording still arriving would overwrite whatever was written there.

### `return;`

The form moved on while the bytes came down; the file stays in the workspace, but it is audio of a word the form no longer holds.

### `_pRecordingStored = false;`

Fetched for the headword as it stands now, so a further edit of it drops this.

### `PDownloader.IsChecked = false;`

Taking a recording closes the menu, the way taking a pronunciation candidate does.

### `recording.PDownloaderRecordingAction = _pEditorHost.PLocalizationTextRead("Downloader.Retry");`

A failed download leaves the row offering another try.

### `void LListener.LListenerRecordingAdd(LRecording recording)`

Handled the same way as lookup: the running line already covers the whole search.
