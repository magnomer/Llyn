# QBerth.cs
Hash: `67c4719e362531eb`

## `public sealed class QBerth : Panel`

The wrap panel of a chip field, which seats the typing entry at the caret among the chips.
The chips are the field's items, and the entry is not one of them.
So moving the caret never touches the order of the rows the engine shows.
Children flow left to right and wrap onto a new line when the width runs out.

## `public static readonly DependencyProperty QBerthAnchorProperty`

The chip the entry stands before, set on the field and bound by its own items panel alone.
It is not inherited, so a chip field nested inside a chip never takes over the outer anchor.
No anchor, or an anchor no item carries, seats the entry after the last chip.

## `public static readonly DependencyProperty QBerthEntryProperty`

The entry element the panel seats, set on the field and bound by its own items panel alone.
It is not inherited, so a nested panel never seats the outer field's entry.

## `internal static ItemsControl QBerthBuild(ItemsControl list, object caret, string anchor)`

Builds the field's entry once, as a one-item list showing the caret through the field's own selector.
Binds the field's anchor to the caret's anchor property, so a caret step reseats the entry.
Answers the entry, so the driver fills it as it fills the chips.

## `protected override int VisualChildrenCount`

The chips, plus the entry once it is seated.

## `protected override IEnumerator LogicalChildren`

The chips, plus the entry, so resources and inherited values reach it.

## `protected override Visual GetVisualChild(int index)`

The children in the order they are laid out, so focus moves through them as they are seen.

## `protected override Size MeasureOverride(Size availableSize)`

Binds to the field once, seats a waiting entry, then measures the children line by line.

## `protected override Size ArrangeOverride(Size finalSize)`

Places the children line by line, each line as tall as its tallest child.

## `private static void QBerthLineApply(List<UIElement> row, double top, double height)`

Places one line of children side by side.

## `private static void QBerthEntryRefine(DependencyObject owner, DependencyPropertyChangedEventArgs e)`

Releases the seated entry and keeps the new one waiting.
An items panel must start with no child of its own.
So the entry is seated only at the next measure.

## `private void QBerthOwnerAttach()`

Binds the panel's anchor and entry to the field that owns it as its items panel, once.
The field sets both values, and only its own panel follows them.
A panel not yet hosted by a field waits for a later measure.

## `private void QBerthEntryAttach()`

Seats the waiting entry as a child of this panel.
An entry still held by a panel the field built earlier is released from it first.

## `private void QBerthEntryDetach()`

Releases the seated entry.

## `private int QBerthSeatRead()`

The place of the anchor chip among the children, or the end.

## `private IEnumerable<UIElement> QBerthOrderRead()`

The children in layout order, with the entry at its seat.
