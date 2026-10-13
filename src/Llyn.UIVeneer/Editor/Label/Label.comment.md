# Label

This folder holds the speech menu's category row and the Tag field of a card being written, as markup alone.
The editor's markup merges them, and the editor's fill subscribes their methods on each part.

## `PCategoryTemplate.xaml`

`Theme.Category.Row` shows a selectable category name and a check mark when it is already taken.
The fill sets the mark's icon and shows it only for a taken category.

## `PLabelTemplate.xaml`

### `Theme.Label.Field`

One surface holds a wrapping run of boxed Tags with the open entry after them.
`QBerth` lays out the run and seats the entry before the chip the caret is anchored to.
The height is not fixed.
So a card with many Tags shows all of them instead of hiding them behind a scroll.

### `PLabelFrame`

The surface is the field, not the entry.
A click on empty space inside it reaches the caret through the handler the card fill subscribes.

### `Theme.Label.Chip`

The chip carries its own cursor, so the field text cursor stops at its edge.
A committed Tag is not text to edit, and its close button points at a click.
The fill sets the close icon, since an icon is drawn by code.

### `Theme.Label.Entry`

The field's panel seats the caret among the chips rather than holding it as one of them.
It answers the key before the text box does, because a text box keeps the arrow keys for itself.
Otherwise the down arrow could never reach the dropdown of stored tags.
It shares the ordinary input style, so its placeholder behaves as every other field's does.
It carries no border, because the surface around the whole field is the border.

## Stamp

Commit: `c2f335c331fe`
- `PCategoryTemplate.xaml` `2925083bcf2f8563`
- `PLabelTemplate.xaml` `5d27ba0fa4b2105f`
