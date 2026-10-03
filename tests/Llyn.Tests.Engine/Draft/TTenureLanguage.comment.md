# TTenureLanguage.cs
Hash: `e4c5c6b4324fd3df`

## `public sealed class TTenureLanguage`

Language checks use the tenure's held draft.
After cancellation, the tenure returns empty defaults.
Checking an independently empty draft also remains false.
The scheme read marks a scheme another row holds, and an ended tenure reads no schemes.

## `public void Language_ReadsHeldDraftAndAnswersEmptyAfterCancel()`

The held Entry draft drives both language queries.
Cancellation clears their answers instead of retaining the prior draft's language state.

## `public void TenureTranscriptionRead_TwoRows_MarksTheSchemeTheOtherRowHolds()`

Each transcription row lists the schemes, and one the other row already holds is marked taken.
A second row of the same scheme would be refused, so the picker must show it as unavailable.
After the tenure is cancelled the sheet reads null.
