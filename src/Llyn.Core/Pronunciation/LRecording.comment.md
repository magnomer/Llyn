# LRecording.cs

## `public sealed record LRecording(string LRecordingSource, string LRecordingAddress);`

One downloadable audio recording found for a headword by an audio source.

**Parameters**

- `LRecordingSource` — The name of the source the recording came from.
  It is the name that source's language pack declares (for example `"Naver"`).
  Config-driven, never an enum.
- `LRecordingAddress` — The absolute URL the audio bytes are retrieved from.
