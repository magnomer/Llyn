# CClipItem.cs

## `public sealed record CClipItem(`

One source row of the clip popup, as the errand keeps it.
A row appears the moment its source starts searching, so every declared source shows before any answers.
It is replaced in place as its source answers, so a row never jumps.

**Parameters**

- `CClipItemSource`: the name of the source the row stands for.
- `CClipItemOrder`: the source's place in the pack, which keeps the row where the pack put it.
- `CClipItemReading`: the recordings the source gave so far, in the order they arrived.
- `CClipItemNotice`: the notice key a row without a recording shows, empty once one landed.
- `CClipItemReady`: whether the row carries a recording the user can take now.
