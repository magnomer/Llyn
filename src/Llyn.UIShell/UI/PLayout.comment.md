# PLayout.cs

## `public sealed class PLayout`

The keeper of panel widths across every tab that has a seam.
Each tab's root grid is registered once, and the seams inside it are handed this keeper.
A drag in one tab is then carried to every other tab while the panels are linked.
The widths are stored per tab either way, so unlinking never loses what a tab had.

## `internal void PLayoutAttach(Grid host, string tab)`

Registers a tab's root grid under the name its widths are stored by.
The widths its markup declares for the fixed columns are recorded alongside, so a reset can find them later.
Every seam standing in that grid is pointed back here, so a drag can report itself.

## `internal void PLayoutRestore()`

Applies the stored widths to every registered grid before the first tab is shown.
A tab with no stored record keeps the width its markup declared.
Only the fixed columns are written, because the last column stays flexible and fills the window.
While the panels are linked, every tab is then set to one width, so the link holds before any drag.
The width taken is the first registered tab's, stored or declared, and the middle width the first three-panel tab's.

## `internal void PLayoutPropagate(Grid source)`

Copies every fixed column of the dragged grid onto every other registered grid.
A tab with fewer columns takes only the columns it has, so a middle width reaches only three-panel tabs.
The dragged grid is remembered as the most recent, so a later link knows which tab to copy from.
Nothing is copied while the panels are unlinked, but the source is still remembered.

## `internal void PLayoutSave(Grid source)`

Stores the widths once a drag has ended, never on every step of it.
Linked panels store every tab, because every tab was just moved.
Unlinked panels store only the dragged tab.

## `internal void PLayoutSync()`

Runs when the user links the panels mid-session.
Every other tab is set to the most recently dragged tab.
Before any drag, the first registered tab is the one copied from.
Unlinking calls nothing, because tabs simply stop following from then on.

## `internal void PLayoutReset()`

Puts every registered grid back at the widths its markup declared.
Only the fixed columns are written, because the last column was never pinned.
The most recent drag is forgotten, so a later link copies from the first registered tab again.

## `private static double? PLayoutColumnRead(ColumnDefinition column)`

A column pinned by a drag is read from its declared width.
The layout pass may not have run yet, so what it measures could be stale.
A column still flexible is read from what it currently measures, and nothing before it was ever laid out.
