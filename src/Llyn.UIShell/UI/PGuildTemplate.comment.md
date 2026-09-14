# PGuildTemplate.xaml

## `ResourceDictionary`

The rows the authors panel draws with: the roll row, the co-author row, the citation row and the union row.
It is a dictionary rather than markup in the panel, because four templates would outgrow one file.

## Inline notes

### `<DataTemplate x:Key="Guild.Roll.Card">`

One Author in the roll: its mark, its name, its source count, and a pellet counting the places citing those.
The mark is handed in by the row, because the uncredited row wears a different one.

### `<DataTemplate x:Key="Guild.Fellow.Card">`

One co-author in the vita: the name and a pellet counting the Sources crediting both.

### `<DataTemplate x:Key="Guild.Citation.Card">`

One place citing the Author's sources: the flag, the headword or sentence, what names the card, and its kind.

### `<DataTemplate x:Key="Guild.Union.Card">`

One Author the union field matched: the name and its source count, so the kept row is chosen knowingly.

### `<Button Style="{DynamicResource Theme.Roll.Row}">`

The row styles stay with the panel and are reached by name at runtime.
A style built on another cannot be resolved in a dictionary that stands on its own.
