# PCardGhost.cs

## `internal sealed class PCardGhost : Adorner`

The picture of a card that follows the pointer while that card is being dragged. The card itself is left on screen and keeps its place in the list until the drag moves it, so what travels with the pointer is this copy: the same card painted dimmed above the list, hit-testable by nothing so the list underneath still hears where the pointer is.

## `internal double PCardGhostTop`

Where the top of the ghost sits, in the coordinates of the list it is drawn over.

## Inline notes

### `_pCardGhostFace = new VisualBrush(card) { Stretch = Stretch.None };`

A brush of the live card rather than a snapshot of it: the card keeps its own look, and the ghost is that look painted somewhere else.
