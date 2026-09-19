# PField.cs

## Inline notes

### `internal static partial class PField`

Builds the reusable text-input field control template in code.
The content host must be named "PART_ContentHost".
WPF's TextBox looks that exact part up to mount its text editor.
The name lives here as a string literal, never as XAML x:Name.
So the framework contract is honored without the name entering the audited XAML surface.
Registered by PFieldApply under the key the "Theme.Input.Field" style binds its Template setter to.

### `var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");`

No Margin binding here: WPF already offsets the editable text by TextBox.Padding internally.
Binding the host's Margin to Padding too would apply it twice.
The caret would be pushed right and down while the placeholder, padded once, stayed put.

## `internal static CustomPopupPlacement[] PFieldPopupPlace(Size popup, Size target, Point offset)`

Places a popup under its field, or above it when the screen ends, overlapping the shade the popup frame paints.

## `internal static FrameworkElement? PFieldSurfaceFind(object? source)`

The surface border of a field's template, which a popup lines up with, or the field itself without one.
Anything but a field answers null, so a handler may pass an event source through unchecked.

## `internal static void PFieldTextShow(TextBox box, string text)`

Writes a fact into a box only when it differs.
The caret of a box being typed into then stays put.
The draft echoes each deferred request back, and an unchanged write would jump the caret to the start.

## `internal static void PFieldNoteShow(TextBox box, string note)`

The same guarded write for the note box, compared without the trailing line breaks the box may hold.

## `internal static string PFieldNoteRead(TextBox box)`

The note box's text without its trailing line breaks, which the engine never stores.

## `internal static string PFieldPathRead(TextBox box)`

The bound property name of a templated field, read from its text binding.
A card learns from it which field changed.
A combo box's inner editor reads the combo's own text binding.

## `internal static void PFieldFocusDefer(ItemsControl host, object? item)`

Puts the caret at the end of the field inside the row for `item`, once the row's container exists.
The container is generated after layout, so the focus waits for the loaded pass.

## `private static readonly Thickness PFieldPlaceholderInset`

The room a placeholder is held off the caret by.
It is the same in a bare field as in a framed one.
A placeholder starting where the text starts is written under the caret.
A writer reads the caret as part of the word.
Both templates hold their placeholder off by this much, so a field in a card sits as the headword does.

## `internal static void PFieldPlaceholderShow(TextBlock block, bool placeholder)`

Dresses a reading-side text block as the placeholder its edit-side field would show, or as a value again.
A field that reads its unknown mark as a placeholder must read the same on the reading side.
So the inset, the muted colour and the fade are taken from here rather than guessed there.
Then the edit side and the reading side put the same word in the same place.

## `private static ControlTemplate PFieldBareBuild()`

Builds the field a card uses while it is being edited.
A card must read the same in both modes, so the text sits where the reading side draws it.
Padding would move it, so the frame is a sibling drawn behind with a negative margin instead.
The frame therefore grows outward on hover and focus and never shifts the text by a pixel.
How far outward is measured by "PFieldConverter" rather than fixed, since a card writes its fields at several faces.
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

## `internal const string PFieldSurfaceName`

The frame is named for the template's own triggers and for whatever must hang off it from outside.
The Situation dropdown is placed against it, so the name is shared rather than written twice.

## `private static ControlTemplate PFieldPlainBuild()`

Builds the field a search bar writes into.
A search bar already draws its own frame around the dropper, the divider and the field together.
The bare template would draw a second frame inside that one on hover and focus.
The reader saw a blue box nested in a blue box and read it as two controls.
So this template carries the placeholder and the text host alone and leaves the framing to the bar.
It is registered under "Theme.Input.Field.Plain", which the "Theme.Search.Field" style binds to.
