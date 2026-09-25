# LCompass.cs

## `public sealed class LCompass`

The floating table of contents over the read-only entry view, and the behavior that keeps it true.
It belongs to the Display control rather than the panels around it.
A panel that reads an Entry through Display therefore gets one without asking.
The comparison panel gets one per side.
The editor has none, because a writer already knows where the card they are typing into is.

The rows are built from what the view is actually showing, never from the draft.
A section the entry left empty is collapsed, and a collapsed section is not a place a reader can go.
The frequency row follows the parts of speech, in the order the sections stand on the page.

## `public LCompass(`

Takes the view's controls as handles and subscribes to them itself.
The view, the header and the toggle re-place the contents, and scrolling syncs the current row.

## `private bool _lCompassOpened = true;`

Whether the reader left the contents open, flipped on each press of the toggle.
It starts open, as the toggle does in markup.
Keeping it here means placing never reads the control.

## `private void LCompassRowCreate()`

The rows are built in a local list, named, then copied into the shown list in one pass.
The shown list is therefore written in one place only.

## `public double LCompassLead { get; } = 14;`

How far above a target the view stops, so a heading never sits flush with the top edge.

## `public void LCompassSectionAttach(`

The eight sections the rows can name, handed over once the veneer has built them.

## `public void LCompassUpdate()`

The rows are built one dispatcher turn after the entry is shown.
A card row points at the container the list generated for it.
Containers do not exist until the layout pass has run.
The turn also comes after the veneer has given every section its visibility.

## `private void LCompassNameApply(List<LCompassItem> rows)`

Hands the row labels to the display and writes back the names it made distinct.
Two sections that share a label come back numbered, so the compass never shows two rows the same.

## `private static void LCompassCardAdd(List<LCompassItem> rows, ItemsControl cards, string kind, string unknown)`

The label comes from `LDisplayTitleRead`, which holds the naming rule.
The card number is carried apart from the label rather than written into it.
A number is not part of a sentence.

## `private void LCompassPlace()`

The contents hide themselves when the entry already fits the view.
A reader who can see the whole entry has nothing to navigate.
One row is not a table of contents either.

## `public double? LCompassOffsetRead(FrameworkElement target)`

Every position is measured at the moment it is needed rather than kept in a table.
A dragged seam, a resized window and a rebuilt card list all move the anchors.
None of them announce it.

## `private void LCompassSync()`

The current row is the last one whose anchor has passed the top of the view.
A reader who has scrolled past nothing is still reading the first section.

## `public void LCompassScroll(FrameworkElement target)`

Scrolls `target` to the top of the view, led by `LCompassLead`.
A row click and a card scroll both land here, so a card lands where its row would put it.
A target not shown in the view leaves the scroll where it is.

## `public void LCompassRowHandle(object sender)`

Scrolls to the section the clicked row names.
