# PContextTemplate.xaml

## `Theme.Context.Field`

The Situation field: a wrapping run of boxed Situations with the open entry after them.
The height is not fixed.
The run wraps and the field grows with it.
So a card that carries many Situations shows all of them instead of hiding them behind a scroll.
A Situation is a chip on both sides of the card, so writing one looks like reading one.

The surface is the field, not the entry.
A click on empty space inside it reaches the caret.

## `Theme.Context.Chip`

One committed Situation: its wording and the button that closes it.
The wording reads the unknown mark when the store could not read it back.
So a chip showing nothing else says which kind of empty it is.
The chip carries its own cursor, so the field text cursor stops at its edge.
A committed Situation is not text to edit, and its close button points at a click.

## `Theme.Context.Entry`

The caret at the end of the run.
It shares the ordinary input style, so its placeholder behaves as every other field's does.
It carries no border of its own, because the field around it is the border.
Its keys are read before the box reads them.
The box would otherwise swallow the arrows for its own caret and the suggestion list would never see them.
Its frame is drawn outward, so the entry is inset by that same amount.
Its own room stands as tall as a chip stands.
That counts the line a chip is drawn with and the entry is not.
Otherwise the run grew by that line the moment a first Situation was committed.
The whole card below stepped down with it.
That way it opens where a chip opens and keeps a chip-wide gap after the run.

## `PContextTemplate.xaml.cs`

The dictionary forwards its events to the editor that owns the cards.
The Tag, Example, Image and Video dictionaries do the same.
The templates are shared by both card kinds.
So the handling belongs to the one editor above them rather than to a copy inside each.
