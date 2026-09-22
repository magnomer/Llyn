# PEtymologySelector.cs

## `internal sealed class PEtymologySelector`

Picks the chip template or the typing entry for one item of the etymology field.

## `public DataTemplate? PEtymologySelectorChip`

The template of a settled source link.

## `public DataTemplate? PEtymologySelectorCaret`

The template of the entry a word is typed into.

## `public override DataTemplate? SelectTemplate(object item, DependencyObject container)`

The entry is the one item that is not a chip.
