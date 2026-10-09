# PSwath.cs
Hash: `584dc17c6a493ecd`

## `public sealed class PSwath : FrameworkElement`

The band of text a reader drags across in the reading view, highlighted so it can be copied.
It runs from wherever the press landed to wherever the pointer is, across everything between.
A text block selects nothing on its own, and one block's selection could never reach the next.
So the band is kept here as two positions in `QSwathText`'s list of page items, and drawn over the page.
A picture, video, glyph shape or tone contour is an item too, taken whole or not at all.
Copying today takes only the text, and a richer export will read the same band later.
It lies over the page inside the scroll viewer, so its highlight scrolls and clips with the text.
It never takes a hit, so every click still lands on the text and buttons under it.
The drag that draws the band covers the press, the move that opens it, the release and the cursor.
A press on a button, link, text box, star row, slider or scroll bar is left to that control.
A drag past the viewer's top or bottom scrolls it a step at a time, so the band grows off-screen.

### `internal const double PSwathRowSlack = 4;`

How far two edges may sit apart vertically and still count as one row.
It is internal so `QSwathText` joins copied rows by the same measure the band is drawn by.

### `public PSwath()`

It takes focus though it takes no hit, so Copy and Select All reach it from the keyboard.
Copy is offered only while a band is drawn.

### `internal void PSwathAttach(ScrollViewer viewer)`

The viewer's tunnelling mouse events are listened to, so a press anywhere on the page is seen first.

### `internal void PSwathClear()`

A press, a new entry and an emptied view each drop the band.
The item list in `QSwathText` is dropped with it, since the page it described is about to change.

### `protected override void OnRender(DrawingContext context)`

The band is the system highlight colour, thinned so the text stays readable through it.
Each block in range contributes the rectangles of its lines between the two positions.
Any other item in range is covered whole.

### `private static IEnumerable<Rect> PSwathBandScan(TextPointer from, TextPointer to)`

A position's rectangle is a zero-width edge, so each is joined with the next position's back edge.
Edges on one line merge into one rectangle, and a new line starts a new one.

### `private void PSwathMoveRefine(object sender, MouseEventArgs e)`

Nothing starts until the pointer has moved a drag's distance.
A plain click therefore still opens a word on a sentence and reaches every control.
The blocks are gathered at that moment, in visual order, which is reading order.
The mouse is then captured by the viewer, so the band follows the pointer past the window's edge.
Focus is taken so the copy key reaches this element.

### `private static bool PSwathControlCheck(DependencyObject? node)`

Walks up from what was hit, through content elements and visuals alike, looking for a control.
A link is a content element, so the logical parent is followed until a visual is reached.

### `private static bool PSwathTextCheck(DependencyObject? node)`

Walks up the same way, and answers true only for text under no control.
So the beam cursor shows over prose alone.
