# LReflexDraft.cs

## `public sealed record LReflexDraft(`

One reflex of an entry as one value the input form carries.
An entry holds an ordered list of these, one per row the form shows.
A row with a language but no text is a row still being filled.
It is kept while the draft lives.
On commit a row is stored whenever any of its four texts is filled.

**Parameters**

- `LReflexDraftLanguage` — The borrowing language, from the pack rule or typed.
- `LReflexDraftKind` — The kind before the reading, such as Go-on, or empty.
- `LReflexDraftText` — The reading as typed or fetched.
- `LReflexDraftMain` — True when the reading is marked as the one in common use.
- `LReflexDraftId` — Id of the stored reflex row, empty when none stands yet.
- `LReflexDraftNote` — The note after the reading, such as a pinyin or a 훈, or empty.
- `LReflexDraftRespelling` — The reading recast through its own language's respelling groups, or empty.
  It is stored beside the reading so the respelling switch only picks which of the two is shown.
  A phonemic language has its brackets turned into slashes here, since the brackets are part of the reading.

## `public bool LReflexDraftEmpty`

True when the row carries no language, kind, text or note, so nothing of it is worth storing.
