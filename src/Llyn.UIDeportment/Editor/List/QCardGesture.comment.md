# QCardGesture.cs
Hash: `58709ec8b5d4eafe`

## `internal sealed class QCardGesture`

One press of a card drag, from the press on its header until the drag ends.
`QCardDrag` makes one per press and drops it at the end, so no drag state outlives its press.
Everything it is handed at the press stays fixed, and only the ghost appears later.

## `internal QCardGesture(object item, long id, ItemsControl host, FrameworkElement container, double origin)`

Takes the pressed card as a list item, its id, its list, its container and where the pointer went down.
It takes only what it uses of the card, so the gesture names no card control.
The grip and the card's height are read off the container at once, since the list rearranges under the drag.

## `internal object QCardGestureItem { get; }`

The row being dragged, a handle on the item shown and never a copy of its list.
Its place is read off the list on screen at every move.
The editor keeps a row while its card lives, so the handle stays the card dragged.

## `internal long QCardGestureId { get; }`

The id of the card dragged, sent with every move.
A card's id is set when its row is made and never changes.
So it is read once at the press.

## `internal bool QCardGestureShow(MouseEventArgs e)`

A press is not a drag.
No ghost is painted until the pointer has travelled far enough.
Only then was the card meant to be moved rather than its header clicked.
This is the gesture alone, and it decides only whether the ghost is painted.
It answers false when a ghost cannot be painted, which ends the drag.

## `internal int QCardGestureResolve(MouseEventArgs e)`

Which place the ghost is now over.
The card is judged by its own edges and never by the pointer.
Where inside the header it was taken hold of would otherwise decide how far it travels.
Going up would then need a different distance from going down.
Its leading edge is what crosses a neighbour.
That is the top of it on the way up and the bottom of it on the way down.
So either direction swaps at the same half-card.
It is the same half-card whatever the grip was.

## `internal void QCardGestureClear()`

Takes the ghost off the list and lets the capture go.
The ghost would otherwise stay painted over the list after its drag is over.

## Inline notes

### `private readonly double _qCardGestureOrigin;`

Where the pointer went down, measured on the list.
No ghost is painted until the pointer has moved a drag's distance from it.

### `private readonly double _qCardGestureGrab;`

How far below the card's top the pointer went down.
The ghost is held at that same distance from the pointer.
So the card does not jump under the hand that took it.

### `private readonly double _qCardGestureHeight;`

How tall the dragged card is.
The order is decided from where the ghost is, not from where the pointer is.
The ghost is this tall.

### `private PCardGhost? _qCardGestureGhost;`

The only state that changes during a press.
It stays empty until the pointer has travelled a drag's distance and a ghost could be painted.

### `if (index < current && top < middle)`

Going up, the target is the first card whose middle the ghost's top has risen above.
Going down, it is the last card whose middle the bottom has fallen past.
So one long drag passes every card it crosses.
