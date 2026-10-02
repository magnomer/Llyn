# LTenureForay.cs
Hash: `00b7428a48ccaa63`

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
It answers the foray, or none for a blank headword.
Waiting keystrokes are written first, so the headword read is the one the form shows.
The word is trimmed by `LForay.LForayWordRead`, the same rule the pick checks against.
The shell hands a delegate and implements no listener, and the engine streams every step to it.
A relay over the sink is built here only to give the foray its end step when the search faults.
The language is the draft's at this moment, and the foray keeps it for the pick to check against.

## `public LForay? LTenureTranscriptionStart(long target, string scheme, Action<LForay, LLookupStep> sink)`

Starts a lookup for the draft's headword on the row `target`, streaming each step to `sink`.
It answers the foray, or none for a blank headword.
Waiting keystrokes are written first, and `LForay.LForayWordRead` trims the word, as for recordings.
The shell hands a delegate and implements no receiver, and a relay here gives the foray its end step.
A named scheme runs that scheme's transcription sources, and an empty one runs the IPA lookup.
The one menu opens both, so the one start serves both.
Each step reaches `sink` with its own foray, so a step landing before the start returns still reads its mark.

## `private void LTenureForayStop()`

Cancels both searches in flight, if any.
Called under the gate.
