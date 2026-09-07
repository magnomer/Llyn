# PCollocationTemplate.xaml.cs

## `public partial class PCollocationTemplate : ResourceDictionary`

The collocation card as a template — the same card shape a meaning uses, over the expression a collocation adds.
Like every template dictionary here, it only hands its events back to the panel.

## Inline notes

### `<Grid Grid.Row="1" Style="{DynamicResource Theme.Card.Body}">`

The fields stand in the order the reading view draws them: expression, meaning, situation, translations, examples, tags.
A writer fills a card in the order a reader will meet it, so nothing has to be moved in the head.
No field is labelled, for the same reason the meaning card labels none.
