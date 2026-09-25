# LLecternPlayback.cs

## `public sealed class LLecternPlayback`

The reading view's playback deportment, standing between the veneer and [LDisplaySound](../../Llyn.Conduct/Display/LDisplaySound.comment.md).
It drives the play button, the volume tray and the slider the veneer hands over.
The level is the workspace's one audio level, held by the posture.
Every move of the grip sets that level, and the hand leaving the grip writes it.
It shares the display's sound with [LLecternSound](LLecternSound.comment.md), which draws the rows these buttons play.

## `public LLecternPlayback(LDisplaySound display)`

Holds the display's sound, which owns the shown draft and plays through the engine.

## `public void LLecternPlaybackAttach(LWindow window, UIElement action, UIElement tray, RangeBase volume)`

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

## `private void LLecternVolumeHandle(double level)`

Sets the workspace's one level as the grip moves, so every audio view changes under the hand.

## `private void LLecternVolumeSave()`

Writes the workspace's level, which skips a level already written.

## Inline notes

### `volume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler((_, _) => LLecternVolumeSave()));`

The level is played as the grip moves but written down only when the hand comes off it.
Writing on every step of a drag would put a file write behind every pixel.
A track click and an arrow key are gestures of their own, so each ends with a write too.
