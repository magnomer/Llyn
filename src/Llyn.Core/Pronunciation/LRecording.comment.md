# LRecording.cs

## `public sealed record LRecording(string LRecordingSource, string LRecordingAddress);`

One downloadable audio recording found for a headword by an audio source.

**Parameters**

- `LRecordingSource` — The name of the source the recording came from, as declared by that source's language pack (for example `"Naver"`). Config-driven, never an enum.
- `LRecordingAddress` — The absolute URL the audio bytes are retrieved from.
