# LRecording.cs

## `public sealed record LRecording(string LRecordingSource, string? LRecordingAddress, int LRecordingOrder, bool LRecordingReached)`

What one audio source had to say about a headword.
Every source produces exactly one, exactly as `LCandidate` does for a lookup.

**Parameters**

- `LRecordingSource` — The name of the source the recording came from.
  It is the name that source's language pack declares (for example `"Naver"`).
  Config-driven, never an enum.
- `LRecordingAddress` — The absolute URL the audio bytes are retrieved from, or null when the source offered none.
- `LRecordingOrder` — The source's position in the pack's `audio` list.
  It carries the declared order to the menu, which fetching in parallel would otherwise lose.
- `LRecordingReached` — Whether the source answered at all.
  A row with no address reads as "no entry" or as "failed to retrieve" from this alone.
