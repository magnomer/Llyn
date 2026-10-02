# QLecternPlayback.cs

## `public sealed class QLecternPlayback`

The reading view's playback driver, over [CDisplaySound](../../Llyn.Conduct/Display/CDisplaySound.comment.md).
It drives the play button and the volume tray, and plays at the slider the veneer hands over.
The slider itself belongs to the window's one volume owner, [QVolume](../Media/Audio/QVolume.comment.md).
That owner hears its moves, sets and saves the level and paints it.
The rows these buttons play are drawn by [QLecternAccent](QLecternAccent.comment.md).

## `public void QLecternPlaybackIntroduce(UIElement action, UIElement tray, RangeBase volume)`

Holds the play button, the tray and the slider.
It hooks nothing on the slider, since the volume owner already hears it.

## `public void QLecternPlaybackRefine()`

Shows the play button and the volume tray by the verdicts the area answers for the shown entry.

## `public void QLecternTrayRefine()`

Hides the play button and the volume tray.

## `public void QLecternPlaybackObserve(object parameter)`

Hears an accent row's play command and hands its recording and the slider's level to the playback gate.
Anything but an accent row is ignored.

## `public void QLecternActionObserve()`

Hears the play button and hands the slider's level to the playback gate for the shown draft's own recording.
