# PReferenceColophon.xaml

## `ResourceDictionary`

The label and value styles of the read area, and the citation row it lists.
The citation row leads to the Entry or Example holding it, and carries that side's language.
It is a dictionary rather than markup in the panel, because the panel outgrew one file.

## Inline notes

### `<Button Style="{DynamicResource Theme.Footnote.Row}">`

The row style stays with the panel and is reached by name at runtime.
A style built on another cannot be resolved in a dictionary that stands on its own.
