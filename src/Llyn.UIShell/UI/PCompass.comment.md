# PCompass.cs

## `public partial class PDisplay`

The floating table of contents over the read-only entry view, and the behavior that keeps it true.
It belongs to the Display control rather than the panels around it.
A panel that reads an Entry through Display therefore gets one without asking.
The comparison panel gets one per side.
The editor has none, because a writer already knows where the card they are typing into is.

The rows are built from what the view is actually showing, never from the draft.
A section the entry left empty is collapsed, and a collapsed section is not a place a reader can go.

### `private void PCompassUpdate()`

The rows are built one dispatcher turn after the entry is shown.
A card row points at the container the list generated for it.
Containers do not exist until the layout pass has run.

### `private void PCompassCardAdd(ItemsControl cards, string kind, string unknown)`

A card is named by its own title.
An untitled card is named by what kind of card it is, so the row is never blank.
The card number is carried apart from the label rather than written into it.
A number is not part of a sentence.
Rows sharing a label are numbered once the list is built, so repeated titles read apart.

### `private void PCompassPlace()`

The contents hide themselves when the entry already fits the view.
A reader who can see the whole entry has nothing to navigate.
One row is not a table of contents either.

### `private double? PCompassOffsetRead(FrameworkElement target)`

Every position is measured at the moment it is needed rather than kept in a table.
A dragged seam, a resized window and a rebuilt card list all move the anchors.
None of them announce it.

### `private void PCompassSync()`

The current row is the last one whose anchor has passed the top of the view.
A reader who has scrolled past nothing is still reading the first section.
