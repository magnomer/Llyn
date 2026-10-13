# Frame

The window's furniture around the panels.
It holds the status bar, the shared panel widths, the command strip and the draggable divider.
It stands apart from the panels because each piece serves every tab alike.

## `QCourier`

Drives the Joplin control at the left end of the status bar.
That is the connect button, the connected badge, the send button and the receipt line.
The courier owns the connect, the push and whether a token is stored.
This driver hears the two buttons and paints what the courier hands back.
It creates its own courier from the atelier, so both buttons share one busy mark.
The state is observed through `QObserver`, so a change repaints on the surface's thread.
A settings bulletin raises the state on the engine's thread, which is why that matters.
It keeps the window's envoy, and both gates show a failure through it.
The first read paints the bar idle and unconnected.
The connect button and the badge swap on the attached state, so exactly one shows.
Both buttons take the allowed state as it stands, since the courier decides when they rest.
The slash shows only beside a line, so an idle bar ends at the send button.
The send and connect gates report their own failure.
`QEstablishmentAttach` introduces it before the atelier opens, so the first open repaints the connection.

## `QEstablishment`

Drives the status bar along the foot of the window.
The left end holds the Joplin control, which `QCourier` drives.
After it the bar says whether the window holds unsaved work, and the right says how big the workspace is.
It holds nothing of its own and prints what the engine answers.
The bar is the veneer's `PEstablishment` page, which the window places.
Each named part is pulled from the bar by contract ID on every read.
The Joplin driver paints its own part from its own courier, so the status read never carries Joplin state.
It listens to `CWorkspaceEstablishmentChanged`, raised on every open and every bulletin.
So a change made anywhere reaches the bar without any panel telling it.
The observer stays inside a detach delegate, so no field holds an engine announcement.
The Joplin driver is introduced last, still before the atelier opens, so its courier hears the first open.
Closing stops listening before the engine goes.
A second close detaches an observer already gone, which the engine ignores.
The entry and size lines go through wording keys the atelier chose.
The unsaved line hides when nothing is unsaved, so a calm window shows nothing on the left.
A refused read never arrives, so the last printed numbers stand.

## `QLayout`

The keeper of panel widths across every tab that has a seam.
Each tab's root grid is registered once, and its seams are handed this keeper.
A drag in one tab is carried to every other tab while the panels are linked.
Widths are stored per tab either way, so unlinking never loses what a tab had.
They go through the posture, so the engine never learns how wide a pane is.
A cleared posture resets the grids, and a flipped linked switch syncs the tabs.
The switch in the settings panel is a setting on the layout and not an object.
The posture keeps it, so the next run opens with it.
The settings panel wires the click while it is introduced, since the window builds this keeper first.

### Registration and restore

Registering records the widths the markup declared for the fixed columns, so a reset can find them later.
Every seam in the grid has its drag and release notices subscribed.
Only the fixed columns are written, because the last column stays flexible and fills the window.
Stored widths apply before the first tab is shown.
The panel roster re-applies them on each workspace open.
A tab with no stored record keeps its markup width.
While linked, every tab is then set to one width, so the link holds before any drag.
That width is the first registered tab's, and the middle width is the first three-panel tab's.

### Linking

Linking copies every fixed column of the source grid onto every other registered grid.
The source is the dragged grid, else the most recently dragged, else the first registered.
A tab with fewer columns takes only the columns it has.
The most recent drag is remembered even while unlinked.
Ticking the switch sets every tab to the most recently dragged tab's widths at once.
Unticking moves nothing, because each tab holds its own widths and just stops following.
The sync runs either way, since an unlinked sync only stores widths already stored.

### Saving and reset

Widths are stored once a drag has ended and never on every step.
Linked panels store every tab, because every tab was just moved.
Unlinked panels store only the dragged tab.
A reset puts every grid back at its markup widths and forgets the most recent drag.
It answers a cleared posture and every workspace open, so a workspace without stored widths opens at markup widths.

