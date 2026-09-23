# LEtymologyDraft.cs

## `public sealed record LEtymologyDraft(`

The etymology of one entry while it is being edited.
It carries both shapes together, the prose with its spans and the ordered link ids.
The editor shows one field for both, and the clerk writes whichever shape the draft ended in.
The draft never decides which shape wins, since a draft may pass through an empty state.

**Parameters**

- `LEtymologyDraftText` — The prose being typed, empty while the entry keeps links instead.
- `LEtymologyDraftMentions` — The spans of that prose which name an Entry, held in offset order.
- `LEtymologyDraftEtymons` — The ids of the source entries linked directly, in the order shown.
- `LEtymologyDraftId` — The id of the stored narrative row, zero while none is written.

## `public bool LEtymologyDraftNarrated`

Whether the draft carries prose.
The text alone answers this, so a narrative with no span still counts as one.

## `public bool LEtymologyDraftLinked`

Whether the draft carries at least one direct link.

## `public bool LEtymologyDraftEmpty`

Whether the draft says nothing at all about the entry's origin.
Such a draft clears both stored shapes when it is saved.

## `public static LEtymologyDraft LEtymologyDraftCreate(LEtymology? etymology, IReadOnlyList<long> etymons)`

Builds the draft from what the store holds for one entry.
A missing narrative gives an empty text, and missing links give an empty list.

## `public LEtymology LEtymologyDraftResolve(long entryId)`

The narrative shape the draft would be stored as, under the entry given.
The link ids are not part of it, since the two shapes are written apart.

## `public LMentionDraft? LEtymologyDraftFind(LMentionDraft span)`

The Mention of the narrative the span lies inside, or none.
A span with no length counts as inside when it stands anywhere on the Mention, its ends included.

## `public IReadOnlyList<LMentionPiece> LEtymologyDraftDivide()`

Parts the prose into the pieces a view draws, each either plain text or one span.
`LMentionSpan` does the cutting, so the reading side and the edit side part the text alike.
