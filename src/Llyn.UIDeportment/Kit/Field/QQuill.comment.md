# QQuill.cs
Hash: `f75f8cd176f99399`

## `internal static readonly DependencyProperty QQuillCellProperty`

The field a row cell grows only while it is being edited.
A row list drew one text box per cell.
A Han character with twenty-six readings therefore built over a hundred editors.
Each editor is a control template, a text editor with its undo stack, and an accessibility peer that reports itself.
Nobody edits more than one cell at a time.
So the rows now draw the same text blocks the reading view draws.
A click on a cell puts one text box over that block, and the box leaves when focus does.
The editor and the reading view therefore share the cell's styles, and the two modes match to the pixel.
A cell declares its editor through the attached `QQuillCellProperty`.

## `internal sealed record QQuillCell(string QQuillCellStyle, string QQuillCellPath, string? QQuillCellHint);`

A cell's editor order.
It holds the box style key, the bound property, and an optional placeholder key.
The row fill is the one place it is written.

## `internal static void QQuillIntroduce(UIElement host)`

Watches the list `host` for presses, so every cell beneath it grows its editor on demand.

## `private static void QQuillPressRefine(object sender, MouseButtonEventArgs e)`

Finds the ordered cell under the press and puts an editor into it, unless one already stands there.
The press then goes no further, since the box is given focus and the caret at the pressed glyph.
A box that cannot take focus is taken out again at once, so no editor is ever left standing unfocused.
A cell without a text block or a known box style is left alone.
So is one whose style key nothing resolves.
The box takes the text block's tooltip, so a reflex language still shows its region while edited.

## `private static bool QQuillTrailDraw(TextBox box, Point point, int index)`

Measures whether the press landed past the middle of the glyph at `index`, so the caret goes after it.

## `private static TextBox QQuillOpenIntroduce(Style style, object row, string path, string? hint)`

Builds the editor with the given style and a two-way binding to `path` on `row` updating on every keystroke.
The row is bound as an explicit source, so the text is settled before the box joins the tree.
A text change raised while joining would otherwise reach the editor as an edit the user never made.
The placeholder resource becomes its hint through `QFieldHintProperty`.
It listens for its own loss of keyboard focus, which is when it leaves.

## `private static void QQuillBlurRefine(object sender, KeyboardFocusChangedEventArgs e)`

Focus has moved on, so the editor leaves its cell.

## `private static void QQuillCloseTeardown(TextBox box, Grid cell)`

Unwires the editor and takes it out of its cell.
The box leaves the tree before its binding is cleared, so the text change that clearing raises reaches nobody.
Both callers then call `QQuillBlockRefine` to show the text block again.

## `private static void QQuillBlockRefine(Grid cell)`

Shows the cell's text block again once the editor has left.
The block's visibility is cleared rather than set, so a style that hides an empty block keeps ruling it.

## `private static Grid? QQuillCellFind(DependencyObject origin, UIElement host)`

The nearest ordered grid above the pressed element, or null when the press was outside every cell.
The walk stops at the host, so an order on the host's own ancestors is never taken for a cell.
A text run is not a visual, so the walk climbs the logical tree until it reaches one.
It only finds the cell the press refines, so it stays a private query and carries no role.
