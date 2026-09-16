# PFieldCell.cs

## `internal static partial class PField`

The field a row cell grows only while it is being edited.
A row list drew one text box per cell.
A Han character with twenty-six readings therefore built over a hundred editors.
Each editor is a control template, a text editor with its undo stack, and an accessibility peer that reports itself.
Nobody edits more than one cell at a time.
So the rows now draw the same text blocks the reading view draws.
A click on a cell puts one text box over that block, and the box leaves when focus does.
The editor and the reading view therefore share the cell's styles, and the two modes match to the pixel.
A cell declares its editor through its `Tag`.
The tag holds the box style key, the bound property, and an optional placeholder key.
The tag is data, not a name, and the row template is the one place it is written.

## `private const char PFieldCellSeparator`

What separates the three words of a cell's tag.

## `internal static void PFieldCellAttach(UIElement host)`

Watches the list `host` for presses, so every cell beneath it grows its editor on demand.

## `private static void PFieldPressHandle(object sender, MouseButtonEventArgs e)`

Finds the tagged cell under the press and puts an editor into it, unless one already stands there.
The press then goes no further, since the box is given focus and the caret at the pressed glyph.
A box that cannot take focus is taken out again at once, so no editor is ever left standing unfocused.
A cell without a text block or with a tag of fewer than two words is left alone.
So is one whose style key nothing resolves.

## `private static bool PFieldTrailCheck(TextBox box, Point point, int index)`

Whether the press landed past the middle of the glyph at `index`, so the caret goes after it.

## `private static TextBox PFieldCellBuild(Style style, object row, string path, string? hint)`

Builds the editor with the given style and a two-way binding to `path` on `row` updating on every keystroke.
The row is bound as an explicit source, so the text is settled before the box joins the tree.
A text change raised while joining would otherwise reach the editor as an edit the user never made.
The placeholder resource becomes its tag.
It listens for its own loss of keyboard focus, which is when it leaves.

## `private static void PFieldBlurHandle(object sender, KeyboardFocusChangedEventArgs e)`

Focus has moved on, so the editor leaves its cell.

## `private static void PFieldCellDetach(TextBox box, Grid cell)`

Takes the editor out of its cell and shows the text block again.
The box leaves the tree before its binding is cleared, so the text change that clearing raises reaches nobody.
The block's visibility is cleared rather than set, so a style that hides an empty block keeps ruling it.

## `private static Grid? PFieldCellFind(DependencyObject origin, UIElement host)`

The nearest tagged grid above the pressed element, or null when the press was outside every cell.
The walk stops at the host, so a tag on the host's own ancestors is never taken for a cell.
A text run is not a visual, so the walk climbs the logical tree until it reaches one.
