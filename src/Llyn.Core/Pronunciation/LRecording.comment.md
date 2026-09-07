# LRecording.cs

## `public sealed record LRecording(string LRecordingSource, string LRecordingAddress, int LRecordingOrder);`

One downloadable audio recording found for a headword by an audio source.

**Parameters**

- `LRecordingSource` — The name of the source the recording came from.
  It is the name that source's language pack declares (for example `"Naver"`).
  Config-driven, never an enum.
- `LRecordingAddress` — The absolute URL the audio bytes are retrieved from.
- `LRecordingOrder` — The source's position in the pack's `audio` list.
  It carries the declared order to the menu, which fetching in parallel would otherwise lose.
