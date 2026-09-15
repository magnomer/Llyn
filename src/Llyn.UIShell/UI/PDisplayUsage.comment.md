# PDisplayUsage.xaml

## `ResourceDictionary`

The shapes of the page around the card: the incoming rows above it and the stamps beneath it.
Nothing here answers an event, so the dictionary is loose and merged beside the card shapes.

## `<Style x:Key="Theme.Usage.Row" TargetType="Button">`

One entry whose sentence mentions the entry being read, drawn as a card that lights under the pointer.
It is a button, because the row opens the entry it names.

## `<Style x:Key="Theme.Usage.Detail" TargetType="TextBlock">`

The sentence under the headword, trimmed to one line.
It collapses when the row has no sentence to show, so the headword stands alone.

## `<Style x:Key="Theme.Stamp.Label" TargetType="TextBlock">`

The label of a stamp row, muted and small, because the stamps are a footnote.
`Theme.Stamp.Time` dresses the time beside it in the same face.
