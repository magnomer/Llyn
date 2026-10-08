# PXiesheng.xaml
Hash: `174da3828cc4c1a6`

## `<Grid Margin="34,20,34,38">`

Three columns, narrow to wide: the series of the language, the entries of the chosen series, then the reader.
The panel is the rime-table panel with one catalog fewer, since a series names its cell by itself.
The toolbars, the reader and the editor are the shape every catalog tab shares.

## `<Border x:Name="PRungBar"`

The search bar of the series column, with the ordering picker beside the field.
The picker is the shared `PChoiceOrder`, placed here as `PXieshengOrder`.

## `<veneer:PPanelRail x:Name="PXieshengRail" Grid.Row="0" Grid.Column="2" Margin="0,0,0,18" />`

The entry actions over the reader, and the reader and editor toggle.
The command row is the shared `PPanelRail`, placed here as `PXieshengRail`.

## `<ItemsControl x:Name="PGrove"`

The series of the language, each with the count of entries it reaches.
Its parts are named, and `QGroveItem.QGroveItemRefine` fills them and marks the chosen row.

## `<ItemsControl x:Name="PKindred"`

The entries of the chosen series, each with its language flag and epithet.
Its parts are named, and `QKindredItem.QKindredItemRefine` fills them and marks the chosen row.

## `<veneer:PStem x:Name="PXieshengStem"`

The series page, shown in the reader while a series is chosen and no entry is.
