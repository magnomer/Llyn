# PImagePending.cs

## `public interface PImagePending`

What a row holding a picture promises the element that draws it.
The row holds the bytes or the address but decodes nothing until told the element is in view.
The card image row and the script picture both keep this promise.
So one lazy element serves every picture the reading view draws.
It is public because the public lazy element holds a row of this type.

## `void PImageLoad()`

Loads the picture now, because the element drawing it has come into view.
A row already loaded does nothing, so the element may call it as often as it is shown.
