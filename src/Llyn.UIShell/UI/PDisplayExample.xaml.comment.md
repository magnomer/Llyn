# PDisplayExample.xaml

## `ResourceDictionary`

The example block of a read-only card, from its indent down to the single line.

## Inline notes

### `<Style x:Key="Display.Card.Example" TargetType="Border">`

Examples are indented as a block, because they are evidence for the definition rather than a field of their own.
Each line opens with a dot, so a reader counts examples without reading them.

### `<TextBlock x:Name="PExampleFrame">`

The frame is drawn in the interface face and the accent colour the example beside it never takes.
A reader tells at a glance what the card writes and what the card only marks.
It stands beside the example rather than inside it, as it does on the writing side.
A row carrying no frame drops it whole.
Its example begins where an unframed example begins on the writing side.
A line set the same way in both views does not move.
A reader who begins to write finds it unchanged.

### `FontFamily="{DynamicResource Theme.Card.ExampleFamily}"`

Examples are drawn in the typography the language pack declares for them.
Both modes read the same two keys, and each sets them on itself for the entry it shows.
The fallback for a pack that declares none is written once, with the rest of a card's measurements.
