# Shengfu

This folder holds the xiesheng panel and the page of one phonetic series.
The panel is the rime-table panel with one catalog fewer, since a series names its cell by itself.
The toolbars, the reader and the editor are the shape every catalog tab shares.

## `PXiesheng`

The shell behind `PXiesheng.xaml`, placed by the main window.
Its constructor only calls `InitializeComponent()`.
Three columns run from narrow to wide.
They hold the series of the language, the entries of the chosen series, then the reader.

## `PRungBar`

The search bar of the series column, with the ordering picker beside the field.
The picker is the shared `PChoiceOrder`, placed as `PXieshengOrder`.

## `PXieshengRail`

The entry actions over the reader, and the reader and editor toggle.
The command row is the shared `PPanelRail`.

## `PGrove` and `PKindred`

`PGrove` lists the series of the language, each with the count of entries it reaches.
`PKindred` lists the entries of the chosen series, each with its language flag and epithet.
Their parts are named, and `QGroveItem.QGroveItemRefine` and `QKindredItem.QKindredItemRefine` fill them and mark the chosen row.

## `PXieshengStem`

The series page, shown in the reader while a series is chosen and no entry is.

## `PStem`

The shell behind `PStem.xaml`, the series page the xiesheng panel places.
Its constructor only calls `InitializeComponent()`.
The page prints the series key as a headword, the series chip and the language, then the members.
The members stack, one under another, each with its reading and its reflex rows.

## `PStemEmpty`

Stands in while the series holds no character to print.

## `PStemList`

One row per member, its character opening the entry written with it.
The list is the shared-size scope, so the reflex columns line up across members as on the entry page.
The rows are filled and the chip command bound by `QStem`, since the markup holds no binding.

## Stamp

Commit: `c2f335c331fe`
- `PStem.xaml` `d984654ec6ca540a`
- `PStem.xaml.cs` `a5432e520addf565`
- `PXiesheng.xaml` `174da3828cc4c1a6`
- `PXiesheng.xaml.cs` `5c1dfd69772c6ff0`
