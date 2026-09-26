# PDisplayImage.xaml

## `ResourceDictionary`

The picture row of a read-only card, from its panel down to the single frame.

## `<Style x:Key="Display.Card.Picture" TargetType="ItemsControl">`

Pictures a card carries are drawn here, in the frame the writing side gives them.
The look sheet folds the panel away while the card carries no picture.

## `<DataTemplate x:Key="Display.Card.PictureLine">`

One picture in the lazy loader, which turns the engine's row into a picture of its own.
The line fill hands that picture to the loader and draws its preview once it is decoded.
