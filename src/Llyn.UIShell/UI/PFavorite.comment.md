# PFavorite.xaml

## `<Border x:Name="PSeries" ... Style="{StaticResource Theme.Search.Bar}">`

The sort control and the search field are one bordered control over the roster column.
Its edge matches the catalog below, and the outline around both is drawn once.
The panel has no new-record button, because an entry is written in the input tab and never here.

## `<local:PDisplay x:Name="PDisplay" />`

The read-only entry view is shared, so it is a control rather than markup written here.
The favorite entries read exactly as they do in the other entry-browsing panels.
What differs between the panels is which entry is selected, not how it reads.
