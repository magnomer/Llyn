# PDisplay.xaml.cs

## `public partial class PDisplay : UserControl`

The shared read-only entry view as a control.
It is handed a loaded draft and draws it.
It never loads one itself, and it never decides which entry is shown.
That belongs to the browse-style panel it sits in.

## `internal void PDisplayAttach(LEngine engine)`

Puts the view on `engine`, the workspace the window opened.
The engine is read for one thing only, the flag drawn beside the language.

## `internal void PDisplayShow(LEntryDraft draft)`

Draws `draft` as the entry being read.
A field the draft left empty collapses instead of standing as a blank line.
The play button appears only when the entry owns a recording that is still on disk.

## `internal void PDisplayClear()`

Empties the view and leaves the unselected notice in its place.
Playback stops, because what it was playing belonged to the entry that was shown.

## `internal void PDisplayClose()`

Releases this view's own playback.

## Inline notes

### `private readonly MediaPlayer _pDisplayPlayer = new();`

This view's own playback.
Each panel that hosts a display gets its own player with the view.
A player shared across panels made one panel's clearing stop another panel's sound.

### `private string? _pDisplayRecording;`

Full path of the audio the shown entry owns, or null when it has none.
That is what the play button plays.

### `private async void PDisplayLanguageShow(string language)`

The flag is fetched, so a later entry may be shown before it arrives.
The language shown now is compared before the image is set.
