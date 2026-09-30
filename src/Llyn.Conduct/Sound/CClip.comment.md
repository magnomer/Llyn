# CClip.cs

## `internal sealed class CClip`

The clip popup's session state, which the errand holds and fills from its recording search.
It keeps the source rows in the pack's order, one row per source, and whether the search still runs.
Each reading on a row keeps its own preview and taking state, so a repaint shows it as it stands.
Only the errand reaches it, so no driver holds the list.

## `private CRecording? _cClipPreview;`

The recording whose preview is marked, fetching or playing, or none.

## `internal void LClipStart()`

Empties the rows, forgets the marked preview and marks the search running.
`LClipFinish` marks it ended.

## `internal CClipRoll LClipRead()`

Answers the state as the popup paints it.
The notice says searching while the search runs, and empty once it ended.

## `internal int LClipPlace(string source, int order)`

Finds the row one source owns, creating it at its declared place when it has none yet.
Rows stand where the language pack put the source, not where the network put it.
Every source is asked at once, so a fast one would otherwise head a list the user did not order.
A new row opens with the searching notice.

## `internal void LClipRecordingAdd(CRecording recording, LForay? foray, LTenure? tenure)`

Resolves the row of one source from one thing it said.
A recording with an address is appended and makes the row takeable.
No address reads as missing when the source answered, and broken when it never did.
An empty answer after a recording has landed changes nothing, so a found row never falls back to a notice.

## `private static CClipReading LClipReadingRead(LForay? foray, LTenure? tenure, CRecording recording)`

Gives one recording its variety keys under the search's language and flag mode.
A step can land before the start returns its foray, so the held tenure answers then.
A new reading is ready to take and shows no preview state.

## `internal void LClipPreviewStart(CRecording recording)`

Clears the last preview, then marks this recording fetching and lifts its refusal.

## `internal bool LClipPreviewPlay(CRecording recording)`

Marks the fetched recording playing, and answers false when another preview took over meanwhile.

## `internal void LClipRefusedSet(CRecording recording)`

Marks a recording whose fetch failed, even when another preview took over meanwhile.

## `internal bool LClipPreviewFinish()`

Returns the marked recording to plain, and answers whether one was marked.

## `internal bool LClipSaveStart(CRecording recording)`

Marks a ready recording saving and not ready, and answers false for one that is not ready.
So a second press during a download starts nothing.

## `internal void LClipSaveFinish(CRecording recording, bool saved)`

A saved recording reads saved and stays not ready, and a failed one offers the retry.

## `private CClipReading? LClipReadingFind(CRecording recording)`

The listed reading of a recording, or none once a new search cleared the rows.

## `private void LClipReadingSet(CRecording recording, Func<CClipReading, CClipReading> change)`

Replaces the reading of a recording with its changed copy, in place on its row.
A recording no longer listed changes nothing.
