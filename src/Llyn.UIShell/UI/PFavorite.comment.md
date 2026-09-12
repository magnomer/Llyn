# PFavorite.xaml

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the favourite catalog from the entry it opens.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin so it meets the navigation's own edge.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything, which is the whole rule: a line marks a division, a box would claim an object.

## `<Border x:Name="PSeries" ... Style="{StaticResource Theme.Search.Bar}">`

The sort control and the search field are one control over the roster column.
Its edge matches the catalog below, and the outline around both is drawn once.
The panel has no new-record button, because an entry is written in the input tab and never here.

## `<local:PDisplay x:Name="PDisplay" />`

The read-only entry view is shared, so it is a control rather than markup written here.
The favorite entries read exactly as they do in the other entry-browsing panels.
What differs between the panels is which entry is selected, not how it reads.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
Rows keep 6 device-independent pixels on both sides, with a reserved scrollbar lane so their width stays stable.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.
