# CSituationDraft.cs

## `public sealed record CSituationDraft(`

One situation, as a card's context chips or the repertoire's scenario and vignette show it.
A card's link carries no media, so its picture and video lists stay empty.

**Parameters**

- `CSituationDraftId`: the stored situation, zero for a fresh one.
- `CSituationDraftTitle`: the situation's title.
- `CSituationDraftKind`: the kind of situation.
- `CSituationDraftDescription`: the situation's description.
- `CSituationDraftImage`: the pictures in their order.
- `CSituationDraftVideo`: the videos in their order.
