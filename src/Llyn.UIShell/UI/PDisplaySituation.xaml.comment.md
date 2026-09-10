# PDisplaySituation.xaml

## `ResourceDictionary`

The situation row of a read-only card, from its panel down to the single chip.

## Inline notes

### `<DataTemplate x:Key="Display.Card.SituationChip">`

A situation names where or when the entry is used.
It is a chip rather than a line of prose.
It is the same chip the writing side is typed into, so the field does not change shape between modes.
The reading chip is a button, so the panel holding the situation it names is one click away.
The click is caught by the list the card sits in.
This dictionary is shared and holds no window of its own.
The chip carries no id of its own.
What was clicked already stands for the situation it was drawn from.
Translations and tags are read the same way, so no chip on a card is a dead end.

### `<Style x:Key="Display.Card.SituationLink" TargetType="Button">`

Wears the chip border of the writing side.
A chip that leads somewhere is still read as the same chip.
The edge lights on hover and the chip dims while pressed.
That is the only sign the reading side gives that a chip is a link.
The translation and tag chips take the same treatment over their own shells.
The three read as one kind of link.

### `<ResourceDictionary.MergedDictionaries>`

The three-state reading is a binding converter, so no dynamic reference can carry it.
This row merges its own copy of a converter that keeps nothing.
