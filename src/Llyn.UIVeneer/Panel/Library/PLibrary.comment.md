# PLibrary.xaml
Hash: `83b94e908469cc1d`

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the entry catalog from the entry it opens.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin so it meets the navigation's own edge.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything, which is the whole rule.
A line marks a division, and a box would claim an object.

## `<Border x:Name="POrder" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one control rather than two boxes.
The bar owns the ground and the focus ring, so both children are drawn bare.
Its outline shows only under the pointer or on focus.
Its width is the index column's, so its edge is the same as the catalog below.
The viewer deliberately has no control in this row.
The ordering picker is the shared `PChoiceOrder`, placed here as `PLibraryOrder`.
The language filter is the shared `PChoiceFilter`, placed here as `PLibraryFilter`.
Their own drivers set their icons and popups, and the panel wires the search field.

## `<local:QRail Grid.Row="0" Grid.Column="1" Margin="0,0,0,18">`

The entry actions over the reader, and the reader and editor toggle.
The command row is the shared `PPanelRail`, placed here as `PLibraryRail`.
The import button `PLibraryMarkup` stands after it as its own group, because no other panel imports.
The outer rail sets the shared row and the import group side by side.
It stacks them when the room is too narrow.

## `<ItemsControl x:Name="PIndex">`

The entry catalog, one `Theme.Catalog.Row` per Entry.
Its row parts are named, and the shared index fill paints them and marks the chosen row.

## `<veneer:PDisplay x:Name="PDisplay" />`

The read-only entry view is shared, so it is a control rather than markup written here.
The phonology panel and the taxonomy panel show an entry the same way.
What differs between the panels is which entry is selected, not how it reads.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
The scroll bar takes its lane only when the list overflows, and the rows then narrow to make room.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.
