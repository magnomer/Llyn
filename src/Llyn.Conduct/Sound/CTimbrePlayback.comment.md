# CTimbrePlayback.cs
Hash: `2aea6bb0d1a8aaf4`

## `public sealed record CTimbrePlayback(string? CTimbrePlaybackAudio, bool CTimbrePlaybackAudible);`

The playback facts of the draft an editor holds, ready to paint.

**Parameters**

- `CTimbrePlaybackAudio`: the draft's own recording while its file exists, else null, which shows the play button.
- `CTimbrePlaybackAudible`: whether the own recording or any accent row has audio, which shows the volume tray.
