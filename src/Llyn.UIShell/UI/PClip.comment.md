# PClip.cs

## `public partial class PEditor : LListener`

Audio download as the editor shows it.
Every pronunciation row carries its own download button, and the menu opens under the one pressed.
The menu is one popup the editor owns, retargeted at the row that asked for it.
Opening it starts a search for recordings of the headword, the same search from whichever row.
Each found recording can be previewed.
Taking one downloads it into the workspace and attaches it to the row that opened the menu.
This is the shell side of `LListener`.
The engine calls back on a worker thread.
So every arrival is marshalled onto the dispatcher here.

## Inline notes

### `await _lEngine.LEngineRecordingFind(`

The panel asks and then listens.
It passes the draft it is editing, so the engine can hand back what that draft already found.
The search is over when the listener is told it is, never when this call returns.
The engine reports the end through LListenerFinish.
Reading completion off the awaited task gave the menu a second opinion.
The search is not one the menu runs.

### `catch (Exception)`

Superseded by a newer discovery, or the window closed.
Ignore it.

### `private async Task PClipOpen(UIElement anchor, long target)`

Opens the menu under the button of one row and remembers which row it serves.
A target of zero is the primary row, which the engine may not have minted yet.
The popup is shut first, so a press on another row's button moves it instead of leaving it put.

### `_pClipSearching = false;`

A discovery that could not be started reports no end of its own.
So the menu is taken out of its searching state here.
It is not left running under a search that never began.

### `private PClipItem PClipPlace(string source, int order)`

Finds the row one source owns, creating it at its declared position when it has none yet.
Rows stand where the language pack put the source, not where the network put it.
Every source is asked at once, so a fast one would otherwise head a list the user did not order.
A new row opens saying it is searching, so every declared source is visible before any of them answers.

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

### `if (target == 0)`

The primary row keeps its recording on the form, the way it always has.
A further row is sent an audio request on its own id, and the draft render shows the file.
Either way the recording is fetched for the headword as it stands now, so editing it drops this.
The recording is sent as a request at once, because a pick is a whole action.

### `PClip.IsOpen = false;`

Taking a recording closes the menu, the way taking a pronunciation candidate does.

### `recording.PClipItemAction = _pEditorHost.PLocalizationTextRead("Downloader.Retry");`

A failed download leaves the row offering another try.

### `void LListener.LListenerRecordingAdd(LRecording recording)`

Handled the same way as lookup: one answer per source, resolved into the row that source owns.
A source that was reached and had nothing reads differently from one that was never reached.
