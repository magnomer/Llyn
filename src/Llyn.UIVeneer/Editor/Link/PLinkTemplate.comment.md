# PLinkTemplate.xaml

## `ResourceDictionary`

The Translation field of a card being written, as markup alone.
The Deportment class of the same name loads this markup and forwards its events to the editor.
The editor's card fill picks the chip or the entry for each item and fills the named parts.

## `Theme.Link.Field`

The Translation field: one surface holding a wrapping run of linked headwords with the open entry after them.
The height is not fixed.
The run wraps and the surface grows with it.
So a card that links many entries shows all of them instead of hiding them behind a scroll.

## `PLinkFrame`

The surface is the field, not the entry.
A click on empty space inside it reaches the caret through the handler the card fill subscribes.

## `Theme.Link.Chip`

One committed link: the target's flag, its headword, its language and the button that closes it.
The language rides beside the headword because a link crosses languages.
The chip carries its own cursor, so the field text cursor stops at its edge.
A committed link is not text to edit, and its close button points at a click.
The unlink icon is set by the fill, since an icon is drawn by code.

## `Theme.Link.Entry`

The caret at the end of the run.
It answers the key before the text box does, because a text box keeps the arrow keys for itself.
Otherwise the down arrow could never reach the dropdown of offered entries.
It shares the ordinary input style, so its placeholder behaves as every other field's does.
But it carries no border of its own.
The surface around the whole field is the border.
