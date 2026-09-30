# CImageDraft.cs

## `public sealed record CImageDraft(`

One image of a card or a scenario, as its image row shows it.

**Parameters**

- `CImageDraftId`: the stored image, zero for a fresh one.
- `CImageDraftLocation`: where the image lives.
- `CImageDraftEmpty`: whether no location was ever recorded, so a reading card folds the image away.
- `CImageDraftAddress`: where the preview is read from, or null when the location reaches nothing.
  A local file counts only while it exists, which the engine checks.

## `public CStateWording CImageDraftWording`

Where the image lives, worded for its location field with the location hint.
