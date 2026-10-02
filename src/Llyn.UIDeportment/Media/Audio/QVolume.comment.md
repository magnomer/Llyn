# QVolume.cs
Hash: `68712cce92dbc4ed`

## `internal sealed class QVolume`

The window's one driver of the shared volume, which every editor's and reading view's tray attaches to.
The level belongs to the workspace rather than to the form, so it outlives the entry being edited.
Every editor and reading view lives as long as the window, so the owner holds its grips for good.
One owner means one listener per notice and one gate call per moved grip, however many editors exist.

## `internal QVolume(CAtelier atelier)`

Hears the shared level and each opened workspace once, for every tray at the same time.

## `internal void QVolumeSliderAttach(FrameworkElement surface)`

Takes the slider of an editor's or a reading view's tray.
It first sets the grip from the shared volume, before listening, as the binding did at load.
It then listens for the three gestures that end a change of volume.
Those gestures are a finished drag, a click on the track and a released key.
A drag that wrote on every step would put a file write behind every pixel of it.

## `internal void QVolumePlayerAttach(MediaPlayer player)`

Takes an editor's one player, which plays at the shared level from the next paint on.
A reading view holds no player, since it hands the slider's level to each playback it starts.

## `private void QVolumeObserve(object sender, RoutedPropertyChangedEventArgs<double> e)`

A grip moved, so its level becomes the shared volume, where a two-way binding stood.
It then hands the level to the volume gate unsettled, so the engine plays at it without a write.
Only a grip moved by hand gets here, since painting goes unheard.

## `private void QVolumeLevelRefine(object? sender, PropertyChangedEventArgs e)`

The shared volume changed from a moved grip or an opened workspace, so every grip and player follows it.
It makes no request, since a moved grip made its own and an opened workspace needs none.

## `private void QVolumeSaveObserve(object sender, RoutedEventArgs e)`

Hands the touched grip's level to the volume gate settled once the hand comes off it.

## `private void QVolumeRefine()`

Puts the workspace's volume on every grip and player each time a workspace opens.
The level is a ready answer read from the gate, so it is painted and makes no request.
It first becomes the shared volume, so a tray attached later starts at it.
Every grip and player is then painted directly.
That paint still lands when the shared volume already held the level and raised no notice.

## `private void QVolumeShow(double level)`

Paints the level on every attached grip and plays every attached player at it.
Each grip is unheard while it is painted, so a painted grip makes no second request.
