# QLecternPlayback.cs
Hash: `096bab48cccf621e`

## `public sealed class QLecternPlayback`

The reading view's playback driver, over [CDisplayPlayback](../../Llyn.Conduct/Display/CDisplayPlayback.comment.md).
It drives the play button and the volume tray, and plays at the page's slider.
The slider itself belongs to the window's one volume owner, [QVolume](../Media/Audio/QVolume.comment.md).
That owner hears its moves, sets and saves the level and paints it.
The rows these buttons play are drawn by [QLecternAccent](QLecternAccent.comment.md).

## `public QLecternPlayback(FrameworkElement surface, CDisplayPlayback area)`

Pulls the play button, the tray, the slider and the accent list from the page by contract ID.
`area` is the display's playback area, the only part it reads.
It binds the accent list's playback command and subscribes the play button.
It hooks nothing on the slider, since the volume owner already hears it.

## `public void QLecternPlaybackRefine()`

Shows the play button and the volume tray by the verdicts the area answers for the shown entry.
The lectern subscribes it to both open and close, since a closed display answers nothing to play.

## `private void QLecternPlaybackObserve(object sender, ExecutedRoutedEventArgs e)`

Hears an accent row's play command and hands its recording and the slider's level to the playback gate.
Anything but an accent row is ignored.

## `private void QLecternActionObserve(object sender, RoutedEventArgs e)`

Hears the play button and hands the slider's level to the playback gate for the shown draft's own recording.
