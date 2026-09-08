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

### `FontFamily="{DynamicResource Theme.Card.ExampleFamily}"`

Examples are drawn in the typography the language pack declares for them.
Both modes read the same two keys, and each sets them on itself for the entry it shows.
The fallback for a pack that declares none is written once, with the rest of a card's measurements.

### `<DataTemplate x:Key="Display.Card.SituationChip">`

A situation names where or when the entry is used, so it is a chip rather than a line of prose.
It is the same chip the writing side is typed into, so the field does not change shape between modes.

### `<DataTemplate x:Key="Display.Card.TranslationChip">`

A translation is a word in another language, so it is drawn as a chip and not as a line of prose.
Tags take an outlined chip instead, so the two rows of chips are never read as one kind.

### `<Style x:Key="Display.Card.Example" TargetType="Border">`

Examples are indented behind a rule, because they are evidence for the definition rather than a field of their own.

### `<Style x:Key="Display.Card.Picture" TargetType="ItemsControl">`

Pictures a card carries are drawn here, in the frame the writing side gives them.
A card whose picture only appeared while it was being written would be a card the reader was never shown.

### `<ColumnDefinition Style="{DynamicResource Theme.Card.Gutter}" />`

The reading view reserves the strip the writing view puts its handles in, and leaves it empty.
A line must break at the same word in both modes, and it only can if it is given the same width in both.
