# PPhonology.xaml

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the pronunciation catalog from the entry it opens.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin so it meets the navigation's own edge.
The second row seam stands only while the articulation aid is unfolded.
A folded aid leaves no row for it to close.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything, which is the whole rule: a line marks a division, a box would claim an object.

## `<Border x:Name="PSequence" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one control over the inventory column.
The library panel joins its own two controls the same way.
The articulation fold stands outside this bar, because it shapes the editor rather than the catalog.

## `<ToggleButton x:Name="PArticulationHelper" ... />`

The fold control of the input aid, standing beside the ordering and the search.
The three controls that shape what the panel shows are read as one row.
It carries the height, the corner and the marked label of the bar beside it.
The row therefore reads as one control rather than two shapes.
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
It is bled to both panel edges like the seams.
Its ground is a band across the panel rather than a box inside it.
It serves the pronunciation field on the right, and the reader reads the catalog on the left.
Neither column owns it.

## `<ItemsControl x:Name="PInventory">`

A row reads as the pronunciation first and the word after it.
The catalog keeps 6 device-independent pixels between each row and either panel seam.
The left inset offsets the panel's wider command margin.
The right inset is the reserved 6-pixel scroll lane.
Its compact scrollbar stays inside the catalog, and rows retain their width when the list starts scrolling.
The row's own padding and vertical spacing remain separate from these outer gutters.
That is the order this panel browses in.
The word is quieter than the pronunciation, because the pronunciation is what is being looked for.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
Rows keep 6 device-independent pixels on both sides, with a reserved scrollbar lane so their width stays stable.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.
