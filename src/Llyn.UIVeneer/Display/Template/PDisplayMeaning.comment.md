# PDisplayMeaning.xaml
Hash: `601a57f55ba4a01a`

## `ResourceDictionary`

The meaning card as the reading view draws it.
Its header carries the position and the title, and its body opens with the definition.
The rows under the definition are the shared card body.

## Inline notes

### `<DataTemplate x:Key="Display.Meaning.Card">`

The header row is fixed at the height the writing card's header takes.
So the body starts at the same depth in both modes.

### `<ColumnDefinition Style="{DynamicResource Theme.Card.Gutter}" />`

The reading view reserves the strip the writing view puts its handles in, and leaves it empty.
A line must break at the same word in both modes.
It only can if it is given the same width in both.

### `<Border Style="{StaticResource Theme.Card.Position}">`

The badge is the plain theme badge, since the reading view never opens a position.

### `<TextBlock x:Name="PCardTitle" Grid.Column="1" Style="{DynamicResource Display.Card.Title}" />`

The title, badge number and body texts are filled by [PLeaf](../../../Llyn.UIDeportment/Display/Template/PLeaf.comment.md).
An unfolded card without a title shows its kind in `PCardCaption` instead.

### `<Border x:Name="PCardHeader" Style="{DynamicResource Theme.Card.Header}">`

The header is named so the card painter can close its corners while the card is folded.
The writing card's header carries the same name, so one painter serves both modes.

### `<TextBlock x:Name="PCardPeek" Grid.Column="1" Style="{DynamicResource Theme.Card.Peek}" />`

The card's definition, shown in the title's place while the card is folded and untitled.
The kind caption gives way to it then.

### `<ToggleButton x:Name="PCardHinge" Grid.Column="0" Grid.ColumnSpan="2" Style="{DynamicResource Theme.Card.Hinge}" />`

The hinge spans both columns and sits at the right edge, vertically centred.
It stands where the writing card's hinge stands, with no eraser to its left.
The reading driver hears its click as it bubbles from the card list.
The reading driver finds it as the only toggle carrying a reading card.

### `<Grid x:Name="PCardBody" Grid.Row="1" Style="{DynamicResource Theme.Card.Body}">`

The body is named so the card painter can collapse it while the card is folded.
