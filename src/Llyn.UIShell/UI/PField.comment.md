# PField.cs

## Inline notes

### `internal static class PField`

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

### `private static ControlTemplate PFieldBareBuild()`

Builds the field a card uses while it is being edited.
A card must read the same in both modes, so the text sits where the reading side draws it.
Padding would move it, so the frame is a sibling drawn behind with a negative margin instead.
The frame therefore grows outward on hover and focus and never shifts the text by a pixel.
It is registered under "Theme.Input.Field.Bare", which the "Theme.Input.Bare" style binds to.
