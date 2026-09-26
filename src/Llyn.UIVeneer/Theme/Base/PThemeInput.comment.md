# PThemeInput.xaml

Every hover, press, check and disabled look is switched by `PLook` in the deportment.
Every template part takes its control's values through `PLook` rather than a template binding.

## `<Style x:Key="Theme.Input.Choice" TargetType="ComboBox">`

The toggle over the whole field opens the dropdown, its check bound to the dropdown by `PLook`.
The arrow names its asset by pack URI, since a markup extension from code would be a hook.

## `<Style x:Key="Theme.Input.Tab" TargetType="Button">`

A tab of the editor's strip, whose selected or idle state `PLook` reads from its tag.
The style sets no tag, so the editor tags each tab when its strip attaches.

## `<Style x:Key="Theme.Text.Reading" TargetType="TextBlock">`

A reading with no text takes no room, collapsed by `PLook`.

## `<Setter Property="Template" Value="{DynamicResource Theme.Input.Field.Template}" />`

The TextBox content host must be named "PART_ContentHost" (a WPF template contract).
That name is authored in code (PField) so it stays out of the audited XAML surface.

## `<Style x:Key="Theme.Input.Bare" TargetType="TextBox">`

A field with no frame of its own, for writing straight over what the reading side draws.
Its frame is drawn outward on hover and focus by "Theme.Input.Field.Bare", so the text never moves.
