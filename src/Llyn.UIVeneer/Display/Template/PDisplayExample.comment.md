# PDisplayExample.xaml

## `ResourceDictionary`

The example block of a read-only card, from its indent down to the single line.

## Inline notes

### `<Style x:Key="Display.Card.Example" TargetType="Border">`

Examples are indented as a block, because they are evidence for the definition rather than a field of their own.
Each line opens with a dot, so a reader counts examples without reading them.

### `<TextBlock x:Name="PExampleFrame" Grid.Column="1" Style="{StaticResource Display.Card.ExampleFrame}" />`

The frame is drawn in the interface face and the accent colour the example beside it never takes.
A reader tells at a glance what the card writes and what the card only marks.
It stands beside the example rather than inside it, as it does on the writing side.
A row carrying no frame drops it whole.
Its example begins where an unframed example begins on the writing side.
A line set the same way in both views does not move.
A reader who begins to write finds it unchanged.

### `<local:PMention x:Name="PExampleText" />`

The sentence is drawn by the control that knows its words, so a reader can click one.
Its text, language and Mentions are set by [PLeaf](../../../Llyn.UIDeportment/Display/Template/PLeaf.comment.md), which keeps the frame out of the sentence.
Its language is the Example's own, because the sentence may be written in a language the entry is not.
Examples are drawn in the typography the language pack declares for them.
Both modes read the same two keys, and each sets them on itself for the entry it shows.
The fallback for a pack that declares none is written once, with the rest of a card's measurements.

### `<DataTemplate x:Key="Display.Card.ExampleGloss">`

One Gloss under the sentence: the flag of its language and the text.
The unknown mark stands in when the text is not known.
It takes the family, size and slant the entry's language pack declares for a Gloss.
It uses the muted colour, as the writing side sets the same row.
The flag and the globe come from the shared Gloss theme, so both modes draw the same language mark.
Its top margin equals the bottom margin the writing side's sentence field carries.
So a Gloss sits the same distance under its sentence in both modes.

### `<ItemsControl x:Name="PExampleGloss" ItemTemplate="{StaticResource Display.Card.ExampleGloss}" Style="{DynamicResource Theme.Gloss.Line}" />`

The Glosses stand under the sentence, aligned with its first character rather than with the frame.
Its parts carry the editor's Gloss names, so the editor's row fill sets them.
The list folds away when the Example carries none.

### `<Border Grid.Column="3" Margin="16,0,0,0">`

The byline of the cited Source stands at the right end of the example line, as dictionaries print quotations.
An example citing nothing collapses the column, so an uncited sentence runs to the edge.
Its font, size and baseline drop are copied from the sentence element when the line is filled.
So the byline sits on the sentence's baseline whatever font the language pack gave the sentence.
