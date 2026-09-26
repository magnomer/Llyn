# PDisplayMeaning.xaml

## `ResourceDictionary`

The meaning card as the reading view draws it.
Its header carries the position and the title, and its body opens with the definition.
The rows under the definition are the shared card body.

## Inline notes

### `<ColumnDefinition Style="{DynamicResource Theme.Card.Gutter}" />`

The reading view reserves the strip the writing view puts its handles in, and leaves it empty.
A line must break at the same word in both modes.
It only can if it is given the same width in both.

### `<Border Style="{StaticResource Theme.Card.Position}">`

The badge is the plain theme badge, since the reading view never opens a position.

### `<TextBlock x:Name="PCardTitle" Grid.Column="1" Style="{DynamicResource Display.Card.Title}" />`

The title, badge number and body texts are filled by [PLeaf](../../../Llyn.UIDeportment/Display/Template/PLeaf.comment.md).
A card without a title shows its kind in `PCardCaption` instead.
