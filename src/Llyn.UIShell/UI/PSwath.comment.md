# PSwath.cs

## `public sealed class PSwath : FrameworkElement`

The band of text a reader drags across in the reading view, highlighted so it can be copied.
It runs from wherever the press landed to wherever the pointer is, across everything between.
A text block selects nothing on its own, and one block's selection could never reach the next.
So the band is kept here as two positions in a list of page items, and drawn over the page.
A picture, video, glyph shape or tone contour is an item too, taken whole or not at all.
Copying today takes only the text, and a richer export will read the same band later.
It lies over the page inside the scroll viewer, so its highlight scrolls and clips with the text.
It never takes a hit, so every click still lands on the text and buttons under it.

### `internal void PSwathAttach(ScrollViewer viewer)`

The viewer's tunnelling mouse events are listened to, so a press anywhere on the page is seen first.
A press on a button, link, star row, slider or scroll bar is left to that control.

### `internal void PSwathClear()`

A press, a new entry and an emptied view each drop the band.
The block list is dropped with it, since the page it described is about to change.

### `private void PSwathMoveHandle(object sender, MouseEventArgs e)`

Nothing starts until the pointer has moved a drag's distance.
A plain click therefore still opens a word on a sentence and reaches every control.
The blocks are gathered at that moment, in visual order, which is reading order.
The mouse is then captured by the viewer, so the band follows the pointer past the window's edge.
Focus is taken so the copy key reaches this element.

### `private static bool PSwathControlCheck(DependencyObject? node)`

Walks up from what was hit, through content elements and visuals alike, looking for a control.
A link is a content element, so the logical parent is followed until a visual is reached.

### `private void PSwathScan(DependencyObject node)`

Every visible text block, picture, video, glyph shape and tone contour under the viewer is an item.
Buttons are entered, since chip text is part of the page even though a press on it stays a click.
A video is one item, so the controls inside it are not walked.
A collapsed section is skipped whole.

### `private PSwathSeam? PSwathFind(Point point)`

The item nearest the point wins, vertical distance counting far more than horizontal.
An item that is not text carries no position and is simply in the band or out of it.
A point above or left of a block maps to its start, below or right of it to its end.
A point inside it asks the block for the position under the pointer.
A block asked while its layout is stale refuses, and its start stands in.

### `private static string PSwathSeparatorRead(Rect previous, Rect bound)`

Blocks on one row read as one line, touching blocks with nothing between, spaced ones with a space.
A block on a lower row starts a new line.

### `protected override void OnRender(DrawingContext context)`

The band is the system highlight colour, thinned so the text stays readable through it.
Each block in range contributes the rectangles of its lines between the two positions.
Any other item in range is covered whole.

### `private static IEnumerable<Rect> PSwathBandScan(TextPointer from, TextPointer to)`

A position's rectangle is a zero-width edge, so each is joined with the next position's back edge.
Edges on one line merge into one rectangle, and a new line starts a new one.
