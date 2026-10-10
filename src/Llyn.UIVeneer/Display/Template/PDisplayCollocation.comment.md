# PDisplayCollocation.xaml
Hash: `5fcad40040b19c5e`

## `ResourceDictionary`

The collocation card as the reading view draws it.
Its body opens with the expression and then the meaning, where a meaning card carries the meaning alone.
Everything else it shares with the meaning card, down to the reserved gutter.

## Inline notes

### `<DataTemplate x:Key="Display.Collocation.Card">`

The header row is fixed at the height the writing card's header takes.
So the body starts at the same depth in both modes.

### `<Border Style="{StaticResource Theme.Card.Position}">`

The badge is the plain theme badge, since the reading view never opens a position.

### `<TextBlock x:Name="PCardTitle" Grid.Column="1" Style="{DynamicResource Display.Card.Title}" />`

The title, badge number and body texts are filled by [PLeaf](../../../Llyn.UIDeportment/Display/Template/PLeaf.comment.md).
An unfolded card without a title shows its kind in `PCardCaption` instead.

### `<Border x:Name="PCardHeader" Style="{DynamicResource Theme.Card.Header}">`

The header is named so the card painter can close its corners while the card is folded.
The writing card's header carries the same name, so one painter serves both modes.

### `<TextBlock x:Name="PCardPeek" Grid.Column="1" Style="{DynamicResource Theme.Card.Peek}" />`

The card's expression, shown in the title's place while the card is folded and untitled.
The kind caption gives way to it then.

### `<ToggleButton x:Name="PCardHinge" Grid.Column="0" Grid.ColumnSpan="2" Style="{DynamicResource Theme.Card.Hinge}" />`

The hinge spans both columns and sits at the right edge, vertically centred.
It stands where the writing card's hinge stands, with no eraser to its left.
The reading driver hears its click as it bubbles from the card list.
The reading driver finds it as the only toggle carrying a reading card.

### `<Grid x:Name="PCardBody" Grid.Row="1" Style="{DynamicResource Theme.Card.Body}">`

The body is named so the card painter can collapse it while the card is folded.
