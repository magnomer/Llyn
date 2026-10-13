# Image

This folder holds the picture row in its two forms: the read-only card row and the writing row.
The writing row is merged by both the entry editor and the situation editor, and each fills the named parts.

## `PDisplayImage.xaml`

`Display.Card.Picture` draws the pictures a card carries, in the frame the writing side gives them.
The look sheet folds the panel away while the card carries no picture.
`Display.Card.PictureLine` holds one picture in the lazy loader, which turns the engine's row into a picture of its own.
The line fill hands that picture to the loader and draws its preview once it is decoded.

## `PImageTemplate.xaml`

`Theme.Image.Preview` is the frame around a preview, and the fill hides it while there is no picture to draw.
`Theme.Image.Row` holds the preview, the location field with its browse button, and the remove handle in the gutter.
The preview sits in the lazy loader, so a picture far below the view is not decoded yet.
The handle panel starts hidden and shows while the pointer or the focus is inside the list.
Typing in the location leaves through whichever editor's text handler hears it bubble.

## Stamp

Commit: `c2f335c331fe`
- `PDisplayImage.xaml` `c7d8b4c8e5bffbd0`
- `PImageTemplate.xaml` `d821aeb850b70174`
