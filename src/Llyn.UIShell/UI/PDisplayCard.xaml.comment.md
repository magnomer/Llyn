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
The reading chip is a button, so the panel holding the situation it names is one click away.
The click is caught by the list the card sits in, because this dictionary is shared and holds no window of its own.
The chip carries no id of its own, because what was clicked already stands for the situation it was drawn from.
Translations and tags are read the same way, so no chip on a card is a dead end.

### `<Style x:Key="Display.Card.SituationLink" TargetType="Button">`

Wears the chip border of the writing side, so a chip that leads somewhere is still read as the same chip.
The edge lights on hover and the chip dims while pressed, which is the only sign the reading side gives that a chip is a link.
The translation and tag chips take the same treatment over their own shells, so the three read as one kind of link.

### `<Style x:Key="Display.Card.TranslationLink" TargetType="Button">`

The translation pellet carries no edge of its own, so hovering draws one rather than repainting the fill.
Repainting would put the chip in the colour a selected chip wears elsewhere.
The edge is there in every state and only its colour changes, because an edge appearing on hover would widen the chip and shove the row it sits in.

### `<DataTemplate x:Key="Display.Card.TranslationChip">`

A translation is a word in another language, so it is drawn as a chip and not as a line of prose.
Tags take an outlined chip instead, so the two rows of chips are never read as one kind.
A translation chip opens the Entry it points at, and a tag chip opens the taxonomy browsing by that tag.

### `<Style x:Key="Display.Card.TagLink" TargetType="Button">`

A tag is its own text and holds no id, so the chip is what the taxonomy is later asked to browse by.

### `<Style x:Key="Display.Card.Example" TargetType="Border">`

Examples are indented as a block, because they are evidence for the definition rather than a field of their own.
Each line opens with a dot, so a reader counts examples without reading them.

### `<TextBlock x:Name="PExampleFrame">`

The frame is drawn in the interface face and the accent colour the example beside it never takes.
A reader tells at a glance what the card writes and what the card only marks.
It stands beside the example rather than inside it, as it does on the writing side.
A row carrying no frame drops it whole, so its example begins where an unframed example begins on the writing side.
A line set the same way in both views is a line that does not move when a reader begins to write.

### `<Style x:Key="Display.Card.Picture" TargetType="ItemsControl">`

Pictures a card carries are drawn here, in the frame the writing side gives them.
A card whose picture only appeared while it was being written would be a card the reader was never shown.

### `<ColumnDefinition Style="{DynamicResource Theme.Card.Gutter}" />`

The reading view reserves the strip the writing view puts its handles in, and leaves it empty.
A line must break at the same word in both modes, and it only can if it is given the same width in both.
