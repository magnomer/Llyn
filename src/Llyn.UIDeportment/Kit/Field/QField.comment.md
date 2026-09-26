# QField.cs

## `internal static readonly DependencyProperty QFieldHintProperty`

The placeholder text a field shows while it is empty, attached to the field.
The deportment sets it by resource key, so it follows a language switch.
The field templates and the bare choice read it, so no hint travels through `Tag`.

## `internal static void QFieldGhostAttach(TextBlock ghost, TextBox field)`

Binds a measuring ghost to its field: the text, or the hint while the field is empty.
The choice is made by `QFieldGhost`, so the ghost follows every text and hint change by itself.

## Inline notes

### `internal static class QField`

Builds the reusable text-input field control template in code.
The content host must be named "PART_ContentHost".
WPF's TextBox looks that exact part up to mount its text editor.
The name lives here as a string literal, never as XAML x:Name.
So the framework contract is honored without the name entering the audited XAML surface.
Registered by QFieldApply under the key the "Theme.Input.Field" style binds its Template setter to.

### `var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");`

No Margin binding here: WPF already offsets the editable text by TextBox.Padding internally.
Binding the host's Margin to Padding too would apply it twice.
The caret would be pushed right and down while the placeholder, padded once, stayed put.

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

## `internal static void QFieldNoteShow(TextBox box, string note)`

The same guarded write for the note box, compared without the trailing line breaks the box may hold.

## `internal static string QFieldNoteRead(TextBox box)`

The note box's text without its trailing line breaks, which the engine never stores.

## `internal static string QFieldPathRead(TextBox box)`

The property name a templated field writes, read from the field's own name.
A card learns from it which field changed.
A combo box's inner editor reads the combo's name instead.
The returned templates carry no binding, so each writing field is named after its property.

## `internal static void QFieldFocusDefer(ItemsControl host, object? item)`

Puts the caret at the end of the field inside the row for `item`, once the row's container exists.
The container is generated after layout, so the focus waits for the loaded pass.

## `private static readonly Thickness QFieldPlaceholderInset`

The room a placeholder is held off the caret by.
It is the same in a bare field as in a framed one.
A placeholder starting where the text starts is written under the caret.
A writer reads the caret as part of the word.
Both templates hold their placeholder off by this much, so a field in a card sits as the headword does.

## `internal static void QFieldPlaceholderShow(TextBlock block, bool placeholder)`

Dresses a reading-side text block as the placeholder its edit-side field would show, or as a value again.
A field that reads its unknown mark as a placeholder must read the same on the reading side.
So the inset, the muted colour and the fade are taken from here rather than guessed there.
Then the edit side and the reading side put the same word in the same place.

## `private static ControlTemplate QFieldBareBuild()`

Builds the field a card uses while it is being edited.
A card must read the same in both modes, so the text sits where the reading side draws it.
Padding would move it, so the frame is a sibling drawn behind with a negative margin instead.
The frame therefore grows outward on hover and focus and never shifts the text by a pixel.
How far outward is measured by "QFieldConverter" rather than fixed, since a card writes its fields at several faces.
A fixed measure framed each face at its own height, and the card showed a different box on every line.
It is registered under "Theme.Input.Field.Bare", which the "Theme.Input.Bare" style binds to.

### `pSurface.SetValue(UIElement.SnapsToDevicePixelsProperty, true);`

The measured margin is seldom a whole pixel, since a face's line spacing is not.
A hairline drawn at a half pixel is smeared over two rows and reads as a faint grey.
Snapping the frame lands each edge on one row so all four sides carry the same colour.

### `pContent.SetValue(FrameworkElement.MarginProperty, new Thickness(-2, 0, 0, 0));`

WPF insets an editable text host two pixels from the left even when the padding is zero.
A reading view draws the same words with a TextBlock, which has no such inset.
Pulling the host back by those two pixels lands the written word on the read word exactly.

## `internal const string QFieldSurfaceName`

The frame is named for the template's own triggers and for whatever must hang off it from outside.
The Situation dropdown is placed against it, so the name is shared rather than written twice.

## `private static ControlTemplate QFieldPlainBuild()`

Builds the field a search bar writes into.
A search bar already draws its own frame around the dropper, the divider and the field together.
The bare template would draw a second frame inside that one on hover and focus.
The reader saw a blue box nested in a blue box and read it as two controls.
So this template carries the placeholder and the text host alone and leaves the framing to the bar.
It is registered under "Theme.Input.Field.Plain", which the "Theme.Search.Field" style binds to.

## `internal static readonly DependencyProperty QFieldCellProperty`

The field a row cell grows only while it is being edited.
A row list drew one text box per cell.
A Han character with twenty-six readings therefore built over a hundred editors.
Each editor is a control template, a text editor with its undo stack, and an accessibility peer that reports itself.
Nobody edits more than one cell at a time.
So the rows now draw the same text blocks the reading view draws.
A click on a cell puts one text box over that block, and the box leaves when focus does.
The editor and the reading view therefore share the cell's styles, and the two modes match to the pixel.
A cell declares its editor through the attached `QFieldCellProperty`.

## `internal sealed record QFieldCell(string QFieldCellStyle, string QFieldCellPath, string? QFieldCellHint);`

A cell's editor order: the box style key, the bound property, and an optional placeholder key.
The row fill is the one place it is written.

## `internal static void QFieldCellAttach(UIElement host)`

Watches the list `host` for presses, so every cell beneath it grows its editor on demand.

## `private static void QFieldPressHandle(object sender, MouseButtonEventArgs e)`

Finds the ordered cell under the press and puts an editor into it, unless one already stands there.
The press then goes no further, since the box is given focus and the caret at the pressed glyph.
A box that cannot take focus is taken out again at once, so no editor is ever left standing unfocused.
A cell without a text block or a known box style is left alone.
So is one whose style key nothing resolves.
The box takes the text block's tooltip, so a reflex language still shows its region while edited.

## `private static bool QFieldTrailCheck(TextBox box, Point point, int index)`

Whether the press landed past the middle of the glyph at `index`, so the caret goes after it.

## `private static TextBox QFieldCellBuild(Style style, object row, string path, string? hint)`

Builds the editor with the given style and a two-way binding to `path` on `row` updating on every keystroke.
The row is bound as an explicit source, so the text is settled before the box joins the tree.
A text change raised while joining would otherwise reach the editor as an edit the user never made.
The placeholder resource becomes its hint.
It listens for its own loss of keyboard focus, which is when it leaves.

## `private static void QFieldBlurHandle(object sender, KeyboardFocusChangedEventArgs e)`

Focus has moved on, so the editor leaves its cell.

## `private static void QFieldCellDetach(TextBox box, Grid cell)`

Takes the editor out of its cell and shows the text block again.
The box leaves the tree before its binding is cleared, so the text change that clearing raises reaches nobody.
The block's visibility is cleared rather than set, so a style that hides an empty block keeps ruling it.

## `private static Grid? QFieldCellFind(DependencyObject origin, UIElement host)`

The nearest ordered grid above the pressed element, or null when the press was outside every cell.
The walk stops at the host, so an order on the host's own ancestors is never taken for a cell.
A text run is not a visual, so the walk climbs the logical tree until it reaches one.
