# LTrove.cs

## `internal sealed class LTrove`

The tentative store of what a pronunciation lookup or an audio discovery already returned.
An answer is kept against the draft the asking editor holds, never against the word alone.
So two editors open at once keep their own answers, and neither reads the other's.
An entry lives exactly as long as its draft does, and the engine drops it when that draft closes.
It is memory only, because a finding is worth reusing while a window stands and worth nothing after.

## `internal IReadOnlyList<LCandidate>? LTroveCandidateRead(string session, string word, string language)`

Returns what this draft already found, or null when there is nothing to reuse.
Null is also the answer once the headword or the language has changed under the draft.
A kept answer belongs to the word it was found for, so a changed word must be searched again.

## `internal void LTroveCandidateSave(string session, string word, string language, IReadOnlyList<LCandidate> found)`

Keeps `found` as this draft's answer, replacing whatever it held before.
One draft holds one answer, so editing a headword repeatedly cannot pile entries up.
A search where no source produced a value is not kept.
One run against dead hosts can be tried again.
Rows saying a source failed are kept alongside the readings, so a replay redraws the whole menu it first showed.
A source may hold several rows at one position, one per variety.
Any row with a phonetic makes the set worth keeping.
A draftless caller keeps nothing, because there would be no close to free it at.

## `internal void LTroveClear(string session)`

Drops what one draft held, called where a draft is committed, cancelled or deleted.

## `internal void LTroveClear()`

Drops everything, called when the workspace changes under the engine.
