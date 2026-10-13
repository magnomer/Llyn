# Transcription

This folder holds the transcription rows of the held entry with their schemes.
It also holds the reading-candidate popup that fills a row.

## `CTranscription`

The transcription rows of the entry an editor holds, split from `CTimbre` because the rows are edited.
`CTranscriptionRead` answers every row outside the glyph scheme, each with its scheme dropdown.
An empty desk answers the hidden block with no rows, so nothing can be added.
A filling desk writes nothing in any gate.
`CTranscriptionSet` is deferred by the engine, and `CTranscriptionSchemeSet` applies at once.
`CTranscriptionAdd` maps null to the engine's id 0, and the engine adds the row under the first free scheme.

## `CTranscriptionRow`, `CTranscriptionSheet`

A row carries its draft and every scheme of the pack, each marked when another row holds it.
The sheet is shown while the pack declares any scheme, and free while some scheme is unused.

## `CScheme`

`CSchemeKeyRead` is the one owner of the scheme's localization key.
The scheme, the transcription draft and the glyph heading share it.
A blank name keys nothing a catalog holds, so a driver prints it as it stands.

## `CNotation`

The notation popup's session state, which the errand holds and fills from its reading search.
It keeps the source rows in the pack's order, one row per source.
Only the errand reaches it, so no driver holds the list.
`LNotationStart` records whether the search was for a transcription scheme, which picks the empty notice.
Once ended, a schemed search reads the transcription notice and a plain one the reading notice.

`LNotationPlace` and `LNotationCandidateAdd` follow the same row rules as the clip popup.
Rows stand in the pack's order, and an empty answer never demotes a found row.
`LNotationReadingRead` takes the mark and the text from the foray, so the respelling rule keeps its one owner below.
Only a reading that names a variety shows a flag, and only when the language shows flags.

## `CNotationItem`, `CNotationReading`, `CNotationRoll`, `CCandidate`, `CLookupStep`

These mirror the clip records for readings.
A reading's variety name is the raw variety that taking writes, blank when the source names none.
`CCandidatePhonetic` is null when the source gave no reading.
`CCandidateReached` picks the notice for a missing reading.
`CLookupStep` carries no kind, since a found reading or the end flag already tells the step apart.

## Stamp

Commit: `c2f335c331fe`
- `CCandidate.cs` `0c6c2a48a0d7ad06`
- `CLookupStep.cs` `724a23dbea7247a3`
- `CNotation.cs` `a1ff26e37c7d40c8`
- `CNotationItem.cs` `4238658664f1d524`
- `CNotationReading.cs` `b8295d70dfb42aa0`
- `CNotationRoll.cs` `07195c961635b8f1`
- `CScheme.cs` `6ffa9b01eb9c03f6`
- `CTranscription.cs` `902b69e36ea27b69`
- `CTranscriptionRow.cs` `0011cc7ba8719efc`
- `CTranscriptionSheet.cs` `4117d0055e9f99a9`
