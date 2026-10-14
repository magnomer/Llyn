# Album

This folder holds the picture and video rows of a card or a Situation.
It stands apart from Card, because the editor and the playwright both build these gates.
A Situation's rows have no card.

## `CImage` and `CVideo`

The picture and video rows of the held draft's cards or of its Situation.
They keep no state, so the editor and the playwright each build them fresh over their desk.
Typed locations and spans are deferred like every typed field.
Add under a Situation's rows passes a null card, which Conduct maps to the engine's card 0.
So the driver never sends a magic id.
The row lands after the rows held.
A chosen file is sent at once rather than deferred, and a cancelled dialog hands null and sends nothing.
Remove needs no card, since the engine finds the card holding the row.

## Stamp

Commit: `c6e60141a172`
- `CImage.cs` `3945020400f272c3`
- `CVideo.cs` `9a6acb191ec7945e`
