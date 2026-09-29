# PLayout.cs

## `public sealed class PLayout`

The keeper of panel widths across every tab that has a seam.
Each tab's root grid is registered once, and the seams inside it are handed this keeper.
A drag in one tab is then carried to every other tab while the panels are linked.
The widths are stored per tab either way, so unlinking never loses what a tab had.
They are read from and written through the posture, so the engine never learns how wide a pane is.
It listens for the posture being cleared and resets the grids, so a width reset is one call.
It listens for the linked switch flipping and syncs the tabs, so ticking the switch is one call.

## `internal void PLayoutAttach(Grid host, string tab)`

Registers a tab's root grid under the name its widths are stored by.
The widths its markup declares for the fixed columns are recorded alongside, so a reset can find them later.
Every seam standing in that grid is pointed back here, so a drag can report itself.

## `internal void PLayoutRefine()`

Applies the stored widths to every registered grid before the first tab is shown.
The window subscribes it to `CWorkspaceOpened`, so a workspace change applies that workspace's widths.
A tab with no stored record keeps the width its markup declared.
Only the fixed columns are written, because the last column stays flexible and fills the window.
While the panels are linked, every tab is then set to one width, so the link holds before any drag.
The width taken is the first registered tab's, stored or declared, and the middle width the first three-panel tab's.

## `internal void PLayoutLinkRefine(Grid? dragged)`

Copies every fixed column of the source grid onto every other registered grid.
A drag hands its grid, and linking the panels hands none, so the source falls back as `PLayoutSourceRead` says.
A tab with fewer columns takes only the columns it has, so a middle width reaches only three-panel tabs.
The dragged grid is remembered as the most recent, so a later link knows which tab to copy from.
Nothing is copied while the panels are unlinked, but the source is still remembered.

## `internal void PLayoutSave(Grid? dragged)`

Stores the widths once a drag has ended, never on every step of it.
Linking the panels mid-session stores them too, after the link copied them, from the same source.
Linked panels store every tab, because every tab was just moved.
Unlinked panels store only the dragged tab.

## `private void PLayoutLinkedRefine()`

Answers the posture's linked switch flipping by copying the source grid onto the others and storing the result.
The sync runs either way, since an unlinked sync only stores the source grid's widths, which are already stored.

## `internal void PLayoutResetRefine()`

Puts every registered grid back at the widths its markup declared.
It answers a cleared posture and every workspace open, so a workspace without stored widths opens at the markup widths.
Only the fixed columns are written, because the last column was never pinned.
The most recent drag is forgotten, so a later link copies from the first registered tab again.

## `private Grid? PLayoutSourceRead(Grid? dragged)`

The grid the others follow: the dragged one, else the most recently dragged, else the first registered.
Linking the panels mid-session sets every other tab to that grid.
Unlinking calls nothing, because tabs simply stop following from then on.

## `private static double? PLayoutColumnRead(ColumnDefinition column)`

A column pinned by a drag is read from its declared width.
The layout pass may not have run yet, so what it measures could be stale.
A column still flexible is read from what it currently measures, and nothing before it was ever laid out.
