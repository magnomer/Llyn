# PDisplayCollocation.xaml

## `ResourceDictionary`

The collocation card as the reading view draws it.
Its body opens with the expression and then the meaning, where a meaning card carries the meaning alone.
Everything else it shares with the meaning card, down to the reserved gutter.

## Inline notes

### `<Border Style="{StaticResource Theme.Card.Position}">`

The badge is the plain theme badge, since the reading view never opens a position.

### `<TextBlock x:Name="PCardTitle" Grid.Column="1" Style="{DynamicResource Display.Card.Title}" />`

The title, badge number and body texts are filled by [PLeaf](../../../Llyn.UIDeportment/Display/Template/PLeaf.comment.md).
A card without a title shows its kind in `PCardCaption` instead.
