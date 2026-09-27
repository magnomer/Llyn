# CErrand.cs

## `public sealed class CErrand`

The recording and reading searches one desk runs over its held tenure.
The desk builds it over itself, so no driver ever holds a foray.

## `private LForay? _cErrandRecording;`

The recording search running over the held tenure.
`_cErrandTranscription` is the reading search, kept apart since the engine runs both kinds side by side.

## `internal CErrand(CDesk desk)`

Only the desk builds one, over itself.

## `internal LForay? CErrandRecording`

The running recording search, read by the clip for its language, target and save.
`CErrandTranscription` hands the reading search to the notation the same way.

## `public bool CErrandRecordingStart(string word, long target, Action<CHarvestStep> sink)`

Starts a recording search over the held tenure and reports whether one started.
Nothing starts while no tenure is held.
The previous recording search is cancelled first.
Each engine step reaches the sink as a Conduct copy.
`CErrandTranscriptionStart` starts a reading search the same way, under one scheme.

## `public void CErrandCancel()`

Stops both searches.
The clip and notation popups never stay open together, so stopping both stops only the one running.

## `private static LForay? CErrandRecordingRun(LTenure? tenure, string word, long target, Action<LHarvestStep> sink)`

Starts the search over the tenure handed in, or nothing when none is held.
The tenure and the forays arrive as parameters, so no field decides the start.
`CErrandTranscriptionRun` and `CErrandForayStop` take their handles the same way.

## `internal static CRecording? CErrandRecordingRead(LRecording? recording)`

Copies an engine recording into its Conduct shape, or none for none.
The overload over `CRecording` copies it back, since the clip saves and plays what it listed.
`CErrandCandidateRead` copies a reading out the same way.
