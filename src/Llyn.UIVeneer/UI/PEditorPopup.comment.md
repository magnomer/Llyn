# PEditorPopup.xaml

## `ResourceDictionary`

The shells the editor's dropdowns are drawn in, apart from the layout that opens them.
Five lists hang off this one editor and three of them were written out row by row.
A row that lights on hover and fills when picked is one behaviour, so it is declared once.

## Inline notes

### `<Style x:Key="Editor.Popup.Stretch" TargetType="ContentPresenter">`

A row in a picker takes the width of the picker, not of its own text.
Notation, clip and category lists all say this and nothing else about their containers.

### `<Style x:Key="Editor.Popup.Frame" TargetType="Border">`

The surface a caret's dropdown stands on, with the gutter its shadow needs.
Only the width follows the caret it was opened from, so that stays in the markup.

### `<Style x:Key="Editor.Popup.List" TargetType="ListBox">`

A picker is a list that is read and clicked, never tabbed into or scrolled sideways.
The caret keeps the focus while its list is open.

### `<Style x:Key="Editor.Popup.Row" TargetType="ListBoxItem">`

The translation row, drawn against a field several times a caret's size.
It carries the height and the corner a reader of that panel expects.

### `<Style x:Key="Editor.Popup.Caret" TargetType="ListBoxItem">`

The row a caret's dropdown carries, cut down from the translation row.
Situation and slate lists are drawn against a short caret, so both take this one.
