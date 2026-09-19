# PEditorCaret.cs

## `public partial class PEditor`

The chip fields share one way of reaching the entry they are typed into.
The Tag field and the Situation field are both a run of chips with one open entry among them.
Neither entry can be bound by name, because it is one item of a templated collection.
So both find it by walking the drawn field, and both put the caret back the same way.

## `internal static void PEditorCaretApply(TextBox box, object row, int caret)`

Moving the entry rebuilds its item, so the focused box is gone by the time the move lands.
The caret is put back on the newly drawn entry once the field is laid out again.
Without it a step across a chip would drop the user out of the field.

## `internal static ItemsControl? PEditorHostFind(DependencyObject start)`

The field the moved entry will be redrawn in, found from the box being left behind.

## `internal static TextBox? PEditorCaretFind(DependencyObject root)`

The entry is the one text box a chip field holds.
A committed chip carries a button and a label, never a field.
So the first text box found under the field is the caret the user types into.
