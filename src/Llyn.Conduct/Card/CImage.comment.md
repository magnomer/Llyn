# CImage.cs

## `public sealed class CImage`

The image rows of the held draft's cards, as the user types into them.
It keeps no state, so the editor builds it fresh over its desk.

## `public void CImageLocationSet(long imageId, string location)`

The user typed an image's location, deferred like every typed field.
