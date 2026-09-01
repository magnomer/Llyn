# LPronunciationAudio.cs

## `public sealed record LPronunciationAudio(`

The recorded audio a pronunciation owns: one downloaded file per pronunciation, stored as a path relative to the workspace folder so moving or copying a workspace keeps its audio. The owning pronunciation id is the identity — at most one audio row per pronunciation — so saving replaces whatever file the pronunciation had.

**Parameters**

- `LPronunciationAudioId` — Owning pronunciation id — the row's identity.
- `LPronunciationAudioFile` — Audio file path, relative to the workspace folder.
- `LPronunciationAudioSource` — Optional label of the source the recording came from.
- `LPronunciationAudioAdded` — Time the file was recorded into the workspace, ISO 8601 UTC.
