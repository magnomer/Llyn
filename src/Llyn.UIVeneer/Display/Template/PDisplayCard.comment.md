# PDisplayCard.xaml

## `ResourceDictionary`

The shared parts of a read-only card, and the dictionary that gathers the rest.
They live apart from `PDisplay.xaml` because the card is its own shape, not part of the entry page around it.
Every `Theme.` reference here is dynamic, since a loose dictionary is parsed before the theme is in reach.
The header text styles stand here because both card shapes read them.
Every value a card shows is set by [PLeaf](../../../Llyn.UIDeportment/Display/Template/PLeaf.comment.md), which finds each named part.
Each row of a card body is written in its own dictionary and merged back in.

## Inline notes

### `<ResourceDictionary.MergedDictionaries>`

A card body is a stack of independent rows, so each row is written where it can be read alone.
The merge order is the reading order of a card, so the file list doubles as the card's outline.

### `<ResourceDictionary Source="/Llyn.UIVeneer;component/Media/Image/PDisplayImage.xaml" />`

The picture and video rows live with the other media markup, and the leaf fills their lines.

### `<DataTemplate x:Key="Display.Card.Body">`

Situations, translations, examples and tags read the same on both cards.
So the shared tail is one template, handed to each card through a content control.
Each row is named, so the fill sets its items and folds it away when it has none.
A meaning card adds its definition above it, and a collocation card its expression and meaning.

### `<Style x:Key="Display.Card.Definition" TargetType="TextBlock">`

The definition is the sentence the card exists for.
It is set at the weight of a title, not of a field value.
No label stands over it.
A label naming what a reader can already see only pushes the reading down the card.
