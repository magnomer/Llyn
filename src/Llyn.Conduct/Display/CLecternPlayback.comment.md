# CLecternPlayback.cs

## `public sealed record CLecternPlayback(bool CLecternPlaybackRecorded, bool CLecternPlaybackAudible);`

The playback verdicts of the reading view for the shown entry.

**Parameters**

- `CLecternPlaybackRecorded`: whether the draft's own recording exists, which shows the play button.
- `CLecternPlaybackAudible`: whether anything plays at all, which shows the volume tray.
