# PXiesheng.xaml

## `<Grid Margin="34,20,34,38">`

Three columns, narrow to wide: the series of the language, the entries of the chosen series, then the reader.
The panel is the rime-table panel with one catalog fewer, since a series names its cell by itself.
The toolbars, the reader and the editor are the shape every catalog tab shares.

## `<Border x:Name="PRungBar"`

The search bar of the series column, with the ordering dropper beside the field.

## `<ItemsControl x:Name="PGrove"`

The series of the language, each with the count of entries it reaches.
Its parts are named, and `PGroveItem.PGroveItemApply` fills them and marks the chosen row.

## `<ItemsControl x:Name="PKindred"`

The entries of the chosen series, each with its language flag and epithet.
Its parts are named, and `PKindredItem.PKindredItemApply` fills them and marks the chosen row.

## `<local:PStem x:Name="PXieshengStem"`

The series page, shown in the reader while a series is chosen and no entry is.
