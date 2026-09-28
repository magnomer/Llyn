# QLecternPlayback.cs

## `public sealed class QLecternPlayback`

The reading view's playback driver, over [CDisplaySound](../../Llyn.Conduct/Display/CDisplaySound.comment.md).
It drives the play button, the volume tray and the slider the veneer hands over.
The level is the workspace's one audio level, held by the posture.
Every move of the grip sets that level, and the hand leaving the grip writes it.
The rows these buttons play are drawn by [QLecternAccent](QLecternAccent.comment.md).

## `public void QLecternPlaybackIntroduce(CAtelier atelier, UIElement action, UIElement tray, RangeBase volume)`

Holds the controls, hooks the slider and puts the workspace's level on it.
The veneer binds the slider to the shared catalog, so every other tray shows the same figure.

## `public void QLecternPlaybackRefine()`

Shows the play button and the volume tray by the verdicts the area answers for the shown entry.

## `public void QLecternVolumeRefine()`

Reads the workspace's level into the slider, so it shows the level a workspace switch brought.
A level that differs moves the grip, which sets the same level back.

## `public void QLecternTrayRefine()`

Hides the play button and the volume tray.

## `public void QLecternPlaybackObserve(object parameter)`

Hears an accent row's play command and hands its recording and the slider's level to the playback gate.
Anything but an accent row is ignored.

## `public void QLecternActionObserve()`

Hears the play button and hands the slider's level to the playback gate for the shown draft's own recording.

## `private void QLecternVolumeObserve(object sender, RoutedPropertyChangedEventArgs<double> e)`

Hands each step of the grip to the volume gate unsettled, so the level plays but is not written.

## `private void QLecternSettleObserve(object sender, RoutedEventArgs e)`

Hands the level to the volume gate settled when a drag, a track click or an arrow key ends.
The gate then decides whether the level is written.
