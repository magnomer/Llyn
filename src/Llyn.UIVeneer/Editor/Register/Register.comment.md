# Register

This folder holds the Register field of a card being written, as markup alone.
The editor's markup merges it, and the editor's fill subscribes its methods on each part.

## `PRegisterTemplate.xaml`

### `Theme.Register.Field`

It arranges the committed register chips with the entry after them.
`QBerth` lays out the run and seats the entry before the chip the caret is anchored to.

### `PRegisterFrame`

The surface is the field, not the entry.
A click on empty space inside it reaches the caret through the handler the card fill subscribes.

### `Theme.Register.Chip`

The wording reads the unknown mark when the store could not read it back.
The fill sets the close icon, since an icon is drawn by code.

### `Theme.Register.Entry`

The field's panel seats the caret among the chips, and the fill subscribes its keys and its leaving.

## Stamp

Commit: `c2f335c331fe`
- `PRegisterTemplate.xaml` `8242d2e5bb8a410b`
