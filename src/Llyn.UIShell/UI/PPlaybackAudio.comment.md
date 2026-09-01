# PPlaybackAudio.cs

## `public partial class PEditor`

The recording the editing form currently carries: which file it is, where it came from, whether it is the loaded entry's own, and playing it back. The downloader attaches one and the entry loader attaches one; this is where it lives, plays, and is let go of.

## Inline notes

### `private bool _pRecordingStored;`

Whether the recording on the form came back with a loaded entry rather than from the downloader. A fetched recording belongs to the spelling it was fetched for; a stored one belongs to the entry, and survives an edit of the headword.

### `private void PHeadwordHandle(object sender, TextChangedEventArgs e)`

A recording the downloader fetched is audio of one spelling, so changing the spelling throws it away rather than leaving the wrong word attached. A recording that came back with a loaded entry is the entry's own, and correcting a typo in the headword must not delete it — dropping it here is what made the next save write the entry with no audio row at all.
