# CErrand.cs

## `public sealed class CErrand`

The recording and reading searches one desk runs over its held tenure.
The desk builds it over itself, so no driver ever holds a foray.
It also keeps the clip popup's state, which the recording search fills.

## `private LForay? _cErrandRecording;`

The recording search running over the held tenure.
`_cErrandTranscription` is the reading search, kept apart since the engine runs both kinds side by side.

## `private readonly CClip _cErrandClip = new();`

The clip popup's session state, which the recording search fills and each start clears.

## `private Action<Action> _cErrandMarshal = static run => run();`

Hands each recording step to the driver's thread before the clip state changes.
It runs the step at once until the desk attaches the driver's marshal.

## `internal CErrand(CDesk desk)`

Only the desk builds one, over itself.

## `public event Action<CClipRoll>? CErrandClipChanged;`

Raised with the ready clip state whenever it changes.
A starting source, a found recording and the search's end each raise it once.
So do the stages of a preview and of a taking, since each changes a recording's look.
The popup repaints from it and needs to tell no cause apart.

## `public event Action<string, int>? CErrandLookupStarted;`

Raised when a reading source starts searching.
`CErrandCandidateAdded` follows a found reading, and `CErrandLookupFinished` the end of the search.
The notation menu repaints from these events.

## `public bool CErrandTranscriptionHeld`

Whether a reading search runs.
The reads below answer its language, flag mode, primary mark, target row and scheme.
Each answers false, zero or empty while none runs.

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

## `public bool CErrandTranscriptionStart(string word, long target, string scheme, Action<CLookupStep> sink)`

Starts a reading search over the held tenure and reports whether one started.
Nothing starts while no tenure is held.
The previous reading search is cancelled first.
Each engine step reaches the sink as a Conduct copy.

## `internal void LErrandObserverAttach(Action<Action> marshal)`

Takes the marshal the desk received from its driver.

## `internal void LErrandHarvestResonate(CHarvestStep step)`

Answers one step of the recording search on the driver's thread.
A found recording, the end, or a starting source each changes the clip state and raises it.

## `public void CErrandLookupResonate(CLookupStep step)`

Answers one step of the reading search, which the driver hands over on its own thread.
A found reading, the end, or a starting source each raises its own event.

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
`LErrandEnsignRun`, `CErrandTranscriptionRun` and `CErrandForayStop` take their handles the same way.

## `internal static CRecording? CErrandRecordingRead(LRecording? recording)`

Copies an engine recording into its Conduct shape, or none for none.
The overload over `CRecording` copies it back, since the clip saves and plays what it listed.
`CErrandCandidateRead` copies a reading out the same way.
