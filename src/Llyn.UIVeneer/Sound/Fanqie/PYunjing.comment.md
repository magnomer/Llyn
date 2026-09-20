# PYunjing.xaml

## `<Grid Margin="34,20,34,38">`

Four columns, narrow to wide: the onset categories, the rime categories, the entries at their cell, then the reader.
The panel is a rime table read as a question: pick an onset and a rime, see that cell's entries.
It is the tenor panel's shape with one more catalog, so the toolbars, the reader and the editor read alike.

## `<local:PSeam ...>`

Three seams, one before each column after the first, so every column can be dragged.
Only the first two widths persist, as the layout store keeps a left and a middle width.

## `<Border x:Name="PLadder" ...>`

The sorting button and the search field over the onset column, one control like the tenor panel's.

## `<Border x:Name="PStair" ...>`

The same control over the rime column, with its own ordering and search.

## `<TextBox x:Name="PBeacon" ...>`

The search field over the entries of the cell.
No language filter stands beside it, since the panel reads one language.

## `<Style x:Key="Theme.Yunjing.Key" TargetType="TextBlock" BasedOn="{StaticResource Theme.Catalog.Title}">`

The category key at 1.8 times the catalog title size, since one Han character reads better large.
The size is this panel's alone, and the entry column keeps the catalog size.

## `<DataTemplate x:Key="Theme.Yunjing.Cell">`

One category row: its key and the count of entries under it, shared by the onset and rime columns.

## `<local:PRail Grid.Row="0" Grid.Column="3" Margin="0,0,0,18">`

The command rail over the reader: new, save, export and print, then the view and edit switch.
One slot sits between save and export, and it holds whichever pair the mode asks for.
Reading shows `PYunjingEarlier` and `PYunjingLater`, which walk the window's trail of records.
Writing shows `PYunjingBackward` and `PYunjingForward`, which walk the chronicle of the editor in front.
Each carries no label, only the arrow and a tooltip, and is lit only while a step is there.
