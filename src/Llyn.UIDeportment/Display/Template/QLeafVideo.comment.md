# QLeafVideo.cs

## `internal sealed record QLeafVideo(QVideoItem QLeafVideoRow, bool QLeafVideoEmpty)`

One film line of a reading card, as the film template receives it.
It carries the film row and Conduct's empty verdict, so the template never holds a Conduct record.

**Parameters**

- `QLeafVideoRow`: the film row the Screen binds.
- `QLeafVideoEmpty`: whether Conduct calls the row empty, which collapses the line.

## `internal static IReadOnlyList<QLeafVideo> QLeafVideoCreate(IReadOnlyList<CVideoDraft> drafts)`

The film lines of a card, one per ready film row, in the order Conduct gave them.
Each film row is built here as the vignette builds its own.
