# PSwathDrag.cs

## `public sealed partial class PSwath`

The drag that draws the band: the press, the move that opens it, the release and the cursor.
A press on a button, link, star row, slider or scroll bar is left to that control.
A drag past the viewer's top or bottom scrolls it a step at a time, so the band grows off-screen.

### `private void PSwathMoveHandle(object sender, MouseEventArgs e)`

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
