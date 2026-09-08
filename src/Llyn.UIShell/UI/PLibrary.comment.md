# PLibrary.xaml

## `<Border x:Name="POrder" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one bordered control rather than two boxes.
The bar owns the ground, the outline and the focus ring, so both children are drawn bare.
Its width is the index column's, so its edge is the same as the catalog below.
The viewer deliberately has no control in this row.

## `<local:PDisplay x:Name="PDisplay" />`

The read-only entry view is shared, so it is a control rather than markup written here.
The phonology panel and the taxonomy panel show an entry the same way.
What differs between the panels is which entry is selected, not how it reads.
