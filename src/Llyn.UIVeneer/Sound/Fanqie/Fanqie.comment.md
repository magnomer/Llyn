# Fanqie

This folder holds the yunjing panel, a rime table read as a question.
Pick an onset and a rime, and see that cell's entries.
It is the tenor panel's shape with one more catalog, so the toolbars, the reader and the editor read alike.

## `PYunjing`

The shell behind `PYunjing.xaml`, placed by the main window.
Its constructor only calls `InitializeComponent()`.

## Layout

Four columns run from narrow to wide.
They hold the onset categories, the rime categories, the entries at their cell, then the reader.
Three seams stand, one before each column after the first, so every column can be dragged.
Only the first two widths persist, as the layout store keeps a left and a middle width.

## `PLadder` and `PStair`

`PLadder` is the ordering picker and the search field over the onset column, one control like the tenor panel's.
Its picker is the shared `PChoiceOrder`, placed as `PYunjingOrder`.
`PStair` is the same control over the rime column, with its own ordering and search.
Its picker is a second `PChoiceOrder`, placed as `PYunmuOrder`.

## `PBeacon`

The search field over the entries of the cell.
No language filter stands beside it, since the panel reads one language.

## `Theme.Yunjing.Key`

The category key at 26 points, since one Han character reads better large.
The size is this panel's alone, and the entry column keeps the catalog size.

## `Theme.Yunjing.Cell`

One category row with its key and the count of entries under it, shared by the onset and rime columns.
Its parts are named, and `PYunjingItem.PYunjingItemRefine` fills them and marks the chosen row.

## `PYunjingRail`

The entry actions over the reader, and the reader and editor toggle.
The command row is the shared `PPanelRail`.

## Stamp

Commit: `c2f335c331fe`
- `PYunjing.xaml` `d9c0aa22d09f72d9`
- `PYunjing.xaml.cs` `d0edf76c7720b903`
