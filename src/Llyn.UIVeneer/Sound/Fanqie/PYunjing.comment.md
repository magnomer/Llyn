# PYunjing.xaml
Hash: `d9c0aa22d09f72d9`

## `<Grid Margin="34,20,34,38">`

Four columns, narrow to wide: the onset categories, the rime categories, the entries at their cell, then the reader.
The panel is a rime table read as a question.
Pick an onset and a rime, see that cell's entries.
It is the tenor panel's shape with one more catalog, so the toolbars, the reader and the editor read alike.

## `<local:QSeam ...>`

Three seams, one before each column after the first, so every column can be dragged.
Only the first two widths persist, as the layout store keeps a left and a middle width.

## `<Border x:Name="PLadder" ...>`

The ordering picker and the search field over the onset column, one control like the tenor panel's.
The picker is the shared `PChoiceOrder`, placed here as `PYunjingOrder`.

## `<Border x:Name="PStair" ...>`

The same control over the rime column, with its own ordering and search.
Its picker is a second `PChoiceOrder`, placed here as `PYunmuOrder`.

## `<TextBox x:Name="PBeacon" ...>`

The search field over the entries of the cell.
No language filter stands beside it, since the panel reads one language.

## `<Style x:Key="Theme.Yunjing.Key" TargetType="TextBlock" BasedOn="{StaticResource Theme.Catalog.Title}">`

The category key at 26 points, since one Han character reads better large.
The size is this panel's alone, and the entry column keeps the catalog size.

## `<DataTemplate x:Key="Theme.Yunjing.Cell">`

One category row: its key and the count of entries under it, shared by the onset and rime columns.
Its parts are named, and `PYunjingItem.PYunjingItemRefine` fills them and marks the chosen row.

## `<veneer:PPanelRail x:Name="PYunjingRail" Grid.Row="0" Grid.Column="3" Margin="0,0,0,18" />`

The entry actions over the reader, and the reader and editor toggle.
The command row is the shared `PPanelRail`, placed here as `PYunjingRail`.
