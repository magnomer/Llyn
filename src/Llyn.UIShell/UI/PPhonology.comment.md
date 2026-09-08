# PPhonology.xaml

## `<Border x:Name="PSequence" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one bordered control over the inventory column.
The library panel joins its own two controls the same way.
The articulation fold stands outside this bar, because it shapes the editor rather than the catalog.

## `<ToggleButton x:Name="PArticulationHelper" ... />`

The fold control of the input aid, standing beside the ordering and the search.
The three controls that shape what the panel shows are read as one row.
The aid is two full charts, so it is folded away until it is asked for.

## `<UserControl.Resources>`

The fold is a binding rather than a handler.
There is no state to keep, because the toggle already is the state.

## `<StackPanel Grid.Row="0" Grid.Column="1" ...>`

The action row of a browse-style panel, on the broader side.
It is the same row the library panel carries, and it names the same four actions.
The mode toggle stands there rather than inside the display, because the display is shared.

## `<local:PArticulation ... Grid.ColumnSpan="2" />`

The input aid spans both columns.
It serves the pronunciation field on the right, and the reader reads the catalog on the left.
Neither column owns it.

## `<ItemsControl x:Name="PInventory">`

A row reads as the pronunciation first and the word after it.
That is the order this panel browses in.
The word is quieter than the pronunciation, because the pronunciation is what is being looked for.
