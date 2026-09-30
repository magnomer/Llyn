# PPlaybackAudio.cs

## `public partial class PEditor`

The play button of the entry's own recording, the volume tray every row shares, and the form's player.
Which recording plays and whether its file still exists are the editor's sound facts in `CTimbre`.
This part only paints them and plays the address the play gate answers.
The further rows carry their own recordings on their row models.
A recording is audio of one word in one language, and only the engine knows which recordings still fit.
So a headword or language change drops recordings in the engine, and the draft render empties the tray.

## `internal void PPlaybackRefine(CEntryDraft _)`

Repaints the play button and the playback tray from the editor's ready playback after each draft change.

## `private void PPlaybackActionObserve(object sender, RoutedEventArgs e)`

Hands the recording the pressed button carries to the play gate, and its answer to the look.

## `private void PPlaybackActionRefine(Uri? address)`

Plays the address the play gate answered.
No address means the file went missing since the button was painted.
The button and the tray are then painted again from a fresh read, which lets the recording go.

## `private void PPlaybackAudioRefine(CTimbrePlayback playback)`

Paints the play button and the volume tray from the editor's ready playback.
The button's tag is its paint memory: the recording it offers, handed back when it is pressed.
A recording other than the painted one stops the player and repaints the button.
An unchanged recording is left alone, so an edit elsewhere in the form never cuts a playing sound short.
The comparison is against what the button painted, never against the draft, so it decides no data.

## `private void PVolumePlayerRefine(object sender, RoutedPropertyChangedEventArgs<double> e)`

Plays at the level the grip stands at, from the moment it is moved there.
It also writes the shared volume, where a two-way binding stood, so every other tray follows.

## `private void PVolumeObserve(object sender, RoutedPropertyChangedEventArgs<double> e)`

Hands the moved level to the volume gate unsettled, so the engine plays at it without a write.

## `private void PVolumeSaveObserve(object sender, RoutedEventArgs e)`

Hands the level to the volume gate settled once the hand comes off the grip.
The level belongs to the workspace rather than to the form, so it outlives the entry being edited.

## `internal void PVolumeAttach()`

Follows the grip, and listens for the three gestures that end a change of volume.
It runs once the host is attached.
It first sets the grip from the shared volume, before listening, as the binding did at load.
It then follows the shared volume, so a tray elsewhere moves this grip.
Those are a finished drag, a click on the track and a released key.
A drag that wrote on every step would put a file write behind every pixel of it.

## `internal void PVolumeRefine()`

Puts the workspace's volume on the grip and on the form's player each time a workspace opens.

## `private void PVolumeLevelRefine(object? sender, PropertyChangedEventArgs e)`

The shared volume changed elsewhere, so the slider follows it, where a two-way binding stood.
