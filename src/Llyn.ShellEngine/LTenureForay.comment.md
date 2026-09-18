# LTenureForay.cs

## `public sealed partial class LTenure`

The searches a tenure starts for its draft, one recording search and one lookup at a time.
Each start cancels the last search of its kind, so a menu moved to another row never hears two.
The forays are dropped with the tenure, whether it commits or cancels.

## `private LForay? _lTenureRecordingForay;`

The recording search in flight, or null before the first one.

## `private LForay? _lTenureTranscriptionForay;`

The pronunciation or transcription lookup in flight, or null before the first one.

## `public LForay LTenureRecordingStart(string word, long target, LListener listener)`

Starts a recording search for `word` on the row `target`, streaming to `listener`, and answers the foray.
The language is the draft's at this moment, and the foray keeps it for the pick to check against.

## `public LForay LTenureTranscriptionStart(string word, long target, string scheme, LReceiver receiver)`

Starts a lookup for `word` on the row `target`, streaming to `receiver`, and answers the foray.
A named scheme runs that scheme's transcription sources, and an empty one runs the IPA lookup.
The one menu opens both, so the one start serves both.

## `private void LTenureForayStop()`

Cancels both searches in flight, if any.
Called under the gate.
