# LPressImage.cs
Hash: `3d69582d14ebd9f9`

## `public sealed record LPressImage(byte[] LPressImagePng)`

One drawn page, carried as encoded bytes so no drawing framework crosses into the logic.
The pixel size is not repeated beside the bytes.
A view lays a page out from the proof's paper size and reads pixels only from the PNG.

**Parameters**

- `LPressImagePng`: the page encoded as a PNG picture.
