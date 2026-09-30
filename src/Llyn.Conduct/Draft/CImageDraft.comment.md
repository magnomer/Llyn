# CImageDraft.cs

## `public sealed record CImageDraft(long CImageDraftId, CStateValue CImageDraftLocation, bool CImageDraftEmpty)`

One image of a card or a scenario, as its image row shows it.

**Parameters**

- `CImageDraftId`: the stored image, zero for a fresh one.
- `CImageDraftLocation`: where the image lives.
- `CImageDraftEmpty`: whether no location was ever recorded, so a reading card folds the image away.

## `public CStateWording CImageDraftWording`

Where the image lives, worded for its location field with the location hint.
