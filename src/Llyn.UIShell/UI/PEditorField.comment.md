# PEditorField.xaml

## `ResourceDictionary`

The styles of the fields at the head of the editor, apart from their layout.
Each is a rule about one box rather than a piece of the form's shape.

## Inline notes

### `<Style x:Key="Editor.Field.Hint" TargetType="TextBlock">`

The word asked for, shown only while the headword box is empty.
It is set as the headword is set, so what is asked for sits where the answer will.

### `<Style x:Key="Editor.Field.Ghost" TargetType="TextBlock">`

The unseen twin that measures the headword.
It decides the cell's width, so the row closes on the text rather than on a fixed box.

### `<Style x:Key="Editor.Field.Measure" TargetType="TextBlock">`

The same twin for the pronunciation field.
An empty field is measured by its hint, so the brackets never close on nothing.

### `<Style x:Key="Editor.Field.Mark" TargetType="Path">`

The heart the favourite toggle carries, hollow until the entry is kept.
A heart, not a star, because the stars beside it belong to the grasp row.
It is drawn in the theme's favorite rose rather than the accent, for the same reason.
It reads the toggle it sits in rather than a name, so the style needs no editor around it.
