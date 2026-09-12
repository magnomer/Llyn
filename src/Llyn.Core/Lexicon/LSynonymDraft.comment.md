# LSynonymDraft.cs

## `public sealed record LSynonymDraft(`

One synonym interlink a collocation is holding before it is stored.

A collocation synonym is a link to a row, never free text.
It points at an entry or at a sense, and at exactly one of the two.
The target travels as a document key from the reader and as a stored id from the loader.

`LCardDraftSynonym` is a different thing wearing the same word.
That field is the free text a card carries, and it points at nothing.
This draft is the stored link, and the two are never merged.

**Parameters**

- `LSynonymDraftEntry` — Anchor of the entry the link points at, empty when it points at a sense.
- `LSynonymDraftMeaning` — Anchor of the sense the link points at, empty when it points at an entry.
- `LSynonymDraftId` — Id of the stored synonym row, negative before the row exists.

## Inline notes

### `public bool LSynonymDraftEmpty`

A link is empty when both anchors are empty.
The reader refuses such a link, and the writer has nothing to write for one.
