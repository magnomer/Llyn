# PLabelTemplate.xaml

## `ResourceDictionary`

The Tag field of a card being written, as markup alone.
The editor's markup merges this dictionary, and the editor's fill subscribes its own methods on each part.
The editor's card fill picks the chip or the entry for each item and fills the named parts.

## `Theme.Label.Field`

The Tag field: one surface holding a wrapping run of boxed Tags with the open entry after them.
The run is laid out by `QBerth`, which seats the entry before the chip the caret is anchored to.
The height is not fixed.
The run wraps and the surface grows with it.
So a card that carries many Tags shows all of them instead of hiding them behind a scroll.

## `PLabelFrame`

The surface is the field, not the entry.
A click on empty space inside it reaches the caret through the handler the card fill subscribes.

## `Theme.Label.Chip`

One committed Tag: its text and the button that closes it.
The chip carries its own cursor, so the field text cursor stops at its edge.
A committed Tag is not text to edit, and its close button points at a click.
The close icon is set by the fill, since an icon is drawn by code.

## `Theme.Label.Entry`

The caret, seated among the chips by the field's panel rather than held as one of them.
It answers the key before the text box does, because a text box keeps the arrow keys for itself.
Otherwise the down arrow could never reach the dropdown of stored tags.
It shares the ordinary input style, so its placeholder behaves as every other field's does.
But it carries no border of its own.
The surface around the whole field is the border.
