# PReferenceColophon.xaml

## `ResourceDictionary`

The title, heading, value and chip styles of the read area, and the row of the entry list beside it.
The chips are built on the speech chip, so the dictionary merges the speech theme to reach it.
The row reads the flag, the headword and the language, as a cohort row does, and leads to that Entry.
It is a dictionary rather than markup in the panel, because the panel outgrew one file.

## Inline notes

### `<Button Style="{DynamicResource Theme.Footnote.Row}">`

The row style stays with the panel and is reached by name at runtime.
A style built on another cannot be resolved in a dictionary that stands on its own.
