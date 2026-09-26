# PTenor.xaml

## `<Grid Margin="34,20,34,38">`

Three columns, narrow to wide: the register catalog, the Entries marked with the chosen Register, then the reader.
The taxonomy panel asks for an Entry by a label somebody put on one of its cards.
This panel asks for it by how formal the wording on that card is.
So the catalog here is a Register, and the entry list beside it is what carries that Register.

## `<Border x:Name="PDegree" ...>`

The sorting button and the search field are one control over the register column.
Both act on the register catalog and nothing else.

## The third ordering

A register catalog offers most-marked-first, which the tag catalog does not.
A Register carries a real count of the cards marked with it.
It is therefore a question worth asking of the shelf.
A tag catalog has no such count to sort on.

## `<Grid Grid.Row="1" Grid.Column="1" Style="{StaticResource Theme.Catalog.Middle}">`

The entry list is a bare catalog column, not a boxed surface, exactly as the taxonomy panel draws it.
Both lists on this panel are asked the same kind of question, so both are drawn the same way.
Each row carries the flag, the headword and the language, in the library catalog's order.

## `<Border x:Name="PGrille" ... Style="{StaticResource Theme.Search.Bar}">`

The search field and the language filter over the entry column.
`PQuest` narrows the entries marked with the chosen Register by typed text.
`PGrilleDropper` opens the menu of loaded languages, and `PGrilleMark` shows while any is hidden.

## `<local:PRail Grid.Row="0" Grid.Column="2" Margin="0,0,0,18">`

The entry actions over the reader, and the reader and editor toggle.
Export and print stand here as on every panel that reads an entry.

## Hooks

The markup carries no hook.
The Deportment class `PTenor` sets icons, commands, clicks, popups and row fills.
It also folds the two button pairs by mode.
`PTenorChronicle` starts collapsed because the reader is the side shown first.
