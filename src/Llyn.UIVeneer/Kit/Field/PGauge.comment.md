# PGauge.cs

## `internal sealed class PGauge : Panel`

The two-child panel an edit field stands in, so the field lands where the read view puts its text.

The first child is the mark: an unseen text block holding the same words.
It takes the read view's own style.
It alone is measured, so the cell closes on the plain text width rather than on the field's own.
The second child is the field, arranged at its own width inside the mark's slot.
Room is left for its caret.

The panel is stretched to the row and both children are centered in it.
The read view centers its text block the same way.
A field wrapped in a centered box is rounded twice against a fractional text height, and lands a pixel off.
One stretched slot with one centered child is rounded once, on both sides alike.

### `InternalChildren[0].Arrange(new Rect(final));`

The mark is given the whole slot, so its own alignment places it, as it would in any panel.

### `new Rect(0, 0, InternalChildren[1].DesiredSize.Width, final.Height)`

The field is given its own width instead of the slot's.
A caret at the end of the text is then not cut off.
