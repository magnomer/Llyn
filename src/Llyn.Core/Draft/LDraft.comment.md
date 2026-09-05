# LDraft.cs

## `public sealed record LDraft(`

One tentative record, held in the workspace folder before it reaches the database.
It is plain data and knows nothing of files or the database.
A draft carries its own throwaway id, discarded once the record becomes real.

**Parameters**

- `LDraftId` — Throwaway id of this draft, valid only while the draft is unsaved.
- `LDraftOrigin` — Panel that started the draft.
  The input panel and the library can each hold one at the same time.
- `LDraftEntry` — Id of the real entry being edited, or empty for a new one.
- `LDraftContent` — The form contents as one immutable value.
- `LDraftMoment` — Instant the draft was last written.
