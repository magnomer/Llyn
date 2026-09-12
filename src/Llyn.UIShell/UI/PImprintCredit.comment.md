# PImprintCredit.xaml

## `ResourceDictionary`

The credit row of the Source edit area, and the author list the crediting menu offers.
A credit row carries the order controls, the rename and the drop, in the Source's own order.

## Inline notes

### `<Button ... Tag="Earlier" />`

Each row button says in its `Tag` which action it is.
A dictionary carries no code, so the four cannot name four handlers.
The edit area reads the `Tag` of whichever button raised the click.
