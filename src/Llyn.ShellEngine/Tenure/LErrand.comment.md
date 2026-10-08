# LErrand.cs
Hash: `36813afa1d154c0b`

## `public sealed class LErrand`

The searches a tenure starts for its draft, and the draft writes a reading lookup ends in.
It runs one recording search and one lookup at a time.
Each start cancels the last search of its kind, so a menu moved to another row never hears two.
The forays are dropped with the tenure, whether it commits or cancels.
Only the start and the apply live here, while the lookup's menu stays in Conduct's `CErrand`.

## `private readonly LEngine _lEngine;`

The engine the searches run on.

## `private readonly LTenure _lErrandTenure;`

The tenure whose draft is searched for, and every request is built for and handed to.

## `private readonly object _lErrandGate;`

The tenure's own gate, so a start and the tenure's end never swap a foray at once.

## `private readonly LQuillPronunciation _lErrandPronunciation;`

The one pronunciation quill that tags a written row with its variety.
Each foray shares it, so a saved recording tags its row through the same quill.

## `private LForay? _lErrandRecording;`

The recording search in flight, or null before the first one.

## `private LForay? _lErrandTranscription;`

The pronunciation or transcription lookup in flight, or null before the first one.

## `internal LErrand(LEngine engine, LTenure tenure, object gate)`

Made by the tenure alone, once, and read through `LTenure.LTenureErrand`.
The tenure hands its gate, so the forays share the lock its cancel and finish hold.

## `public LForay? LErrandRecordingStart(long target, Action<LHarvestStep> sink)`

Starts a recording search for the draft's headword on the row `target`, streaming each step to `sink`.
It answers the foray, or none for a blank headword or a draft raised in a workspace no longer open.
Waiting keystrokes are written first, so the headword read is the one the form shows.
The word is trimmed by `LForay.LForayWordRead`, the same rule the pick checks against.
The shell hands a delegate and implements no listener.
The engine streams every step to it, the end step included.
The errand wraps `sink` in the one `LListenerRelay` the search runs through.
It owns the end only when that relay never sent one.
So a fault before the harvest starts, such as a pack load or source build, still ends the sink once.
The language is the draft's at this moment, and the foray keeps it for the pick to check against.
A stale draft is expected after a workspace change and is no failure, so only its refusal is caught here.
Every other failure still reaches the caller.

## `public LForay? LErrandTranscriptionStart(long target, string scheme, Action<LForay, LLookupStep> sink)`

Starts a lookup for the draft's headword on the row `target`, streaming each step to `sink`.
It answers the foray, or none for a blank headword or a draft raised in a workspace no longer open.
Waiting keystrokes are written first, and `LForay.LForayWordRead` trims the word, as for recordings.
The shell hands a delegate and implements no receiver.
The engine streams every step to it, the end step included.
The errand wraps `sink` in the one `LReceiverRelay` the lookup runs through.
It owns the end only when that relay never sent one.
A named scheme runs that scheme's transcription sources, and an empty one runs the pronunciation lookup.
The one menu opens both, so the one start serves both.
Each step reaches `sink` with its own foray, so a step landing before the start returns still reads its mark.
A stale draft starts nothing, as for recordings.

## `internal void LErrandStop()`

Cancels both searches in flight, if any.
The tenure calls it from cancel and finish while holding its gate.

## `public void LErrandReadingSet(LForay foray, string phonetic, string variety)`

Writes a reading the user took from the lookup `foray` into the row it was opened for.
A pick is one deliberate act, so each write applies at once rather than waiting like typing.
A schemed lookup writes the transcription row's text and names no variety.
Otherwise the primary pronunciation or the accent row takes the IPA, then the variety through `LQuillPronunciation.LQuillVarietySet`.
A row removed while the menu was open is skipped, since the clerk refuses an unknown row.

## `private static bool LErrandRowCheck<LErrandRow>(IReadOnlyList<LErrandRow>? rows, long id, Func<LErrandRow, long> key)`

Whether the draft still lists the row `id`, through the clerk's own `LDraftListFind`.
No draft or no row id lists nothing.
