# PRail.cs

## `public sealed class PRail : Panel`

The command strip across the top of a panel, holding the groups of buttons that act on the record.
The leading group stands at the left and every later group is pushed to the right edge.
A window narrow enough to make those two meet drops the later groups onto lines of their own.
The panel therefore never lets one group be drawn over another, whatever the window is doing.

### `child.Measure(room)`

Every group is measured against unlimited room, so each asks for the width its buttons truly need.
Measuring against the room actually offered would let a cramped group report a width it cannot draw in.

### `_pRailFold = double.IsFinite(available.Width) && line > available.Width;`

The strip folds only when one line of groups would be wider than the room the panel was given.
An unbounded measure never folds, because there is no width to be too narrow.

### `return _pRailFold ? new Size(widest, stack) : new Size(line, tallest);`

Folded, the strip asks for the height of every group and the width of the widest one.
Unfolded, it asks for one line of groups and the height of the tallest.
The header row is sized from that answer, so folding pushes the panel below it down.

### `if (child.Visibility == Visibility.Collapsed)`

A collapsed group takes no line of its own and adds no gap.
It is still measured, because a group hidden now may be shown before the next layout pass.

### `edge -= size.Width;`

Later groups are laid from the right edge inward, in the order they were declared.
The second group therefore sits at the far right and a third would sit just left of it.
