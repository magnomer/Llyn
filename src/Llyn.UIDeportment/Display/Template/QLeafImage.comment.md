# QLeafImage.cs

## `internal sealed record QLeafImage(QImageItem QLeafImageRow, bool QLeafImageEmpty)`

One picture line of a reading card, as the picture template receives it.
It carries the loading row and Conduct's empty verdict, so the template never holds a Conduct record.

**Parameters**

- `QLeafImageRow`: the loading picture row the line fill paints.
- `QLeafImageEmpty`: whether Conduct calls the row empty, which collapses the line.

## `internal static IReadOnlyList<QLeafImage> QLeafImageCreate(IReadOnlyList<CImageDraft> drafts)`

The picture lines of a card, one per ready picture row, in the order Conduct gave them.
Each loading row is built here as the vignette builds its own, and loads only once in view.
