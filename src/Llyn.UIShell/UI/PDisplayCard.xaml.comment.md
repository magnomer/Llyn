# PDisplayCard.xaml

## `ResourceDictionary`

The meaning card and the collocation card as the reading view draws them.
They live apart from `PDisplay.xaml` because the card is its own shape, not part of the entry page around it.
Every `Theme.` reference here is dynamic, since a loose dictionary is parsed before the theme is in reach.

## Inline notes

### `<DataTemplate x:Key="Display.Card.Body">`

Situations, translations, examples and tags read the same on both cards.
So the shared tail is one template, handed to each card through a content control.
A meaning card adds its definition above it, and a collocation card its expression and meaning.

### `<Style x:Key="Display.Card.Definition" TargetType="TextBlock">`

The definition is the sentence the card exists for, so it is set at the weight of a title, not of a field value.
No label stands over it.
A label naming what a reader can already see only pushes the reading down the card.

### `<FontFamily x:Key="Display.Card.ExampleFamily">`

Examples are drawn in the typography the language pack declares for them.
These two keys hold what the reading view sets for the entry now shown.
The values written here are the fallback for a pack that declares none.

### `<DataTemplate x:Key="Display.Card.TranslationChip">`

A translation is a word in another language, so it is drawn as a chip and not as a line of prose.
Tags take an outlined chip instead, so the two rows of chips are never read as one kind.

### `<Style x:Key="Display.Card.Example" TargetType="Border">`

Examples are indented behind a rule, because they are evidence for the definition rather than a field of their own.
