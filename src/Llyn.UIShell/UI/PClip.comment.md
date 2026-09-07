# PClip.cs

## `public partial class PEditor : LListener`

Audio download as the editor shows it.
Opening the menu starts a search for recordings of the headword.
Each found recording can be previewed.
Taking one downloads it into the workspace and attaches it to the form.
This is the shell side of `LListener`.
The engine calls back on a worker thread.
So every arrival is marshalled onto the dispatcher here.

## Inline notes

### `await _lEngine.LEngineRecordingFind(word, _pSpeakerChoice, this, _pClipCancellation.Token);`

The panel asks and then listens.
The search is over when the listener is told it is, never when this call returns.
The engine reports the end through LListenerFinish.
Reading completion off the awaited task gave the menu a second opinion.
The search is not one the menu runs.

### `catch (Exception)`

Superseded by a newer discovery, or the window closed.
Ignore it.

### `_pClipSearching = false;`

A discovery that could not be started reports no end of its own.
So the menu is taken out of its searching state here.
It is not left running under a search that never began.

### `private void PClipPlace(PClipItem recording)`

Puts an arriving recording where the language pack put its source, not where the network put it.
Every source is asked at once, so a fast one would otherwise head a list the user did not order.
Equal positions cannot occur, because a source answers once.

### `private void PClipUpdate()`

What the menu shows, from the two things it knows.
Those are whether the search is still running, and what has arrived so far.
They are independent, because a source that has already answered does not end the search.
That is why the running line follows the search alone.
Reading it off "nothing found yet" instead is what left it running under a menu that was plainly finished.

### `if (recordings)`

The notice is the one line the menu says while it has no rows to show.
It says what it is doing, or that there was nothing to find.
With rows on screen it says nothing — a row reports its own download itself.

### `string path = await _lEngine.LEngineRecordingPrepare(recording.PClipItemModel, CancellationToken.None);`

Streaming the remote, token-bearing URL through the media stack is unreliable.
Fetch it to a local temp file first, then play that.

### `internal async void PClipSelectorHandle(object sender, RoutedEventArgs e)`

Preview is best-effort.
A failed fetch leaves the menu untouched.

### `recording.PClipItemAction = _pEditorHost.PLocalizationTextRead("Downloader.Saving");`

The download is reported on the row that was taken, not on the status card.
That card belongs to the search.
A recording still arriving would overwrite whatever was written there.

### `return;`

The form moved on while the bytes came down.
The file stays in the workspace.
But it is audio of a word the form no longer holds.

### `_pRecordingStored = false;`

Fetched for the headword as it stands now, so a further edit of it drops this.

### `PDownloader.IsChecked = false;`

Taking a recording closes the menu, the way taking a pronunciation candidate does.

### `recording.PClipItemAction = _pEditorHost.PLocalizationTextRead("Downloader.Retry");`

A failed download leaves the row offering another try.

### `void LListener.LListenerRecordingAdd(LRecording recording)`

Handled the same way as lookup: the running line already covers the whole search.
