# QCompass.cs
Hash: `8b59e79e86a917b1`

## `public sealed class QCompass`

The floating table of contents over the read-only entry view, and the driver that keeps it true.
It belongs to the Display control rather than the panels around it.
A panel that reads an Entry through Display therefore gets one without asking.
The comparison panel gets one per side.
The editor has none, because a writer already knows where the card they are typing into is.

The rows follow what the view is actually showing, never the draft alone.
A section the entry left empty is collapsed, and a collapsed section is not a place a reader can go.
The frequency row follows the parts of speech, in the order the sections stand on the page.

## `public QCompass(CCompass area, FrameworkElement view)`

Takes the display's compass, whose `CCompassRead` answers the ready rows.
Pulls its controls and the six sections the rows can name from `view` by contract ID.
It subscribes to them itself.
Each section is paired with the part Conduct knows it by, in page order.
The two card lists are kept apart, so a card row finds its container by its part.
The view, the header and the toggle re-place the contents, and scrolling syncs the current row.
The toggle is set from `_qCompassOpened`, so the two start alike whatever the markup says.
The toggle's compass icon is resolved here too, since the toggle is this driver's.

## `private bool _qCompassOpened = true;`

Whether the reader left the contents open, flipped on each press of the toggle.
It starts open, as the toggle does in markup.
Keeping it here means placing never reads the control.
Each press writes it back to the toggle, so the control cannot drift from it.

## `private const double QCompassLead = 14;`

How far above a target the view stops, so a heading never sits flush with the top edge.

## `public void QCompassRefine()`

The rows are built one dispatcher turn after the entry is shown.
A card row points at the container the list generated for it.
Containers do not exist until the layout pass has run.
The turn also comes after the veneer has given every section its visibility.

## `public void QCompassEmptyRefine()`

Clears the rows and hides the column when the display closes.
A closed display shows no entry, so it has no place to point at.

## `private void QCompassListRefine()`

Hands Conduct the visible parts and the text lookup, then fills the rows it answers.
Which part is visible is a pixel fact, so the driver gathers it.
The wording, the card names and the numbering are Conduct's answer.

## `private void QCompassRowsRefine(IReadOnlyList<CCompassRow> rows)`

Pairs each answered row with the element it names and fills the shown list in one pass.
A card whose container the list has not generated is skipped, since the reader cannot go there.

## `private void QCompassColumnRefine()`

The contents hide themselves when the entry already fits the view.
A reader who can see the whole entry has nothing to navigate.
One row is not a table of contents either.

## `private double QCompassTopDraw(double side)`

Drops the column below the header when the header would run under it.
Otherwise the column sits at its usual height beside the header.

## `private double? QCompassOffsetDraw(FrameworkElement target)`

Every position is measured at the moment it is needed rather than kept in a table.
A dragged seam, a resized window and a rebuilt card list all move the anchors.
None of them announce it.

## `private void QCompassCurrentRefine()`

The current row is the last one whose anchor has passed the top of the view.
A reader who has scrolled past nothing is still reading the first section.

## `private void QCompassTargetRefine(FrameworkElement target)`

Scrolls `target` to the top of the view, led by `QCompassLead`.
A row click and a card scroll both land here, so a card lands where its row would put it.
A target not shown in the view leaves the scroll where it is.

## `public void QCompassRowRefine(object sender)`

Scrolls to the section the clicked row names, a look-only answer with no gate.

## `public void QCompassSpotlightRefine(long id)`

Scrolls the card with `id` into view and plays the spotlight on it.
The card containers exist one dispatcher turn after the entry is shown, so the scroll waits for the layout pass.

## `private void QCompassSpotlightRefine((CCompassPart, int)? place)`

Finds the container at the place `CCompassCardFind` answered and scrolls it through `QCompassTargetRefine`.
The card is thus led by the same distance as a row.
A card the entry no longer has is left unfound, and nothing moves.
