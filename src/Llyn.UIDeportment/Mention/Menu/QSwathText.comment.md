# QSwathText.cs
Hash: `123171a15a75d495`

## `public sealed class QSwathText`

The page items a band runs across, and the text the band covers.
It stands apart from `PSwath`, which keeps the drag and the drawing.
Every bound is measured against its owner, so the items and the highlight share one frame.

### `public QSwathText(PSwath owner)`

Takes the swath every item bound is measured against.

### `public IReadOnlyList<FrameworkElement> QSwathTextItem`

The gathered items in visual order, which is reading order.
The swath reads it to draw the band and to select everything.

### `public void QSwathTextClear()`

Drops the gathered items, since the page they described is about to change.

### `public void QSwathTextScan(DependencyObject node)`

Every visible text block, picture, video, glyph shape and tone contour under the viewer is an item.
Buttons are entered, since chip text is part of the page even though a press on it stays a click.
A video is one item, so the controls inside it are not walked.
A video is known as a surface that `QScreen` drives, since Deportment never names the Veneer's screen type.
A collapsed section is skipped whole.

### `public Rect QSwathTextPlace(FrameworkElement item)`

The item's bounds in the owner's frame, where the band is drawn.

### `public PSwathSeam? QSwathTextFind(Point point)`

The item nearest the point wins, vertical distance counting far more than horizontal.
An item that is not text carries no position and is simply in the band or out of it.
A point above or left of a block maps to its start, below or right of it to its end.
A point inside it asks the block for the position under the pointer.
A block asked while its layout is stale refuses, and its start stands in.

### `public string QSwathTextRead(PSwathSeam? start, PSwathSeam? finish)`

The text between the two ends, empty while no band is drawn.
Only text blocks contribute, so pictures and videos in the band add nothing yet.

### `private static string QSwathTextFormat(Rect previous, Rect bound)`

Blocks on one row read as one line, touching blocks with nothing between, spaced ones with a space.
A block on a lower row starts a new line.
