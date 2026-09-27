# QErrand.cs

## `public sealed class QErrand`

The recording and reading searches one desk runs over its held tenure.
The desk builds it over itself, so no driver ever holds a foray.

## `private LForay? _qErrandRecording;`

The recording search running over the held tenure.
`_qErrandTranscription` is the reading search, kept apart since the engine runs both kinds side by side.

## `internal QErrand(LDesk desk)`

Only the desk builds one, over itself.

## `internal LForay? QErrandRecording`

The running recording search, read by the clip for its language, target and save.
`QErrandTranscription` hands the reading search to the notation the same way.

## `public bool QErrandRecordingStart(string word, long target, Action<CHarvestStep> sink)`

Starts a recording search over the held tenure and reports whether one started.
Nothing starts while no tenure is held.
The previous recording search is cancelled first.
Each engine step reaches the sink as a Conduct copy.
`QErrandTranscriptionStart` starts a reading search the same way, under one scheme.

## `public void QErrandCancel()`

Stops both searches.
The clip and notation popups never stay open together, so stopping both stops only the one running.

## `private static LForay? QErrandRecordingRun(LTenure? tenure, string word, long target, Action<LHarvestStep> sink)`

Starts the search over the tenure handed in, or nothing when none is held.
The tenure and the forays arrive as parameters, so no field decides the start.
`QErrandTranscriptionRun` and `QErrandForayStop` take their handles the same way.

## `internal static CRecording? QErrandRecordingRead(LRecording? recording)`

Copies an engine recording into its Conduct shape, or none for none.
The overload over `CRecording` copies it back, since the clip saves and plays what it listed.
`QErrandCandidateRead` copies a reading out the same way.
