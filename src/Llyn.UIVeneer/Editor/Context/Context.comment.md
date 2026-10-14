# Context

This folder holds the Situation field of a card being written, as markup alone.
The editor's markup merges it, and the editor's fill subscribes its own methods on each part.
The card fill picks the chip or the entry for each item and fills the named parts.

## `PContextTemplate.xaml`

### `Theme.Context.Field`

The field is a wrapping run of boxed Situations with the open entry after them.
`QBerth` lays out the run and seats the entry before the chip the caret is anchored to.
The height is not fixed.
So a card with many Situations shows all of them instead of hiding them behind a scroll.
A Situation is a chip on both sides of the card, so writing one looks like reading one.

### `PContextFrame`

The surface is the field, not the entry.
A click on empty space inside it reaches the caret through the handler the card fill subscribes.

### `Theme.Context.Chip`

The wording reads the unknown mark when the store could not read it back.
So a chip showing nothing else says which kind of empty it is.
The chip carries its own cursor, so the field's text cursor stops at its edge.
A committed Situation is not text to edit, and its close button points at a click.
The fill sets the close icon, since an icon is drawn by code.

### `Theme.Context.Entry`

The caret is seated among the chips by the field's panel rather than held as one of them.
It takes the caret input style, a bare field that takes no room while it is empty and unfocused.
It carries no border, because the field around it is the border.
Its keys are read before the box reads them.
The box would otherwise swallow the arrows for its own caret and the suggestion list would never see them.
Its frame is drawn outward, so the entry is inset by that same amount.
Its room stands as tall as a chip.
So it opens where a chip opens, and while focused or written it keeps a chip-wide gap after the run.

## Stamp

Commit: `c6e60141a172`
- `PContextTemplate.xaml` `8ac460332082c5b1`
