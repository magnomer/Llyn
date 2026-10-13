# Errand

This folder holds the recording and transcription searches a desk runs.
It also holds the candidates and recordings they return.

## `TCandidate`

Covers the reading a driver lists in the notation popup.
It is regional only while it names a variety.

## `TErrand`

Covers the searches a desk runs and the maps between engine recordings or candidates and their Conduct shapes.
A recording travels both ways unchanged, since the clip saves and plays what it listed.
A candidate travels out unchanged, and a missing recording or candidate maps to none.
With no tenure held, neither search starts, and both popups answer their empty notice.
Each recording step raises the clip once, and a row turns ready only when its recording lands.
Rows keep the pack's order once each, and a row without an address says missing or broken.
An empty answer after a recording keeps the recording, and an untagged one carries a blank flag key.
A pick saved or previewed while no recording search runs does nothing.
A start over a tenure the workspace change made stale shows no notice and leaves each popup finished.
The stale-start desk is an Input desk holding a persisted entry headed "tomato", whose notices land in a recorded list.

## `TErrandClip`

Covers the clip popup's gates end to end on a real editor.
The run goes from start and flag load to preview and taking.
A recording search over the held draft fills the clip with the source, its recording and the end.
The recording lands with its variety keys, and a blank headword answers the empty notice.
The flag load stores the variety flags of a flagged pack, and nothing while no search runs.
A step that lands mid-fetch keeps the preview's fetching mark, which turns playing once the file lands.
The finish returns the recording to plain once, and a refused fetch marks the recording refused.
A popup cancelled mid-fetch plays nothing.
A taking reads saving while a step lands, starts nothing on a second press, then reads saved.
It tags the primary row with the recording's variety, and a refused download offers the retry.
The fault paths of the same gates live in `TErrandFault`.
Editors and recording searches come from `TInterfaceConductDesk`, which both classes share.
The full-run test's pack lists the British variety and one source, whose answer yields one recording.
Most other tests use an empty pack and raise their steps by hand.

## `TErrandFault`

Covers the clip popup's gates when the host or the recording store fails, on a real editor.
A broken host shows the recording notice once, for the preview, the taking and the flag load alike.
The preview still marks the recording refused, and the taking still offers the retry.
A recording store that raises a vault fault shows the same notice and records the fault once.
The preview then marks the recording refused, and the taking offers the retry with nothing attached.
The engine's recording store faults on every save and fetch, and its audit recorder collects each fault.

## `TErrandNotation`

Covers the notation popup's start and flag load end to end on a real editor.
A padded headword starts a search whose every step passes through the desk's marshal and raises a notation state.
The marshal queues each step and the test runs them in turn, so the gated source fixes their order.
The draft is read through the desk before the marshal attaches, so exactly the search's three steps queue.
A step of a search replaced by a later start lands after it and lists nothing.
A blank headword starts nothing, answers the empty notice and still stops a running recording search.
The flag load stores the variety flags for an IPA search only, and answers the state to repaint.
The reading pick lives in `TErrandNotationReading`, and the listed rows live in `TErrandNotationRoll`.

## `TErrandNotationPack`

A one-source pack whose IPA source answers one British reading.

## `TErrandStepRead`

Takes the next step the desk's marshal queued, failing after five seconds so a lost step cannot hang the run.

## `TErrandNotationRead`

Reads a notation state as each row's source and reading texts, for comparing the raised states in order.

## `TErrandNotationPrepare`

Opens a new draft in the pack's language with the headword typed, as the input tab holds it.
`TErrandNotationRoll` and most of `TErrandNotationReading` build their editor through it.

## `TErrandNotationReading`

Covers how the notation popup writes a picked reading into the draft, on a real editor.
It takes its pack from `TErrandNotation.TErrandNotationPack`.
It builds its editors through `TErrandNotation.TErrandNotationPrepare`, except the schemed one, which opens a stored entry.
A pick writes the primary IPA and its variety at once, before any persist.
An accent row takes the pick, and a row removed since the start takes nothing.
A schemed search writes the transcription text at once and names no variety.
No search, or a desk still filling, writes nothing.

## `TErrandNotationRoll`

Covers the rows the notation popup lists as its search answers, on a real editor.
It builds its editors through `TErrandNotation.TErrandNotationPrepare`.
Its missing and ordering tests take their pack from `TErrandNotation.TErrandNotationPack`.
A found reading lists its text and mark as the search's settings answer them.
A reading shows a flag only when it names a variety.
A schemed search lists the phonetic bare, whatever the respelling switch shows.
A source without a reading says missing or broken, and an empty answer after a reading keeps it.
Rows keep the pack's order once each, and the end of a schemed search reads the transcription notice.

## `TRecording`

Covers the recording the clip popup lists.
It is addressed only while it carries a non-empty address.

## Stamp

Commit: `c2f335c331fe`
- `TCandidate.cs` `ea5097af3de7a77c`
- `TErrand.cs` `b09faf6aa61a038d`
- `TErrandClip.cs` `8f7981aa9bfcfdf1`
- `TErrandFault.cs` `fc7504c00cfc67f5`
- `TErrandNotation.cs` `2cf220a9fd7b420f`
- `TErrandNotationReading.cs` `d64260cf4771f331`
- `TErrandNotationRoll.cs` `839f0a57d04fd60d`
- `TRecording.cs` `334b415330962833`
