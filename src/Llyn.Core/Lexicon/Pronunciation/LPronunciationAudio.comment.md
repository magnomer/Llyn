# LPronunciationAudio.cs

## `public sealed record LPronunciationAudio(`

The recorded audio a pronunciation owns, one downloaded file per pronunciation.
It is stored as a path relative to the workspace folder.
So moving or copying a workspace keeps its audio.
The owning pronunciation id is the identity, so there is at most one audio row per pronunciation.
Saving replaces whatever file the pronunciation had.

**Parameters**

- `LPronunciationAudioFile` — Audio file path, relative to the workspace folder.
- `LPronunciationAudioSource` — Optional label of the source the recording came from.
