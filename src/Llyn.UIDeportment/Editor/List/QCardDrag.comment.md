# QCardDrag.cs

## `internal sealed class QCardDrag`

Dragging a card to another place in its list.
The card is dragged by its header.
A ghost of it follows the pointer while the card itself stays on screen.
The list rearranges under that ghost as the pointer crosses the middle of a neighbour.
So the order is already the new order when the button comes up.
The engine is told each move as it happens, and hands back the numbering.
The list order is the stored order, so no drag is left for the store to discover.
The drag state is the gesture's alone, so it stays in the deportment.

## `internal QCardDrag(FrameworkElement surface)`

Finds both card lists by their markup ids and hears the pointer on each.
The lists, not the cards, hear the move and the release, since the capture is taken on the list.

## `internal void QCardDragIntroduce(CEditor editor)`

Holds the Conduct editor whose move gate the drag reaches.

## Inline notes

### `private PCard? _qCardDragCard;`

The row being dragged, a handle on the card shown and never a copy of its list.
Its id is read off the row when the move is sent.
Its place is read off the list on screen at every move.
The editor keeps a row while its card lives, so the handle stays the card dragged.

### `private double _qCardDragOrigin;`

Where the pointer went down, and how far below the card's top that was.
The ghost is held at that same distance from the pointer.
So the card does not jump under the hand that took it.

### `private double _qCardDragHeight;`

How tall the dragged card is.
The order is decided from where the ghost is, not from where the pointer is.
The ghost is this tall.

### `internal void QCardDragRefine(object sender, MouseButtonEventArgs e)`

The press on a card's header, heard for both lists.
It finds which list shows the card, where it was taken hold of, and takes the capture.
The list is the one whose items hold the card, since each list shows exactly its cards.

### `if (host is null || host.Items.Count <= 1)`

A list of one has nowhere else to put its card, so no drag ghost begins on it.
This only decides whether the drag is shown, since a lone card's drag could never send a move.

### `host.CaptureMouse();`

Captured on the list and not on the card.
The card slides out from under the pointer as soon as the order changes.
It is the list that has to keep hearing the pointer until the button comes back up.

### `private void QCardDragShow(object sender, MouseEventArgs e, double pointer, double top)`

A press is not a drag.
No ghost is painted until the pointer has travelled far enough.
Only then was the card meant to be moved rather than its header clicked.
This is the gesture alone, and it decides only whether the ghost is painted.
A ghost that cannot be painted ends the drag.

### `QCardDragShow(sender, e, pointer, top);`

The move is judged from the ghost's edges whether or not the ghost is painted yet.
The drag distance is far shorter than half a card, so a click never finds a new place.
Whether the place is a move at all is the move gate's answer, not the view's.

### `if (_qCardDragHost is null)`

A ghost that could not be painted has already ended the drag, so nothing is left to move.

### `private void QCardDragReset(object sender, MouseEventArgs e)`

Ends the drag, heard both when the button comes up and when capture is lost.
Capture can be taken away without the button ever coming up, by another window or a menu.
A ghost left painted over the list would outlive the drag it belongs to.

### `private void QCardDragMove(double top)`

Which place the ghost is now over.
The card is judged by its own edges and never by the pointer.
Where inside the header it was taken hold of would otherwise decide how far it travels.
Going up would then need a different distance from going down.
Its leading edge is what crosses a neighbour.
That is the top of it on the way up and the bottom of it on the way down.
So either direction swaps at the same half-card.
It is the same half-card whatever the grip was.

### `_cEditor.CEditorList.CCardMove(_qCardDragCard!.PCardId, target);`

The place the geometry found goes to the one move gate the position badge also uses.
It is sent on every pointer move, even when the place is the one the card holds.
The gate sends nothing for an unchanged place or a card the draft lacks.
The list is moved by the answer, not here.
Moving it first would show an order the engine may yet refuse.

### `if (index < current && top < middle)`

Upward: the first card whose middle the ghost's top has risen above.
Downward: the last card whose middle its bottom has fallen past, so one long drag passes every card it crosses.

### `_qCardDragHost = null;`

Cleared before the capture is released.
Letting go raises the lost-capture notice, which comes straight back here.
It must find a drag that is already over.
