# TTenureLanguage.cs

## `public sealed class TTenureLanguage`

Language checks and variety lookup use the tenure's held draft.
After cancellation, the tenure returns empty defaults.
Checking an independently empty draft also remains false.

## `Language_ReadsHeldDraftAndAnswersEmptyAfterCancel()`

The held Entry draft drives both language queries.
Cancellation clears their answers instead of retaining the prior draft's language state.
