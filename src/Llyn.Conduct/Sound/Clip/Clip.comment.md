# Clip

This folder holds the audio-recording popup with its session state, rows, readings and harvest steps.

## `CClip`

The clip popup's session state, which the errand holds and fills from its recording search.
It keeps the source rows in the pack's order, one row per source, and whether the search still runs.
Only the errand reaches it, so no driver holds the list.
Each reading keeps its own preview and taking state, so a repaint shows it as it stands.

`LClipStart` empties the rows and forgets the marked preview.
`LClipFinish` keeps the rows, so the popup still lists what was found.
The errand calls it when the search ends, never starts, or throws at its start.
`LClipRead` has the notice say searching while the search runs, and empty once it ended.

`LClipPlace` finds or creates the row at one source's declared order.
Rows stand where the language pack put the source, not where the network put it.
Every source is asked at once, so a fast one would otherwise head a list the user did not order.
A new row opens with the searching notice.

`LClipRecordingAdd` appends a recording with an address and makes the row takeable.
No address reads as missing when the source answered, and broken when it never did.
An empty answer after a recording has landed changes nothing, so a found row never falls back to a notice.

`LClipReadingRead` can run before the start returns its foray.
A pronunciation quill over the held tenure answers then.
A new reading is ready to take and shows no preview state.

`LClipPreviewStart` clears the last preview, then marks this recording fetching and lifts its refusal.
`LClipPreviewPlay` answers false when another preview took over meanwhile.
`LClipRefusedSet` marks a failed fetch even when another preview took over.
`LClipSaveStart` answers false for a recording that is not ready, so a second press during a download starts nothing.
`LClipSaveFinish` leaves a saved recording not ready, and a failed one offers the retry.
`LClipReadingSet` changes nothing for a recording no longer listed, once a new search cleared the rows.

## `CClipItem`, `CClipReading`, `CClipRoll`, `CRecording`, `CHarvestStep`

The clip popup's rows, readings and state are answered by the errand.
It answers them from the start gate, the flag load and each search event.
A row appears the moment its source starts searching, so every declared source shows before any answers.
It is replaced in place as its source answers, so a row never jumps.
The notice key of a row is empty once a recording landed.

A reading's variety is blank for an untagged recording, whose flag key finds nothing.
The refused flag lasts until the same recording is previewed again.
The action key reads saving, saved or retry once the user took the recording.
`CRecording` keeps every field of the engine's recording, since the clip plays and saves the one it listed.
`CHarvestStep` carries no kind, since a found recording or the end flag already tells the step apart.

## Stamp

Commit: `c2f335c331fe`
- `CClip.cs` `6050c76e8cea0348`
- `CClipItem.cs` `e294fb2ad85bc190`
- `CClipReading.cs` `5d6edc4fa39063e8`
- `CClipRoll.cs` `71c16169b341a872`
- `CHarvestStep.cs` `09213bc87a0aa1cf`
- `CRecording.cs` `035955e38d4b2fe8`
