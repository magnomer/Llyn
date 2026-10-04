# QEnvelope.cs
Hash: `3fa4b64ee223d726`

## `internal static class QEnvelope`

Builds the reusable text-input field control templates in code.
The content host must be named "PART_ContentHost".
WPF's TextBox looks that exact part up to mount its text editor.
The name lives here as a string literal, never as XAML x:Name.
So the framework contract is honored without the name entering the audited XAML surface.
The placeholder inset, its fade, the hint and the surface name are read from [QField](QField.comment.md).

## `internal static void QEnvelopeIntroduce(ResourceDictionary resources)`

Registers the three templates under the keys their styles bind a Template setter to.
The framed one goes under "Theme.Input.Field.Template", which the "Theme.Input.Field" style binds to.

## `private static ControlTemplate QEnvelopeBareIntroduce()`

Builds the field a card uses while it is being edited.
A card must read the same in both modes, so the text sits where the reading side draws it.
Padding would move it, so the frame is a sibling drawn behind with a negative margin instead.
The frame therefore grows outward on hover and focus and never shifts the text by a pixel.
How far outward is measured by "QFieldConverter" rather than fixed, since a card writes its fields at several faces.
A fixed measure framed each face at its own height, and the card showed a different box on every line.
It is registered under "Theme.Input.Field.Bare", which the "Theme.Input.Bare" style binds to.

## `private static ControlTemplate QEnvelopePlainIntroduce()`

Builds the field a search bar writes into.
A search bar already draws its own frame around the dropper, the divider and the field together.
The bare template would draw a second frame inside that one on hover and focus.
The reader saw a blue box nested in a blue box and read it as two controls.
So this template carries the placeholder and the text host alone and leaves the framing to the bar.
It is registered under "Theme.Input.Field.Plain", which the "Theme.Search.Field" style binds to.

## Inline notes

### `var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");`

No template binds a Margin to the host, since WPF already offsets the editable text internally.
Binding the host's Margin to Padding too would apply it twice.
The caret would be pushed right and down while the placeholder, padded once, stayed put.

### `pSurface.SetValue(UIElement.SnapsToDevicePixelsProperty, true);`

The measured margin is seldom a whole pixel, since a face's line spacing is not.
A hairline drawn at a half pixel is smeared over two rows and reads as a faint grey.
Snapping the frame lands each edge on one row so all four sides carry the same colour.

### `pContent.SetValue(FrameworkElement.MarginProperty, new Thickness(-2, 0, 0, 0));`

WPF insets an editable text host two pixels from the left even when the padding is zero.
A reading view draws the same words with a TextBlock, which has no such inset.
Pulling the host back by those two pixels lands the written word on the read word exactly.
