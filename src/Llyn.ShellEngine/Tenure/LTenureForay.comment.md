# LTenureForay.cs
Hash: `fb97936519a7f4b2`

## `public sealed partial class LTenure`

The searches a tenure starts for its draft, one recording search and one lookup at a time.
Each start cancels the last search of its kind, so a menu moved to another row never hears two.
The forays are dropped with the tenure, whether it commits or cancels.

## `private LForay? _lTenureRecordingForay;`

The recording search in flight, or null before the first one.

## `private LForay? _lTenureTranscriptionForay;`

The pronunciation or transcription lookup in flight, or null before the first one.

## `public LForay? LTenureRecordingStart(long target, Action<LHarvestStep> sink)`

Starts a recording search for the draft's headword on the row `target`, streaming each step to `sink`.
It answers the foray, or none for a blank headword or a draft raised in a workspace no longer open.
Waiting keystrokes are written first, so the headword read is the one the form shows.
The word is trimmed by `LForay.LForayWordRead`, the same rule the pick checks against.
The shell hands a delegate and implements no listener.
The engine streams every step to it, the end step included.
The tenure wraps `sink` in the one `LListenerRelay` the search runs through.
It owns the end only when that relay never sent one.
So a fault before the harvest starts, such as a pack load or source build, still ends the sink once.
The language is the draft's at this moment, and the foray keeps it for the pick to check against.
A stale draft is expected after a workspace change and is no failure, so only its refusal is caught here.
Every other failure still reaches the caller.

## `public LForay? LTenureTranscriptionStart(long target, string scheme, Action<LForay, LLookupStep> sink)`

Starts a lookup for the draft's headword on the row `target`, streaming each step to `sink`.
It answers the foray, or none for a blank headword or a draft raised in a workspace no longer open.
Waiting keystrokes are written first, and `LForay.LForayWordRead` trims the word, as for recordings.
The shell hands a delegate and implements no receiver.
The engine streams every step to it, the end step included.
The tenure wraps `sink` in the one `LReceiverRelay` the lookup runs through.
It owns the end only when that relay never sent one.
A named scheme runs that scheme's transcription sources, and an empty one runs the pronunciation lookup.
The one menu opens both, so the one start serves both.
Each step reaches `sink` with its own foray, so a step landing before the start returns still reads its mark.
A stale draft starts nothing, as for recordings.

## `private void LTenureForayStop()`

Cancels both searches in flight, if any.
Its callers hold the gate.
