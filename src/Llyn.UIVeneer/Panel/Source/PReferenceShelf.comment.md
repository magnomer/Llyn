# PReferenceShelf.xaml

## `ResourceDictionary`

The shelf row as the source catalog draws it.
The row shows the resolved name over its credits and year, with the citation count at the right.
It is a dictionary rather than markup in the panel, because the panel outgrew one file.
The authors panel merges it too, so a Source reads alike in both catalogs.

## Inline notes

### `<Button x:Name="PShelfRow" Style="{DynamicResource Theme.Catalog.Row}">`

The row wears the shared catalog row, reached by name at runtime.
A style built on another cannot be resolved in a dictionary that stands on its own.

### `<Button>` without a click

The row names no handler and binds nothing, because a dictionary carries no code.
Every part is named, and the shared shelf fill paints it and marks the chosen row.
Each panel takes the row's click once on the whole list, through a routed handler.
