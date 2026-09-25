# PDisplayMeaning.xaml

## `ResourceDictionary`

The meaning card as the reading view draws it.
Its header carries the position and the title, and its body opens with the definition.
The rows under the definition are the shared card body.

## Inline notes

### `<ColumnDefinition Style="{DynamicResource Theme.Card.Gutter}" />`

The reading view reserves the strip the writing view puts its handles in, and leaves it empty.
A line must break at the same word in both modes.
It only can if it is given the same width in both.

### `<ResourceDictionary.MergedDictionaries>`

The shared header, body and text styles are reached dynamically, through the display around this meaning card.
A static reference would look only inside this file and fail while the card is being measured.
The three-state reading cannot be reached that way, so it is merged in.
