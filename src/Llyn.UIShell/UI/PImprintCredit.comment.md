# PImprintCredit.xaml

## `ResourceDictionary`

The credit row of the Source edit area.
A credit row is one field naming the author it credits.
The add, the drop and the order controls stand beside it.
The handles are the entry's sentence handles, so the two lists are worked the same way.
The field is the whole of the row's editing, so no menu stands apart from the list.

## Inline notes

### `<Button ... Tag="Earlier" />`

Each row button says in its `Tag` which action it is.
A dictionary carries no code, so the four cannot name four handlers.
The edit area reads the `Tag` of whichever button raised the click.

### `<Style x:Key="Imprint.Credit.Field" ...>`

The field is bare and sits where the reading side draws the name, at the same size.
So the edit side reads as the view side does, with only the caret to tell them apart.

### `<Style x:Key="Imprint.Credit.Control" ...>`

The handles stay hidden until the pointer or the caret is in the list, as an entry's sentence handles do.
So the row at rest is the name alone, as the reading side shows it.

### `<Style x:Key="Imprint.Credit.Handle" ...>`

A handle that cannot act is faded rather than hidden, so the row keeps its shape.
The marks take the handle's own foreground, so the pointer over one lights it as the entry's handles light.

### `Text="{Binding PAuthorItemName, Mode=OneWay}"`

The field shows the credited name, and typing over it changes nothing until it is entered.
A changed credit list is a rebuilt list, so every field is refilled from its own row.
