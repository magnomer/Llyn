# PCardGhost.cs
Hash: `11d4107967115302`

## `internal sealed class PCardGhost : Adorner`

The picture of a card that follows the pointer while that card is being dragged.
The card itself is left on screen and keeps its place in the list until the drag moves it.
So what travels with the pointer is this copy.
It is the same card painted dimmed above the list.
It is hit-testable by nothing, so the list underneath still hears where the pointer is.

## `internal PCardGhost(UIElement list, FrameworkElement card, double left)`

Takes the card's size once, so the ghost keeps the size the drag started with.
The left edge is fixed here, so the ghost moves only up and down the list.

## `internal double PCardGhostTop`

Where the top of the ghost sits, in the coordinates of the list it is drawn over.

## Inline notes

### `_pCardGhostFace = new VisualBrush(card) { Stretch = Stretch.None };`

A brush of the live card rather than a snapshot of it.
The card keeps its own look, and the ghost is that look painted somewhere else.
