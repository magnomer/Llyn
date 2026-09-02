# PTagTemplate.xaml

## `Theme.Tag.Field`

The Tag field: one bordered surface holding a wrapping run of boxed Tags with the open entry after them.
The height is not fixed.
The run wraps and the surface grows with it.
So a card that carries many Tags shows all of them instead of hiding them behind a scroll.

The surface is the field, not the entry.
So the border reacts to focus anywhere within it.
A click on empty space inside it reaches the caret.

## `Theme.Tag.Chip`

One committed Tag: its text and the button that closes it.

## `Theme.Tag.Entry`

The caret at the end of the run.
It shares the ordinary input style, so its placeholder behaves as every other field's does.
But it carries no border of its own.
The surface around the whole field is the border.

## `PTagTemplate.xaml.cs`

The dictionary forwards its events to the editor that owns the cards.
The Example, Situation, Image and Video dictionaries do the same.
The templates are shared by both card kinds.
So the handling belongs to the one editor above them rather than to a copy inside each.
