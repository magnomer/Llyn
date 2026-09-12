# LRelationDraft.cs

## `public sealed record LRelationDraft(`

One lexical relation a sense is holding before it is stored.

A relation points outward, at an entry or at a sense that may stand in another entry.
The target therefore travels as a name rather than as the row itself.
Which name it is depends on where the draft came from.
A draft the markup reader built carries the document key the file wrote.
A draft the workspace loader built carries the stored id of the row.
The layer that saves the draft turns the first into the second.

A collocation carries no relation, and a sense carries no synonym link.
The two links differ in what they may point at and in which table stores them.

**Parameters**

- `LRelationDraftType` — The kind of relation the user named, such as a synonym or an antonym.
- `LRelationDraftLabel` — What the relation displays, or null when it displays nothing.
- `LRelationDraftLabels` — The localized form of that label, or null when there is none.
- `LRelationDraftEntry` — Anchor of the entry the relation points at, empty when it points at a sense.
- `LRelationDraftMeaning` — Anchor of the sense the relation points at, empty when it points at an entry.
- `LRelationDraftId` — Id of the stored relation row, negative before the row exists.

## Inline notes

### `public bool LRelationDraftEmpty`

A relation is empty when both anchors are empty.
The reader refuses such a relation, and the writer has nothing to write for one.
