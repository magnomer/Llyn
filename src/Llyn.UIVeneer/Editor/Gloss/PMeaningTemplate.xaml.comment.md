# PMeaningTemplate.xaml.cs

## `public partial class PMeaningTemplate : ResourceDictionary`

The meaning card as a template.
A template lives in a dictionary rather than in the panel it fills.
So the panel keeps its own layout.
The events a card raises belong to the panel all the same.
This class exists only to hand them back to it.

## Inline notes

### `<Grid Grid.Row="1" Style="{DynamicResource Theme.Card.Body}">`

The fields stand in the order the reading view draws them: definition, situation, translations, examples, tags.
A writer fills a card in the order a reader will meet it.
Nothing has to be moved in the head.
No field is labelled, because each is already drawn as what it is.
A label naming what a writer can read off the field only pushes the card down.
It also makes the two modes disagree.

### `<TextBox Grid.Column="1" Margin="0,0,34,0" Style="{DynamicResource Theme.Card.Title}">`

The title keeps the reading side's place.
The mark that drops the card is laid over the strip beside it.
Given a column of its own it would narrow the title.
The same title would then wrap in one mode and not the other.

### `<Border Margin="0,16,-76,0" HorizontalAlignment="Center" Style="{DynamicResource Theme.Command.Group}">`

The pair takes the icon cluster the pronunciation lookup and the audio download already carry.
An outlined pill of muted text read as one more tag chip beside the tags above it.
Phosphor image.svg and video-camera.svg, tinted through an opacity mask (see PPlayback).
The tooltip carries the label the icon drops.
The negative margin cancels the gutter column, so the cluster centres on the whole card body.

### `<Border Style="{DynamicResource Theme.Card.Position}" MouseLeftButtonDown="PCardPositionHandle">`

The badge is a field rather than a label, so the number a card carries can be written over.
Reordering by number reaches a place a drag has to scroll to.
