# PThemeInput.xaml

## `<Setter Property="Template" Value="{DynamicResource Theme.Input.Field.Template}" />`

The TextBox content host must be named "PART_ContentHost" (a WPF template contract).
That name is authored in code (PField) so it stays out of the audited XAML surface.

## `<Style x:Key="Theme.Input.Bare" TargetType="TextBox">`

A field with no frame of its own, for writing straight over what the reading side draws.
Its frame is drawn outward on hover and focus by "Theme.Input.Field.Bare", so the text never moves.
