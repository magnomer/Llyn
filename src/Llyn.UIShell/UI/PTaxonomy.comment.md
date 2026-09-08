# PTaxonomy.xaml

## `<Grid Margin="34,20,34,38">`

Three columns, narrow to wide: the tag catalog, the entries under the chosen tag, then the reader.
The library panel asks for an entry by its headword.
This panel asks for it by a label somebody put on one of its cards.
So the catalog here is a tag, and the entry list beside it is what that tag holds.

## `<Grid x:Name="PFunnel" Grid.Row="0" Grid.Column="0" Margin="0,0,20,18">`

The sorting button and the search field share the tag column.
Their combined edge is the same as the catalog below.
Both act on the tag catalog and nothing else.
The membership list has no toolbar because it is not searched.
It is what the chosen tag decides it is.

## `<Canvas Width="16" Height="14">`

Three centred descending bars: the funnel mark, drawn rather than fetched.
The library panel's sorting mark is left-aligned, so the two panels are told apart at a glance.

## `<DataTrigger Binding="{Binding PDirectoryItemChosen}" Value="True">`

The chosen tag stays visible while the eye is on the entries beside it.
Without it the membership list would show a filtered set with nothing on screen saying which filter.

## `<TextBlock x:Name="PMembershipEmpty" ...>`

Shown when the chosen tag holds no entry.
It also stands while no tag is chosen and the workspace is empty.
Either way there is nothing to read, which is what the line says.

## `<StackPanel Grid.Row="0" Grid.Column="2" Margin="0,0,0,18" HorizontalAlignment="Right" Orientation="Horizontal">`

The same entry actions the library panel offers, over the same reader.
Only New and the mode toggle are wired today.
Export and Print are present as the requested mock-up controls.
