# PTranslationTemplate.xaml

## `Theme.Translation.Field`

The Translation field: one bordered surface holding a wrapping run of linked headwords with the open entry after them.
The height is not fixed.
The run wraps and the surface grows with it.
So a card that links many entries shows all of them instead of hiding them behind a scroll.

The surface is the field, not the entry.
So the border reacts to focus anywhere within it.
A click on empty space inside it reaches the caret.

## `Theme.Translation.Chip`

One committed link: the target's flag, its headword, its language and the button that closes it.
The language rides beside the headword because a link crosses languages.

## `Theme.Translation.Entry`

The caret at the end of the run.
It shares the ordinary input style, so its placeholder behaves as every other field's does.
But it carries no border of its own.
The surface around the whole field is the border.

## `Theme.Translation.Candidate`

One row of the dropdown offered when the typed word does not name one entry outright.
It reads as the other headword lists do, so the same word means the same thing everywhere.
A create row is marked with a leading plus, because it makes an entry rather than pointing at one.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

## `PTranslationTemplate.xaml.cs`

The dictionary forwards its events to the editor that owns the cards.
The Example, Situation, Image, Video and Tag dictionaries do the same.
The templates are shared by both card kinds.
So the handling belongs to the one editor above them rather than to a copy inside each.
