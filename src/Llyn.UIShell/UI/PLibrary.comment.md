# PLibrary.xaml

## `<Grid x:Name="POrder" Grid.Row="0" Grid.Column="0" Margin="0,0,20,18">`

The two controls share the index column, so their combined edge is the same as the catalog below.
The viewer deliberately has no control in this row.

## `<local:PDisplay x:Name="PDisplay" />`

The read-only entry view is shared, so it is a control rather than markup written here.
The phonology panel and the taxonomy panel show an entry the same way.
What differs between the panels is which entry is selected, not how it reads.
