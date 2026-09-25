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

### `<Style x:Key="Editor.Field.Measure" TargetType="TextBlock">`

The same twin for the pronunciation field.
An empty field is measured by its hint, so the brackets never close on nothing.
