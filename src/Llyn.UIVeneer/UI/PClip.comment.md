# PClip.cs

## `public partial class PEditor : LListener`

Audio download as the editor shows it.
Every pronunciation row carries its own download button, and the menu opens under the one pressed.
The menu is one popup the editor owns, retargeted at the row that asked for it.
Opening it starts a search for recordings of the headword, the same search from whichever row.
The search is a foray the tenure starts, and the menu keeps only that handle.
The word, language, target row and flag mode are read off the foray, never copied.
Recordings stream in from the engine and fill the list, one per variety a source returned.
The engine already narrowed them to the variety of the row that opened the menu.
Each recording can be previewed.
Taking one has the foray download and attach it to that row, then tags the row with its variety.
The form refills from the draft bulletin that attach raises.
This is the shell side of `LListener`.
The engine calls back on a worker thread.
So every arrival is marshalled onto the dispatcher here.

## Inline notes

### `if (language.Length > 0 && _lEngine.LEngineFlaggedCheck(language))`

In flag mode every declared variety's flag is resolved before the search starts.
A recording's flag is then ready the moment the recording lands.
A menu closed while the flags loaded starts no search.

### `_lEditor.LEditorClipStart(word, target, this);`

The tenure starts the search and the menu listens.
The tenure passes the draft it holds, so the engine can hand back what that draft already found.
The search is over when the listener is told it is, never when the start returns.
The engine reports the end through LListenerFinish.

### `catch (Exception)`

A discovery that could not be started reports no end of its own.
So the menu is taken out of its searching state here.
It is not left running under a search that never began.

### `private async Task PClipOpen(UIElement anchor, long target)`

Opens the menu under the button of one row and starts the search for that row.
A target of zero is the primary row, which the engine may not have minted yet.
The popup is shut first, so a press on another row's button moves it instead of leaving it put.

### `private void PClipCancel()`

Ends the search in flight and drops the handle, so a stale arrival finds no foray to read.

### `private PClipItem PClipPlace(string source, int order)`

Finds the row one source owns, creating it at its declared position when it has none yet.
Rows stand where the language pack put the source, not where the network put it.
Every source is asked at once, so a fast one would otherwise head a list the user did not order.
A new row opens saying it is searching, so every declared source is visible before any of them answers.

### `private PClipReading PClipReadingCreate(LRecording recording)`

Builds the previewable, takeable entry for one recording that carries an address.
Its label and flag are resolved as a pronunciation row resolves its own, under the draft's language of the moment.
An untagged recording gets neither, so the row shows its two buttons alone.

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

### `string path = await _pEditorHost.PWindowDeportment.LWindowRecordingPrepare(reading.PClipReadingModel, CancellationToken.None);`

Streaming the remote, token-bearing URL through the media stack is unreliable.
Fetch it to a local temp file first, then play that.

### `internal async void PClipSelectorHandle(object sender, RoutedEventArgs e)`

Preview is best-effort.
A failed fetch leaves the menu untouched, only the button's fill changing.
The button fills orange while the recording is fetched and blue once it sounds.
It fills with the warning colour when the fetch failed.
The warning stays until the same button is pressed again, so a refusal is not mistaken for silence.
Only one preview is marked at a time, so starting another clears the last.
A fetch that finishes after another preview took over does not play.

### `private void PClipPreviewClear()`

Returns the marked preview's button to plain and forgets it.
It is called when a sound ends or fails, when another preview starts, and when the menu closes.

### `private void PClipEndHandle(object? sender, EventArgs e)`

The player's end and failure both clear the mark, since either way nothing sounds any more.

### `reading.PClipReadingAction = _pEditorHost.PLocalizationTextRead("Downloader.Saving");`

The download is reported on the recording that was taken, not on the status card.
That card belongs to the search.
A recording still arriving would overwrite whatever was written there.

### `if (!attached)`

The draft moved on while the bytes came down, and the foray attached nothing.
The file stays in the workspace.
But it is audio of a word the form no longer holds.

### `long id = _lEditor.LEditorClip.LClipPrimary`

The foray attached the recording to the row it was opened for, and the draft bulletin refills the form.
The primary row's id is read back after the audio is attached, since the engine mints that row late.

### `PNotationVarietySend(id, reading.PClipReadingVariety);`

Taking a recording tags its row with the variety, exactly as taking a reading does.
An untagged recording sends nothing, and the row keeps whatever it had.

### `PClip.IsOpen = false;`

Taking a recording closes the menu, the way taking a pronunciation candidate does.

### `reading.PClipReadingAction = _pEditorHost.PLocalizationTextRead("Downloader.Retry");`

A failed download leaves the row offering another try.

### `void LListener.LListenerRecordingAdd(LRecording recording)`

Handled the same way as lookup: each answer resolved into the row its source owns.
A source may answer once per variety, and each answer with an address becomes one more recording on the row.
A source that was reached and had nothing reads differently from one that was never reached.
