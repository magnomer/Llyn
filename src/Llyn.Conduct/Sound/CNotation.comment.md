# CNotation.cs

## `internal sealed class CNotation`

The notation popup's session state, which the errand holds and fills from its reading search.
It keeps the source rows in the pack's order, one row per source, and whether the search still runs.
Only the errand reaches it, so no driver holds the list.

## `private bool _cNotationSchemed;`

Whether the search was started for a transcription scheme, which picks the empty notice.

## `internal void LNotationStart(bool schemed)`

Empties the rows and marks the search running.
`LNotationFinish` marks it ended.

## `internal CNotationRoll LNotationRead()`

Answers the state as the popup paints it.
The notice says searching while the search runs.
Once it ended, a schemed search reads the transcription notice and a plain one the reading notice.

## `internal int LNotationPlace(string source, int order)`

Finds the row one source owns, creating it at its declared place when it has none yet.
Rows stand where the language pack put the source, not where the network put it.
A new row opens with the searching notice.

## `internal void LNotationCandidateAdd(CCandidate candidate, LForay foray)`

Resolves the row of one source from one thing it said.
A candidate with a reading is appended and makes the row takeable.
No reading reads as missing when the source answered, and broken when it never did.
An empty answer after a reading has landed changes nothing, so a found row never falls back to a notice.

## `private static CNotationReading LNotationReadingRead(CCandidate candidate, LForay foray)`

Gives one reading its shown text, its mark and its variety keys, all under the search it came from.
The foray answers the mark and the text, so the respelling rule keeps its one owner below.
Only a reading that names a variety shows a flag, and only when the language shows flags.
