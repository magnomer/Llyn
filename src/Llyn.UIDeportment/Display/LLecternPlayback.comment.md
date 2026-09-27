# LLecternPlayback.cs

## `public sealed class LLecternPlayback`

The reading view's playback deportment, standing between the veneer and [LDisplaySound](../../Llyn.Conduct/Display/LDisplaySound.comment.md).
It drives the play button, the volume tray and the slider the veneer hands over.
The level is the workspace's one audio level, held by the posture.
Every move of the grip sets that level, and the hand leaving the grip writes it.
It shares the display's sound with [LLecternSound](LLecternSound.comment.md), which draws the rows these buttons play.

## `public LLecternPlayback(LDisplaySound display)`

Holds the display's sound, which owns the shown draft and plays through the engine.

## `public void LLecternPlaybackAttach(CAtelier atelier, UIElement action, UIElement tray, RangeBase volume)`

Holds the controls, hooks the slider and puts the workspace's level on it.
The veneer binds the slider to the shared catalog, so every other tray shows the same figure.

## `public void LLecternPlaybackShow()`

Shows the play button while the shown draft owns a recording on disk.
Shows the volume tray while that recording or any notated accent has audio.
The level is read again, so the slider shows the level a workspace switch brought.

## `public void LLecternPlaybackClear()`

Hides the play button and the volume tray.

## `public void LLecternPlaybackHandle(object parameter)`

Plays the recording of the accent row the command carries at the workspace's level, and ignores anything else.

## `public void LLecternActionHandle()`

Plays the shown draft's own recording at the workspace's level.

## `private void LLecternVolumeLoad()`

Reads the workspace's level into the slider.
A level that differs moves the grip, which sets the same level back.

## `private void LLecternVolumeHandle(object sender, RoutedPropertyChangedEventArgs<double> e)`

Hands each step of the grip to the volume gate unsettled, so the level plays but is not written.

## `private void LLecternVolumeSettle(object sender, RoutedEventArgs e)`

Hands the level to the volume gate settled when a drag, a track click or an arrow key ends.
The gate then decides whether the level is written.
