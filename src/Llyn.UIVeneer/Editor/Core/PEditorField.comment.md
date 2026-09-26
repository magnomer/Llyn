# PEditorField.xaml

## `ResourceDictionary`

The styles of the fields at the head of the editor, apart from their layout.
Each is a rule about one box rather than a piece of the form's shape.

## Inline notes

### `<Style x:Key="Editor.Field.Ghost" TargetType="TextBlock">`

The unseen twin that measures the headword, standing in the same PGauge as the box.
It alone is measured, so the cell closes on the text rather than on the box.
A box measures wider than its text, by its caret and its scroll host.
An empty box is measured by the word it asks for, so the row never closes on nothing.
The editor writes that text, so the style holds only how the twin stands.
The pronunciation twin wears the theme measure style straight, since nothing else was left in its own.
