# CErrand.cs
Hash: `7632751047e7ab12`

## `public sealed class CErrand`

The recording and reading searches one desk runs over its held tenure.
The desk builds it over itself, so no driver ever holds a foray.
It also keeps the clip popup's state, which the recording search fills.
The notation popup's state, which the reading search fills, sits beside it.

## `private LForay? _cErrandRecording;`

The recording search running over the held tenure.
`_cErrandTranscription` is the reading search, kept apart since the engine runs both kinds side by side.

## `private readonly CClip _cErrandClip = new();`

The clip popup's session state, which the recording search fills and each start clears.

## `private readonly CNotation _cErrandNotation = new();`

The notation popup's session state, which the reading search fills and each reading start clears.

## `private Action<Action> _cErrandMarshal = static run => run();`

Hands each recording or reading step to the driver's thread before any state changes or event rises.
It runs the step at once until the desk attaches the driver's marshal.

## `internal CErrand(CDesk desk)`

Only the desk builds one, over itself.

## `public event Action<CClipRoll>? CErrandClipChanged;`

Raised with the ready clip state whenever it changes.
A starting source, a found recording and the search's end each raise it once.
So do the stages of a preview and of a taking, since each changes a recording's look.
The popup repaints from it and needs to tell no cause apart.

## `public event Action<CNotationRoll>? CErrandNotationChanged;`

Raised with the ready notation state whenever the reading search changes it.
A starting source, a found reading and the search's end each raise it once.
The popup repaints from it and needs to tell no cause apart.

## `public CClipRoll CErrandRecordingStart(long target)`

The user asks for recordings of the headword on one pronunciation row.
It stops both searches, clears the popup and starts a recording search over the held tenure.
The tenure reads the draft's headword itself and starts nothing for a blank one.
The clip is marked searching before the start, since a step may land during it.
A start that answers nothing or fails leaves the popup finished with its empty notice.
Each engine step reaches the popup's state through the marshal.

## `public async Task<CClipRoll> CErrandEnsignLoad(`

Loads the flags of the running search's varieties, then answers the clip state to repaint.
The foray loads nothing when its language shows no flags.
A failed load leaves the flags out and the search running.

## `public CNotationRoll CErrandTranscriptionStart(long target, string scheme)`

The user asks for readings of the headword for one row, in IPA or in the row's scheme.
It stops both searches first, as the clip popup's start does, and answers the popup's state to paint.
The tenure reads the draft's headword itself and starts nothing for a blank one.
The popup is marked searching before the start, since a step may land during it.
A start that answers nothing or fails leaves the popup finished with its empty notice.
A schemed start picks the transcription notice, and a plain one the reading notice.
Each engine step reaches `LErrandLookupResonate` through the marshal, with the foray it came from.

## `public async Task<CNotationRoll> CErrandFlagLoad(`

Loads the flags of the running reading search's varieties, after the start.
The foray loads nothing for a schemed search or a language that shows no flags.
It then answers the notation state, so readings that landed before the flags repaint with them.
A failed load leaves the flags out and still answers the state.

## `public void CErrandReadingSet(string phonetic, string variety)`

The user takes one listed reading into the row the search was opened for.
It does nothing while no reading search runs or the desk fills.
The quill routes the reading by the search's target and writes it at once.

## `internal void LErrandObserverAttach(Action<Action> marshal)`

Takes the marshal the desk received from its driver.

## `internal void LErrandHarvestResonate(CHarvestStep step)`

Answers one step of the recording search on the driver's thread.
A found recording, the end, or a starting source each changes the clip state and raises it.

## `internal void LErrandLookupResonate(CLookupStep step, LForay foray)`

Answers one step of the reading search on the driver's thread.
A found reading, the end, or a starting source each changes the notation state and raises it.
The step's own foray answers the reading's mark and text, even before the start has returned it.
A step whose foray was cancelled belongs to a replaced search, so it changes nothing and raises nothing.
The foray's own mark decides it, since a step may land before the start has returned the current foray.

## `public async Task<Uri?> CErrandPreviewStart(CRecording recording)`

The user asks to hear one listed recording.
It marks that recording fetching, and the foray fetches it into the workspace.
Streaming the remote, token-bearing address through the media stack is unreliable, so a local file plays.
It answers the file's address once the recording is marked playing.
It answers nothing when another preview took over or the popup closed during the fetch.
A failed fetch marks the recording refused and answers nothing, so a refusal is not mistaken for silence.
Only one preview is marked at a time, so starting another clears the last.
It does nothing while no recording search runs.

## `public void CErrandPreviewFinish()`

The preview sound ended or failed, so the marked recording returns to plain.
It raises the clip only when a preview was marked, since the player is shared with other sounds.

## `public async Task<bool> CErrandRecordingSave(CRecording recording)`

The user takes one listed recording, and it answers whether the popup closes.
It does nothing while no recording search runs or the recording is not ready to take.
The download is reported on the recording taken, not on the popup's notice, which belongs to the search.
The foray downloads it, attaches it and tags its row with its variety.
A taken recording then reads saved, even when the draft moved on and nothing was attached.
Only an attached recording closes the popup.
A failed download offers another try and asks the user nothing, as before.

## `public void CErrandCancel()`

Stops both searches and forgets the marked preview, so a fetch that lands later plays nothing.
The clip and notation popups never stay open together, so stopping both stops only the one running.

## `private static LForay? LErrandRecordingRun(LTenure? tenure, long target, Action<LHarvestStep> sink)`

Starts the search over the tenure handed in, or nothing when none is held.
The tenure and the forays arrive as parameters, so no field decides the start.
`LErrandEnsignRun`, `LErrandTranscriptionRun` and `CErrandForayStop` take their handles the same way.

## `internal static CRecording? CErrandRecordingRead(LRecording? recording)`

Copies an engine recording into its Conduct shape, or none for none.
The overload over `CRecording` copies it back, since the clip saves and plays what it listed.
`CErrandCandidateRead` copies a reading out the same way.
