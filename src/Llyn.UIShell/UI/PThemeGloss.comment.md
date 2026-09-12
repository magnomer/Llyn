# PThemeGloss.xaml

The look of a Gloss row, shared by the card editor and the corpus panel.
The input part is merged so the remove control and the boxed field stand on the same styles as every other input.

## `<local:PStateConverter x:Key="Theme.Gloss.State" />`

Turns the unknown mark of a row into the placeholder its field shows, with the localized hint when the row is blank.

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

The inline row under a card's sentence: the flag that opens the picker, the bare text field in the muted colour, and the cross.
The margin under each row keeps two neighbouring frames apart for the same reason.
The flag toggle is pulled left by its own padding, so the flag starts where the sentence above starts.
The text takes the family, size and slant the entry's language pack declares for a Gloss, through the card resources the panel sets.

## `<DataTemplate x:Key="Theme.Gloss.Field">`

The boxed row of the corpus edit area: the language toggle with flag and name, the boxed text field, and the cross.

## `<DataTemplate x:Key="Theme.Gloss.Display">`

The read-only line of the corpus read area: the flag and the text, or the unknown mark when the text is not known.
