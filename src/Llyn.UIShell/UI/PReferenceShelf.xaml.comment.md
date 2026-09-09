# PReferenceShelf.xaml

## `ResourceDictionary`

The shelf row as the source catalog draws it.
The row shows the resolved name over its credits and year, with the citation count at the right.
It is a dictionary rather than markup in the panel, because the panel outgrew one file.

## Inline notes

### `<Button Style="{DynamicResource Theme.Shelf.Row}">`

The chosen-row style stays with the panel and is reached by name at runtime.
A style built on another cannot be resolved in a dictionary that stands on its own.

### `<Button>` without a click

The row names no handler, because a dictionary carries no code.
The panel takes the click over the whole list instead.
