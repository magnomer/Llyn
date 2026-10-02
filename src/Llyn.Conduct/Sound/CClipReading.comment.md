# CClipReading.cs
Hash: `5d6edc4fa39063e8`

## `public sealed record CClipReading(`

One recording on a source row, with its variety keys ready.
It also carries the recording's preview and taking state.
The driver looks up the label and the flag, and decides nothing about them.

**Parameters**

- `CClipReadingRecording`: the recording, which preview fetches and taking saves.
- `CClipReadingVariety`: the variety's name, key and flag key under the search's language.
  An untagged recording carries a blank variety, whose flag key finds nothing.
- `CClipReadingFlagged`: whether the search's language shows its varieties as flags.
- `CClipReadingAction`: the key of the taking button's label.
  It reads saving, saved or retry once the user took the recording.
- `CClipReadingReady`: whether the user can take the recording now.
- `CClipReadingFetching`: whether the recording's preview is being fetched.
- `CClipReadingPlaying`: whether the recording's preview is the one now sounding.
- `CClipReadingRefused`: whether the last preview fetch of the recording failed.
  It lasts until the same recording is previewed again.
