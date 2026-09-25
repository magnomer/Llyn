# PLinkTemplate.xaml

## `Theme.Link.Field`

The Translation field: one bordered surface holding a wrapping run of linked headwords with the open entry after them.
The height is not fixed.
The run wraps and the surface grows with it.
So a card that links many entries shows all of them instead of hiding them behind a scroll.

The surface is the field, not the entry.
So the border reacts to focus anywhere within it.
A click on empty space inside it reaches the caret.

## `Theme.Link.Chip`

One committed link: the target's flag, its headword, its language and the button that closes it.
The language rides beside the headword because a link crosses languages.
The chip carries its own cursor, so the field text cursor stops at its edge.
A committed link is not text to edit, and its close button points at a click.

## `Theme.Link.Entry`

The caret at the end of the run.
It answers the key before the text box does, because a text box keeps the arrow keys for itself.
Otherwise the down arrow could never reach the dropdown of offered entries.
It shares the ordinary input style, so its placeholder behaves as every other field's does.
But it carries no border of its own.
The surface around the whole field is the border.

## `PLinkTemplate.xaml.cs`

The dictionary forwards its events to the editor that owns the cards.
The Example, Situation, Image, Video and Label dictionaries do the same.
The templates are shared by both card kinds.
So the handling belongs to the one editor above them rather than to a copy inside each.
