# QField.cs
Hash: `a821f672842cf9dc`

## `internal static class QField`

The shared helpers every text field leans on.
It owns the hint property, the placeholder dress, the popup place and the caret walk.
The field templates live in [QEnvelope](QEnvelope.comment.md).
The editor a row cell grows lives in [QQuill](QQuill.comment.md).

## `internal static readonly DependencyProperty QFieldHintProperty`

The placeholder text a field shows while it is empty, attached to the field.
The deportment sets it by resource key, so it follows a language switch.
The field templates and the bare choice read it, so no hint travels through `Tag`.

## `internal static void QFieldGhostAttach(TextBlock ghost, TextBox field)`

Binds a measuring ghost to its field.
The ghost shows the text, or the hint while the field is empty.
The choice is made by `QFieldGhost`, so the ghost follows every text and hint change by itself.

## `internal static CustomPopupPlacement[] QFieldPopupPlace(Size popup, Size target, Point offset)`

Places a popup under its field, or above it when the screen ends, overlapping the shade the popup frame paints.

## `internal static FrameworkElement? QFieldSurfaceFind(object? source)`

The surface border of a field's template, which a popup lines up with, or the field itself without one.
Anything but a field answers null, so a handler may pass an event source through unchecked.

## `internal static void QFieldTextShow(TextBox box, string text)`

Writes a fact into a box.
An unchanged text is no change to the property.
So WPF leaves the caret of a box being typed into alone.
The draft echoes each deferred request back, so that write must stay a no-op.

## `internal static void QFieldFocusDefer(ItemsControl host, object? item)`

Puts the caret at the end of the field inside the row for `item`, once the row's container exists.
The container is generated after layout, so the focus waits for the loaded pass.

## `internal static readonly Thickness QFieldPlaceholderInset`

The room a placeholder is held off the caret by.
It is the same in a bare field as in a framed one.
A placeholder starting where the text starts is written under the caret.
A writer reads the caret as part of the word.
Every `QEnvelope` template holds its placeholder off by this much, so a card field sits as the headword does.

## `internal static void QFieldPlaceholderShow(TextBlock block, bool placeholder)`

Dresses a reading-side text block as the placeholder its edit-side field would show, or as a value again.
A field that reads its unknown mark as a placeholder must read the same on the reading side.
So the inset, the muted colour and the fade are taken from here rather than guessed there.
Then the edit side and the reading side put the same word in the same place.

## `internal const string QFieldSurfaceName`

The frame is named for the template's own triggers and for whatever must hang off it from outside.
The slate, proffer and drawer dropdowns are placed against it, so the name is shared rather than written twice.

## `internal static void QFieldCaretApply(TextBox box, object row, int caret)`

The chip fields share one way of reaching the entry they are typed into.
The Context, Label, Link and Register fields are each a run of chips with one open entry among them.
No entry can be bound by name, because it is one item of a templated collection.
So all find it by walking the drawn field, and all put the caret back the same way.
Moving the entry rebuilds its item, so the focused box is gone by the time the move lands.
The caret is put back on the newly drawn entry once the field is laid out again.
A caret past the end of the text lands at the end, since the text box clamps it itself.
Without it a step across a chip would drop the user out of the field.

## `internal static ItemsControl? QFieldHostFind(DependencyObject start)`

The field the moved entry will be redrawn in, found from the box being left behind.

## `internal static TextBox? QFieldCaretFind(DependencyObject root)`

The entry is the one text box a chip field holds.
A committed chip carries a button and a label, never a field.
So the first text box found under the field is the caret the user types into.
