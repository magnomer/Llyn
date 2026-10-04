# CImage.cs
Hash: `19ccfc7dfe8ae5a4`

## `public sealed class CImage`

The image rows of the held draft's cards, as the user types into them.
It keeps no state, so the editor and the playwright each build it fresh over their desk.

## `public void CImageLocationSet(long imageId, string location)`

The user typed an image's location, deferred like every typed field.

## `public void CImageAdd(long? cardId)`

The user pressed add under a card's pictures, or under a Situation's with a null card.
Null means no card owns the row.
Conduct maps null to the engine's card 0, so the driver never sends a magic id.
The row lands after the rows held, and the driver hands no count.

## `public void CImageRemove(long imageId)`

The user dropped a picture row, and the engine finds the card holding it.

## `public void CImageFileSet(long imageId, string? file)`

The user answered the file dialog of a picture row.
A chosen file is sent at once rather than deferred.
A cancelled dialog hands null, and nothing is sent.
