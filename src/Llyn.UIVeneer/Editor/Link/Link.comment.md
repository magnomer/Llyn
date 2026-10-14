# Link

This folder holds the Translation field of a card being written, as markup alone.
The editor's markup merges it, and the editor's fill subscribes its methods on each part.

## `PLinkTemplate.xaml`

### `Theme.Link.Field`

One surface holds a wrapping run of linked headwords with the open entry after them.
`QBerth` lays out the run and seats the entry before the chip the caret is anchored to.
The height is not fixed.
So a card that links many entries shows all of them instead of hiding them behind a scroll.

### `PLinkFrame`

The surface is the field, not the entry.
A click on empty space inside it reaches the caret through the handler the card fill subscribes.

### `Theme.Link.Chip`

The chip shows the target's flag, headword and language, because a link crosses languages.
It carries its own cursor, so the field text cursor stops at its edge.
A committed link is not text to edit, and its close button points at a click.
The fill sets the unlink icon, since an icon is drawn by code.

### `Theme.Link.Entry`

The field's panel seats the caret among the chips rather than holding it as one of them.
It answers the key before the text box does, because a text box keeps the arrow keys for itself.
Otherwise the down arrow could never reach the dropdown of offered entries.
It takes the caret input style, a bare field that takes no room while it is empty and unfocused.
It carries no border, because the surface around the whole field is the border.

## Stamp

Commit: `c6e60141a172`
- `PLinkTemplate.xaml` `f79a6de4c1275440`
