# PDisplayPlayback.cs

## `public partial class PDisplay`

The playback side of the reading view: the entry's own recording, the player it plays on, and the volume tray.
The level is held by the workspace.
This view reads it on every show and writes it when the hand leaves the grip.

## `private void PDisplayPlaybackAttach()`

Wires the three gestures that end a volume change to one save.
The constructor calls it once, before the view is put on an engine.

## `private void PPlaybackActionHandle(object sender, RoutedEventArgs e)`

Plays the shown entry's own recording, and does nothing when it has none.

## `private void PVolumeHandle(object sender, RoutedPropertyChangedEventArgs<double> e)`

Follows the grip as it moves, so the sound changes under the hand.
The level is pushed to the shared catalog, so every other tray shows the same figure.

## `private void PVolumeSave(object sender, RoutedEventArgs e)`

Writes the level to the workspace once, and only when it differs from what is stored.

## `private void PVolumeLoad()`

Reads the stored level into the slider and the shared catalog.

## `private void PDisplayPlaybackShow()`

Shows the volume tray while the primary or any further row has a recording.

## Inline notes

### `private readonly MediaPlayer _pDisplayPlayer = new();`

This view's own playback.
Each panel that hosts a display gets its own player with the view.
A player shared across panels made one panel's clearing stop another panel's sound.

### `private string? _pDisplayRecording;`

Full path of the audio the shown entry owns, or null when it has none.
That is what the play button plays.

### `PVolume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(PVolumeSave));`

The level is played as the grip moves but written down only when the hand comes off it.
Writing on every step of a drag would put a file write behind every pixel.
It would also write back the level the view had just loaded.
A track click and an arrow key are gestures of their own, so each ends with a write too.
