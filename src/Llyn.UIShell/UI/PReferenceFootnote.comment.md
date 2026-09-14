# PReferenceFootnote.xaml

## `ResourceDictionary`

The row of the entry list beside the read area of the sources panel.
The row reads the flag, the headword and the language, as a cohort row does, and leads to that Entry.
It is a dictionary rather than markup in the panel, because the panel outgrew one file.
The read area's own styles moved out with it into `PColophon.xaml`, where the reading now lives.

## Inline notes

### `<Button Style="{DynamicResource Theme.Footnote.Row}">`

The row style stays with the panel and is reached by name at runtime.
A style built on another cannot be resolved in a dictionary that stands on its own.
