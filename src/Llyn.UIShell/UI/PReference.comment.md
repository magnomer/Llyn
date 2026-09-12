# PReference.xaml

## `<UserControl.Resources>`

The row and card templates the panel draws with live in `PReferenceShelf.xaml` and `PReferenceColophon.xaml`.
Only the styles built on another style stay here, because a dictionary standing on its own cannot resolve one.

## `<Rectangle ... Style="{StaticResource Theme.Panel.SeamRow}" />`

The seams that part the source catalog from the source it opens.
The row seam runs under the ordering bar and the command row.
It is bled past the panel margin so it meets the navigation's own edge.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything, which is the whole rule: a line marks a division, a box would claim an object.

## `<Border x:Name="PGrade" ... Style="{StaticResource Theme.Search.Bar}">`

The ordering button and the search field are one control over the catalog column.
The action row over the broader column stands in the same top row, as the corpus panel arranges it.
The catalog column takes 380, because a row carries a name over an author-and-year line.

## `<Popup x:Name="PGradeDropdown" ...>`

The orderings the shelf may be listed in, one radio row each, carrying its choice in `Tag`.
There is no ordering by kind of material, because no Source-type discriminator is stored.
Ordering by recency is not offered, because the `source` row carries no creation or modification time.
A field standing Unspecified or Unknown still has a place in every ordering, ordered by its state.

## `<StackPanel Grid.Row="0" Grid.Column="1" ...>`

The action row of the panel.
`PReferenceFresh` opens the edit area on a Source nothing cites yet.
That is the normal case, because a Source is written down first and cited afterwards.
Export and print are mock-up controls and are not wired.

## `<ItemsControl x:Name="PShelf" Button.Click="PShelfHandle" ...>`

Every Source the workspace holds, including one nothing cites.
The row itself is drawn by a template the merged dictionary holds.
The click of every row is taken here, because a template in a dictionary can name no handler.
`PShelfEmpty` covers both an empty workspace and a search that matches nothing.

## `<Grid x:Name="PColophon">`

The read area, this panel's own rather than the Entry-shaped `PDisplay`.
Every field is shown, including one standing Unspecified, so a reader learns what the Source does not state.
The mode toggle is not in here.
It sits in the action row as it does in every built panel.

## `<ItemsControl x:Name="PFootnote" Button.Click="PFootnoteHandle" ...>`

The Entries and Examples citing the selected Source, each row leading to the panel that holds it.
The row is drawn by a template the merged dictionary holds, so the click is taken over the whole list.
A Situation is not listed, because a Situation is written rather than quoted and cites no Source.
The list is read-only, because a citation is set or cleared where it is held.

## `<local:PImprint x:Name="PImprint" Visibility="Collapsed" />`

The edit area, a control of its own and described in `PImprint.comment.md`.
It sits over the read area in the same cell, because one Source is either being read or being written.
The panel decides which is shown, so the mode toggle stays in the action row.

## Catalog spacing

The catalog uses the shared Theme.Catalog.Frame and Theme.Catalog.Scroll styles.
Rows keep 6 device-independent pixels on both sides, with a reserved scrollbar lane so their width stays stable.
The shared Theme.Catalog.Row preserves the same rounded shape, internal padding and row spacing across browse panels.
