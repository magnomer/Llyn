# CVideoDraft.cs

## `public sealed record CVideoDraft(long CVideoDraftId, CStateValue CVideoDraftLocation, CStateValue CVideoDraftSpan, bool CVideoDraftEmpty)`

One video of a card or a scenario, as its video row shows it.

**Parameters**

- `CVideoDraftId`: the stored video, zero for a fresh one.
- `CVideoDraftLocation`: where the video lives.
- `CVideoDraftSpan`: the timestamp the video plays from.
- `CVideoDraftEmpty`: whether no location was ever recorded, so a reading card folds the video away.
