# PPlaybackAudio.cs

## `public partial class PEditor`

The recording the editing form currently carries.
That is which file it is, where it came from, and whether it is the loaded entry's own.
Playing it back belongs here too.
The downloader attaches one and the entry loader attaches one.
This is where it lives, plays, and is let go of.

## `private void PVolumeHandle(object sender, RoutedPropertyChangedEventArgs<double> e)`

Plays at the level the grip stands at, from the moment it is moved there.

## `private void PVolumeSave(object sender, RoutedEventArgs e)`

Writes the level down once the hand comes off the grip.
The level belongs to the workspace rather than to the form, so it outlives the entry being edited.

## `internal void PVolumeAttach()`

Listens for the three gestures that end a change of volume.
Those are a finished drag, a click on the track and a released key.
A drag that wrote on every step would put a file write behind every pixel of it.

## `internal void PVolumeLoad()`

Puts the workspace's volume on the grip when the form is attached.

## Inline notes

### `private bool _pRecordingStored;`

Whether the recording on the form came back with a loaded entry rather than from the downloader.
A fetched recording belongs to the spelling it was fetched for.
A stored one belongs to the entry, and survives an edit of the headword.

### `private void PHeadwordHandle(object sender, TextChangedEventArgs e)`

A recording the downloader fetched is audio of one spelling.
So changing the spelling throws it away rather than leaving the wrong word attached.
A recording that came back with a loaded entry is the entry's own.
Correcting a typo in the headword must not delete it.
Dropping it here is what made the next save write the entry with no audio row.
