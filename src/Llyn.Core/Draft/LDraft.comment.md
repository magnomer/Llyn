# LDraft.cs

## `public sealed record LDraft(`

One tentative record, held in the workspace folder before it reaches the database.
It is plain data and knows nothing of files or the database.
A draft carries its own throwaway id, discarded once the record becomes real.
One draft holds one kind of work, told apart by which content field carries it.
A draft naming no example, no situation and no source holds an entry.
One naming an example holds a sentence and one naming a situation holds a context.
One naming a source holds a citation.
The kind is read from the content rather than from a tag beside it, so the two cannot disagree.

**Parameters**

- `LDraftId` — Throwaway id of this draft, valid only while the draft is unsaved.
- `LDraftOrigin` — Panel that started the draft.
  The input panel and the library can each hold one at the same time.
- `LDraftEntry` — Id of the real record being edited, or empty for a new one.
  It names an entry, an example, a situation or a source, whichever kind the draft holds.
- `LDraftContent` — The entry form contents as one immutable value.
  A draft of another kind carries a blank one here, because its work lives in its own field.
- `LDraftMoment` — Instant the draft was last written.
- `LDraftExample` — The sentence being written, or null when this draft holds no sentence.
- `LDraftSituation` — The context being written, or null when this draft holds no context.
- `LDraftReference` — The source being written, or null when this draft holds no source.
