# PField.cs

## Inline notes

### `internal static class PField`

Builds the reusable text-input field control template in code. The content host must be named "PART_ContentHost" because WPF's TextBox looks that exact part up to mount its text editor; the name lives here as a string literal (never as XAML x:Name) so the framework contract is honored without the name entering the audited XAML naming surface. Registered by PFieldApply under the key the "Theme.Input.Field" style binds its Template setter to.

### `var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");`

No Margin binding here: WPF already offsets the editable text by TextBox.Padding internally. Binding the host's Margin to Padding too would apply it twice, pushing the caret right and down while the placeholder (padded once) stayed put.
