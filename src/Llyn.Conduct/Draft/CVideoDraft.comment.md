# CVideoDraft.cs
Hash: `c294d414325388ac`

## `public sealed record CVideoDraft(long CVideoDraftId, CStateValue CVideoDraftLocation, CStateValue CVideoDraftSpan, bool CVideoDraftEmpty, CScreen? CVideoDraftScreen, TimeSpan CVideoDraftFrom, TimeSpan? CVideoDraftUntil)`

One video of a card or a scenario, as its video row shows it.

**Parameters**

- `CVideoDraftId`: the stored video, zero for a fresh one.
- `CVideoDraftLocation`: where the video lives.
- `CVideoDraftSpan`: the timestamp the video plays from.
- `CVideoDraftEmpty`: whether no location was ever recorded, so a reading card folds the video away.
- `CVideoDraftScreen`: what the screen plays, or null when the location reaches nothing.
  A local file counts only while it exists, which the engine checks.
- `CVideoDraftFrom`: the moment the screen plays from, the film's start when the span names none.
- `CVideoDraftUntil`: the moment the screen stops, or null to play to the end.
