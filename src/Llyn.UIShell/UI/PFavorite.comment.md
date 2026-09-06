# PFavorite.xaml

## `<Grid x:Name="PSeries" Grid.Row="0" Grid.Column="0" Margin="0,0,20,18">`

The sort control and the search field share the roster column, so their edge matches the catalog below.
The panel has no new-record button, because an entry is written in the input tab and never here.

## `<local:PDisplay x:Name="PDisplay" />`

The read-only entry view is shared, so it is a control rather than markup written here.
The favorite entries read exactly as they do in the other entry-browsing panels.
What differs between the panels is which entry is selected, not how it reads.
