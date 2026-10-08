# PPhonology.xaml
Hash: `4c2070af2e07035a`

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the pronunciation catalog from the entry it opens.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin so it meets the navigation's own edge.
The second row seam stands only while the articulation aid is unfolded.
A folded aid leaves no row for it to close.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything.
That is the whole rule, since a line marks a division and a box would claim an object.

## `<Border x:Name="PSequence" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one control over the inventory column.
The library panel joins its own two controls the same way.
The articulation fold stands outside this bar, because it shapes the editor rather than the catalog.
The ordering picker is the shared `PChoiceOrder`, placed here as `PPhonologyOrder`.
The language filter is the shared `PChoiceFilter`, placed here as `PPhonologyFilter`.

## `<ToggleButton x:Name="PArticulationHelper" ... />`

The fold control of the input aid, standing beside the ordering and the search.
The three controls that shape what the panel shows are read as one row.
It carries the height, the corner and the marked label of the bar beside it.
The row therefore reads as one control rather than two shapes.
The aid is two full charts, so it is folded away until it is asked for.

## `<Rectangle x:Name="PArticulationSeam" ... Visibility="Collapsed" />`

The seam and the aid start folded, because the toggle starts unchecked.
`QArticulation` shows both when the toggle is checked, since the markup carries no fold binding.

## `<veneer:PPanelRail x:Name="PPhonologyRail" Grid.Row="0" Grid.Column="1" Margin="0,0,0,18" />`

The action row of a browse-style panel, on the broader side.
The command row is the shared `PPanelRail`, placed here as `PPhonologyRail`.
It is the same row the library panel carries, and it names the same four actions.
One slot sits between save and export, and it holds whichever pair the mode asks for.
Reading shows the pair that walks the window's trail of records.
Writing shows the pair that walks the chronicle of the editor in front.
The mode toggle stands there rather than inside the display, because the display is shared.

## `<veneer:PArticulation ... Grid.ColumnSpan="2" />`

The input aid spans both columns.
It is bled to both panel edges like the seams.
Its ground is a band across the panel rather than a box inside it.
It serves the pronunciation field on the right, and the reader reads the catalog on the left.
Neither column owns it.

## `<ItemsControl x:Name="PInventory">`

A row reads as the pronunciation first and the word after it.
That is the order this panel browses in.
The word is quieter than the pronunciation, because the pronunciation is what is being looked for.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
The scroll bar takes its lane only when the list overflows, and the rows then narrow to make room.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.
