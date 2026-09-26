# PLabelTemplate.xaml

## `ResourceDictionary`

The Tag field of a card being written, as markup alone.
The Deportment class of the same name loads this markup and forwards its events to the editor.
The editor's card fill picks the chip or the entry for each item and fills the named parts.

## `Theme.Label.Field`

The Tag field: one surface holding a wrapping run of boxed Tags with the open entry after them.
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

The caret at the end of the run.
It answers the key before the text box does, because a text box keeps the arrow keys for itself.
Otherwise the down arrow could never reach the dropdown of stored tags.
It shares the ordinary input style, so its placeholder behaves as every other field's does.
But it carries no border of its own.
The surface around the whole field is the border.
