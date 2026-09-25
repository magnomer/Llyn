# PPlaybackAudio.cs

## `public partial class PEditor`

The recording the primary pronunciation row currently carries.
That is which file it is, where it came from, and whether it is the loaded entry's own.
Playing it back belongs here too.
The downloader attaches one and the entry loader attaches one.
This is where it lives, plays, and is let go of.
The further rows carry their own recordings on their row models.
A recording is audio of one word in one language, and only the engine knows which recordings still fit.
So a headword or language change drops recordings in the engine, and the draft render empties the tray.
The volume tray is shared by every row, so whether it shows is decided here as well.

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

## `private void PPlaybackTrayShow()`

Shows the volume tray while any row has a recording, and hides it when none does.
