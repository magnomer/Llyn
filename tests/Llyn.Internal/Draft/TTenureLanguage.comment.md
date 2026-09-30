# TTenureLanguage.cs

## `public sealed class TTenureLanguage`

Language checks use the tenure's held draft.
After cancellation, the tenure returns empty defaults.
Checking an independently empty draft also remains false.
The scheme read marks a scheme another row holds, and an ended tenure reads no schemes.

## `Language_ReadsHeldDraftAndAnswersEmptyAfterCancel()`

The held Entry draft drives both language queries.
Cancellation clears their answers instead of retaining the prior draft's language state.
