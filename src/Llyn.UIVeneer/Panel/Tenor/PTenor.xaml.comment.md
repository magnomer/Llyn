# PTenor.xaml

## `<Grid Margin="34,20,34,38">`

Three columns, narrow to wide: the register catalog, the Entries marked with the chosen Register, then the reader.
The taxonomy panel asks for an Entry by a label somebody put on one of its cards.
This panel asks for it by how formal the wording on that card is.
So the catalog here is a Register, and the entry list beside it is what carries that Register.

## `<Border x:Name="PDegree" ...>`

The sorting button and the search field are one control over the register column.
Both act on the register catalog and nothing else.
The entry list has no toolbar because it is not searched.
It is what the chosen Register decides it is.

## `<Canvas Width="16" Height="14">`

Four ascending bars: the degree mark, drawn rather than fetched.
The taxonomy panel's mark descends and the library's is left-aligned, so no two panels read alike at a glance.
Ascending is the right figure here, because a register catalog is a range and not a filter.

## The third ordering

A register catalog offers most-marked-first, which the tag catalog does not.
A Register carries a real count of the cards marked with it.
It is therefore a question worth asking of the shelf.
A tag catalog has no such count to sort on.

## The chosen mark

The chosen Register stays visible while the eye is on the Entries beside it.
Without it the entry list would show a filtered set with nothing on screen saying which filter.

## `<Grid Grid.Row="1" Grid.Column="1" ...>`

The entry list is a bare catalog column, not a boxed surface, exactly as the taxonomy panel draws it.
Both lists on this panel are asked the same kind of question, so both are drawn the same way.
Each row carries the flag, the headword and the language, in the library catalog's order.

## `<local:PRail Grid.Row="0" Grid.Column="1" Grid.ColumnSpan="2" ...>`

The entry actions over the reader, and the reader/editor toggle.
Export and Print stand here as on every panel that reads an entry.

## `internal void PTenorVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void PTenorRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void PTenorAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `private void PTenorUndoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void PTenorRedoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void PTenorChronicleUpdate()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.
