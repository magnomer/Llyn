# PDisplayCollocation.xaml

## `ResourceDictionary`

The collocation card as the reading view draws it.
Its body opens with the expression and then the meaning, where a meaning card carries the meaning alone.
Everything else it shares with the meaning card, down to the reserved gutter.

### `<ResourceDictionary.MergedDictionaries>`

The shared header, body and text styles are reached dynamically, through the display around this collocation card.
A static reference would look only inside this file and fail while the card is being measured.
The three-state reading cannot be reached that way, so it is merged in.
