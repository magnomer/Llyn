# PCardDrag.cs

## `public partial class PEditor`

Dragging a card to another place in its list.
The card is dragged by its header.
A ghost of it follows the pointer while the card itself stays on screen.
The list rearranges under that ghost as the pointer crosses the middle of a neighbour.
So the order is already the new order when the button comes up.
The engine is told each move as it happens, and hands back the numbering.
The list order is the stored order, so no drag is left for the store to discover.

## Inline notes

### `private double _pCardDragOrigin;`

Where the pointer went down, and how far below the card's top that was.
The ghost is held at that same distance from the pointer.
So the card does not jump under the hand that took it.

### `private double _pCardDragHeight;`

How tall the dragged card is.
The order is decided from where the ghost is, not from where the pointer is.
The ghost is this tall.

### `if (list is null || list.Count <= 1)`

A list of one has nowhere else to put its card, so no drag begins on it.

### `host.CaptureMouse();`

Captured on the list and not on the card.
The card slides out from under the pointer as soon as the order changes.
It is the list that has to keep hearing the pointer until the button comes back up.

### `if (_pCardDragGhost is null)`

A press is not a drag.
Nothing is shown until the pointer has travelled far enough.
Only then was the card meant to be moved rather than its header clicked.

### `private void PCardDragCancel(object sender, MouseEventArgs e)`

Capture can be taken away without the button ever coming up, by another window or a menu.
A ghost left painted over the list would outlive the drag it belongs to.

### `private void PCardMove(double top)`

Which place the ghost is now over.
The card is judged by its own edges and never by the pointer.
Where inside the header it was taken hold of would otherwise decide how far it travels.
Going up would then need a different distance from going down.
Its leading edge is what crosses a neighbour.
That is the top of it on the way up and the bottom of it on the way down.
So either direction swaps at the same half-card.
It is the same half-card whatever the grip was.

### `PCardMove(_pCardDragCard!, target);`

The move is one shift request, and the list is moved by the answer, not here.
Moving it first would show an order the engine may yet refuse.

### `if (index < current && top < middle)`

Upward: the first card whose middle the ghost's top has risen above.
Downward: the last card whose middle its bottom has fallen past, so one long drag passes every card it crosses.

### `_pCardDragHost = null;`

Cleared before the capture is released.
Letting go raises the lost-capture notice, which comes straight back here.
It must find a drag that is already over.
