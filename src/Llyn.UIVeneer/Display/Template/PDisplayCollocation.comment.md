# PDisplayCollocation.xaml
Hash: `3319fa918b8df9eb`

## `ResourceDictionary`

The collocation row as the reading view draws it, under its home meaning and in the Other list.
It also holds the "see" line a meaning shows for a collocation homed under another meaning.
A closed row is one line: hinge, phrase, the meaning cut at the line end, then the filled markers.
An open row adds the reading card's own body beneath, so every filled field shows as on a card.
All values and visibilities are set by [PLeaf](../../../Llyn.UIDeportment/Display/Template/PLeaf.comment.md).

## Inline notes

### `<Style x:Key="Display.Collocation.Marker" TargetType="TextBlock">`

The muted look the example and "also" markers share, collapsed until the row fill shows a filled one.

### `<DataTemplate x:Key="Display.Collocation.Row">`

The header line and the body are two rows of one grid, so the body starts under the phrase.

### `<ToggleButton x:Name="PCardHinge" Margin="0,0,6,0" HorizontalAlignment="Left" VerticalAlignment="Top" Style="{DynamicResource Theme.Card.Hinge}" />`

The row's chevron is the card hinge, moved to the left edge.
The reading driver hears its click as it bubbles from the list, the same as a card's hinge.
It is named like a card's so one fold painter checks it and hides it on an unstored row.

### `<TextBlock x:Name="PCardMeaning" Grid.Column="2" Margin="10,3,0,0" VerticalAlignment="Top" Style="{DynamicResource Display.Card.Definition}" TextTrimming="CharacterEllipsis" />`

The meaning takes the room left between the phrase and the markers.
The row fill turns wrapping off while the row is closed, so the trimming cuts it at the line end.

### `<ItemsControl x:Name="PCardFlag" Grid.Column="3" VerticalAlignment="Center" Visibility="Collapsed">`

One ensign per translation language, in a single horizontal line.

### `<Image x:Name="PCardEnsign" Width="18" Height="13" Margin="8,0,0,0" VerticalAlignment="Center" Stretch="Uniform" />`

Sized like the translation chip's flag, so a row's ensigns match the open body's chips.

### `<Grid x:Name="PCardBody" Grid.Row="1" Margin="32,6,0,4">`

The body is named so the fold painter collapses it while the row is closed.
Its left margin lines the fields up under the phrase rather than under the hinge.

### `<ContentControl x:Name="PCardContents" ContentTemplate="{DynamicResource Display.Card.Body}" Focusable="False" />`

The reading card's own body template, filled by the same body fill, so no field is drawn twice.

### `<DataTemplate x:Key="Display.Collocation.See">`

One "see" line, a bare button so the reading driver hears its click as it bubbles from the meaning list.
The button carries the line's item, which names the collocation whose row the click opens.

### `<TextBlock x:Name="PCardSeeLabel" Background="Transparent" FontSize="{DynamicResource Theme.Card.DefinitionSize}" Foreground="{DynamicResource Theme.Muted}" />`

The line stays muted at the definition's size, and the hand cursor marks it as a link.
Its background is transparent rather than empty, so the whole line takes the pointer, not only the glyphs.
