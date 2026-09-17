# PThemeGloss.xaml

The look of a Gloss row, shared by the card editor and the corpus panel.
The input part is merged.
So the remove control and the boxed field stand on the same styles as every other input.

## `<local:PStateConverter x:Key="Theme.Gloss.State" />`

Reads the row's text value into the field and the placeholder beside it.
The field binds the value one way, and the placeholder reads the unknown mark or the localized hint.
What is typed leaves through the editor's own handler, so the row holds no copy of it.

## `<Style x:Key="Theme.Gloss.Line" TargetType="ItemsControl">`

The list of rows, folded away while it holds none.
A bare field draws its hover frame past its own bounds, above and below the text.
The margin above the list keeps the first row's frame clear of the sentence's frame.

## `<Style x:Key="Theme.Gloss.Choice" TargetType="ListBoxItem">`

One language of the picker, lit when the pointer rests on it or it is the chosen one.

## `<DataTemplate x:Key="Theme.Gloss.Option">`

A language of the picker as its flag and its name.

## `<Style x:Key="Theme.Gloss.Globe" TargetType="Ellipse">`

The ring drawn in place of a flag while the row has no language yet.

## `<Style x:Key="Theme.Gloss.Flag" TargetType="Image">`

The flag of the row's language, hidden while there is none.

## `<Style x:Key="Theme.Gloss.Name" TargetType="TextBlock">`

The name of the row's language, or the muted language label while none is chosen.

## `<Style x:Key="Theme.Gloss.Remove" TargetType="Button" BasedOn="{StaticResource Theme.Input.Action}">`

The cross that drops the row.
It raises `PGlossCommand.PGlossCommandRemoval` with the row as its parameter, so the dictionary needs no code of its own.

## `<DataTemplate x:Key="Theme.Gloss.Row">`

The inline row under a card's sentence: the flag that opens the picker and the bare text field.
The field is in the muted colour, and the cross follows it.
The margin under each row keeps two neighbouring frames apart for the same reason.
The flag toggle is pulled left by its own padding, so the flag starts where the sentence above starts.
The text takes the family, size and slant the entry's language pack declares for a Gloss.
They come through the card resources the panel sets.

## `<DataTemplate x:Key="Theme.Gloss.Field">`

The boxed row of the corpus edit area: the language toggle with flag and name.
The boxed text field and the cross follow it.

## `<DataTemplate x:Key="Theme.Gloss.Display">`

The read-only line of the corpus read area: the flag and the text.
The unknown mark stands in when the text is not known.
