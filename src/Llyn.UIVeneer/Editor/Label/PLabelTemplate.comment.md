# PLabelTemplate.xaml

## `Theme.Label.Field`

The Tag field: one bordered surface holding a wrapping run of boxed Tags with the open entry after them.
The height is not fixed.
The run wraps and the surface grows with it.
So a card that carries many Tags shows all of them instead of hiding them behind a scroll.

The surface is the field, not the entry.
So the border reacts to focus anywhere within it.
A click on empty space inside it reaches the caret.

## `Theme.Label.Chip`

One committed Tag: its text and the button that closes it.
The chip carries its own cursor, so the field text cursor stops at its edge.
A committed Tag is not text to edit, and its close button points at a click.

## `Theme.Label.Entry`

The caret at the end of the run.
It answers the key before the text box does, because a text box keeps the arrow keys for itself.
Otherwise the down arrow could never reach the dropdown of stored tags.
It shares the ordinary input style, so its placeholder behaves as every other field's does.
But it carries no border of its own.
The surface around the whole field is the border.

## `PLabelTemplate.xaml.cs`

The dictionary forwards its events to the editor that owns the cards.
The Example, Situation, Image and Video dictionaries do the same.
The templates are shared by both card kinds.
So the handling belongs to the one editor above them rather than to a copy inside each.
