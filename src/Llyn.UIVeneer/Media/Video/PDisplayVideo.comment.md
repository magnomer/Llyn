# PDisplayVideo.xaml

## `ResourceDictionary`

The video row of a read-only card, from its panel down to the single screen.

## Inline notes

### `<DataTemplate x:Key="Display.Card.VideoLine">`

A clip is played by the same screen the writing side embeds, in the same frame a picture takes.
The screen reads the row's location through the state converter, since the row holds the engine's value.
Its volume is read off the shared catalog, so every screen in the app answers one control.
