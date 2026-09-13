# PSeam.cs

## `public sealed class PSeam : Thumb`

The divider between two side-by-side panels, dragged to give one panel more room.
It draws the same one-pixel line the gutter always showed, inside a wider strip the pointer can catch.
The strip is twelve pixels wide and centred on the line, so the line itself stays where it was.

### `internal PLayout? PSeamLayout { get; set; }`

The keeper the seam reports its drags to, set by the window when the tab is registered.
A seam with no keeper still resizes its own tab, so a tab standing alone keeps working.

### `PSeamLayout?.PLayoutPropagate(host);`

Every step of the drag is reported, so linked tabs follow live rather than at the end.
A step that crossed back over its start is reported too, because the snapshot was just put back.

### `private void PSeamFinishHandle(object sender, DragCompletedEventArgs e)`

The widths are stored once when the pointer is released, not on every step.
A drag that never took a snapshot has nothing to store.

### `private static double PSeamRoomRead(Grid host, int column)`

A panel stops shrinking where its own contents would start being cut off.
The limit is read from what the panel holds at this moment, never from a fixed number.
Each thing standing in the column is measured against no width at all.
What it asks for under that measure is the part of it that cannot be made narrower.
A wrapping paragraph asks for its longest word, a row of buttons asks for all of them.
Text that already trims itself asks for nothing, because trimming is how it was meant to shrink.

### `child.Visibility != Visibility.Visible`

Only what the user can see right now sets the limit.
A collapsed region takes no room, so it may not claim any.
The measure runs at the start of every drag, so the limit follows the panel as its contents change.

### `Grid.GetColumnSpan(child) != 1`

An element laid across several columns belongs to none of them.
The rules across the top of a tab are the case this skips.

### `child.InvalidateMeasure();`

The measure is asked for an answer, not for a layout.
Each element measured is marked stale straight after, and one layout pass then restores the real one.

### `Math.Min(PSeamRoomRead(host, index), _pSeamWidth[index])`

A panel already too narrow for its contents would otherwise be frozen where it stands.
Its limit is its current width instead, so it can still be widened and simply not narrowed.

### `private void PSeamStartHandle(object sender, DragStartedEventArgs e)`

Every width the tab declared is frozen the moment a drag begins.
The whole drag is then measured from that one snapshot rather than from the last mouse step.
A drag that runs into the minimum therefore stops moving instead of drifting away from the pointer.

### `_pSeamOrigin = Mouse.GetPosition(host).X;`

The pointer is read against the tab grid, not against the seam.
The seam travels while it is dragged, so its own coordinates would move under the measurement.

### `int grow = shift > 0 ? seam - 1 : seam;`

The panel next to the seam on the side being opened is the one that grows.

### `int pay = shift > 0 ? last : 0;`

The room comes from the outermost panel on the other side, never from the immediate neighbour.
With three panels this is what lets a middle panel keep its width and merely shift across.
Widening the left panel therefore moves the middle one and narrows the right one.
Widening the middle panel leftward narrows the left one, and rightward narrows the right one.

### `for (int index = 0; index < last; index++)`

Every fixed width is put back to its snapshot before the new one is applied.
Without it a drag that crossed back over its start would leave the earlier side still shrunk.

### `private static void PSeamColumnNormalize(Grid host)`

A tab declares its panels as fixed widths with one flexible column at the end.
The flexible column is left flexible so the tab still fills the window as it is resized.
Every other column is pinned to the width it currently has, which is what makes it adjustable.
A panel pair declared as two flexible halves is pinned the same way on its first drag.

### `if (grow != last)`

The last column is never written, because it is the flexible one.
It takes whatever the fixed columns leave, so growing or shrinking it is done by writing the others.
