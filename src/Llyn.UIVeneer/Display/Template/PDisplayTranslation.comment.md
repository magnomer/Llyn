# PDisplayTranslation.xaml

## `ResourceDictionary`

The translation row of a read-only card, from its panel down to the single chip.

## Inline notes

### `<Style x:Key="Display.Card.TranslationLink" TargetType="Button">`

The translation pellet carries no edge of its own, so hovering draws one rather than repainting the fill.
Repainting would put the chip in the colour a selected chip wears elsewhere.
The edge is there in every state and only its colour changes.
An edge appearing on hover would widen the chip and shove its row.

### `<DataTemplate x:Key="Display.Card.TranslationChip">`

A translation is a word in another language.
It is drawn as a chip and not as a line of prose.
Tags take an outlined chip instead, so the two rows of chips are never read as one kind.
A translation chip opens the Entry it points at, and a tag chip opens the taxonomy browsing by that tag.