### Reading a column

A column pinned by a drag is read from its declared width.
The layout pass may not have run yet, so what it measures could be stale.
A column still flexible is read from what it measures now.

## `QRail`

The command strip across the top of a panel, holding groups of buttons that act on the record.
The leading group stands at the left and every later group is pushed to the right edge.
When those two would meet, the later groups drop onto lines of their own.
So the panel never draws one group over another.

### Measure

Measure decides whether the strip folds and keeps that answer for arrange.
Every group is measured against unlimited room, so each asks for the width its buttons truly need.
Measuring against the offered room would let a cramped group report a width it cannot draw in.
The strip folds only when one line of groups is wider than the room given.
An unbounded measure never folds, because no width can be too narrow.
Folded, it asks for every group's height and the widest group's width.
Unfolded, it asks for one line of groups and the tallest height.
The header row is sized from that answer, so folding pushes the panel below it down.
A collapsed group takes no line and adds no gap.
It is still measured, because a hidden group may be shown before the next pass.

### Arrange

Arrange follows the fold the last measure decided and never decides it again.
It takes the whole room, so the right edge is the panel's own edge.
Later groups are laid from the right edge inward in declared order.
So the second group sits at the far right and a third just left of it.

## `QSeam`

The divider between two side-by-side panels, dragged to give one panel more room.
It draws the one-pixel line the gutter always showed, inside a twelve-pixel strip the pointer can catch.
The strip is centred, so the line stays where it was.
The seam holds no keeper, so a tab never registered with `QLayout` still resizes alone.
The duplex and settings tabs are such tabs.
Every drag step is reported with the tab grid, so linked tabs follow live.
A step that crossed back over its start is reported too, because the snapshot was just put back.
The release is reported once, and `QLayout` stores the widths then.
A drag that never took a snapshot has nothing to store.

### Snapshot

Every width the tab declared is frozen when a drag begins.
The whole drag is measured from that snapshot and not from the last mouse step.
So a drag that runs into the minimum stops moving instead of drifting from the pointer.
Every fixed width is put back to its snapshot before the new one is applied.
Without it a drag that crossed back over its start would leave the earlier side still shrunk.
The pointer is read against the tab grid, because the seam travels and its own coordinates would move.

### Pinning columns

A tab declares fixed widths with one flexible column at the end.
Every other column is pinned to its current width, which makes it adjustable.
A pair declared as two flexible halves is pinned the same way on its first drag.
The last column is never written.
It takes what the fixed columns leave.

### Who pays

The panel next to the seam on the side being opened grows.
The room comes from the outermost panel on the other side and not from the neighbour.
With three panels that lets a middle panel keep its width and shift across.
Widening the left panel moves the middle one and narrows the right one.
Widening the middle one leftward narrows the left, and rightward narrows the right.

### Minimum width

A panel stops shrinking where its own contents would be cut off.
The limit is read from the contents at this moment and not from a fixed number.
Each visible item in the column is measured against no width at all.
What it asks for is the part that cannot be made narrower.
A wrapping paragraph asks for its longest word and a row of buttons asks for all of them.
Text that trims itself asks for nothing.
Only what the user can see sets the limit, so a collapsed region claims nothing.
An element spanning several columns belongs to none, such as the rules across the top of a tab.
The measure runs at the start of every drag, so the limit follows the contents.
Each measured element is marked stale straight after, and one layout pass restores the real one.
A panel already too narrow for its contents takes its current width as the limit.
So it can still be widened and simply not narrowed.

## Stamp

Commit: `c2f335c331fe`
- `QCourier.cs` `a500f6f9e4cfed13`
- `QEstablishment.cs` `9ccaa34c07e40d56`
- `QLayout.cs` `cc7862a7034f266b`
- `QRail.cs` `a99800e66fa9103c`
- `QSeam.cs` `d891f1e383c6901a`
