# LLecternPlayback.cs

## `public sealed class LLecternPlayback`

The reading view's playback deportment, standing between the veneer and [LDisplaySound](../../Llyn.Conduct/Display/LDisplaySound.comment.md).
It drives the play button, the volume tray and the slider the veneer hands over.
The level is held by the workspace.
This class reads it on every show and writes it when the hand leaves the grip.
It shares the display's sound with [LLecternSound](LLecternSound.comment.md), which draws the rows these buttons play.

## `public LLecternPlayback(LDisplaySound display)`

Holds the display's sound, which owns the shown draft and plays through the engine.

## `public void LLecternPlaybackAttach(LWindow window, UIElement action, UIElement tray, RangeBase volume, Action<double> volumeSeam)`

Holds the controls, hooks the slider and loads the stored level.
`volumeSeam` carries each level to the shared catalog, so every other tray shows the same figure.

## `public void LLecternPlaybackShow()`

Shows the play button while the shown draft owns a recording on disk.
Shows the volume tray while that recording or any notated accent has audio.
The stored level is read again, so a level the editor set is the level this view plays at.

## `public void LLecternPlaybackClear()`

Hides the play button and the volume tray.

## `public void LLecternPlaybackHandle(object parameter)`

Plays the recording of the accent row the command carries, and ignores anything else.

## `public void LLecternActionHandle()`

Plays the shown draft's own recording.

## `private void LLecternVolumeLoad()`

Reads the stored level into the slider and hands it on as if the grip had moved.

## `private void LLecternVolumeHandle(double level)`

Follows the grip as it moves, so the sound changes under the hand.

## `private void LLecternVolumeSave()`

Writes the slider's level to the workspace, which skips a level already stored.

## Inline notes

### `volume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler((_, _) => LLecternVolumeSave()));`

The level is played as the grip moves but written down only when the hand comes off it.
Writing on every step of a drag would put a file write behind every pixel.
A track click and an arrow key are gestures of their own, so each ends with a write too.
