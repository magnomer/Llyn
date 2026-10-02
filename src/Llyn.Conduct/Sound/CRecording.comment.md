# CRecording.cs
Hash: `035955e38d4b2fe8`

## `public sealed record CRecording(`

One recording a search found, as the clip popup lists it.
It keeps every field of the engine's recording, since the clip plays and saves the one it listed.

**Parameters**

- `CRecordingSource`: the name of the source that answered.
- `CRecordingAddress`: where the audio lives, or null when the source had none.
- `CRecordingOrder`: the source's place in the pack, so rows keep the pack's order.
- `CRecordingReached`: whether the source answered at all, which picks the notice for a missing address.
- `CRecordingVariety`: the variety the audio speaks, empty when the source names none.

## `public bool CRecordingAddressed`

Whether the recording carries an address, so the row offers to play it.
