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
The collocated flag picks the card kind, a Collocation card or a Meaning card.
The row lands after the rows held.
A chosen file is sent at once rather than deferred, and a cancelled dialog hands null and sends nothing.
Remove takes the same flag and card as Add.
So the engine addresses the row by kind, card and id.
It no longer searches the draft for the card.

## Stamp

Commit: `8c403d1df768`
- `CImage.cs` `6acac79e39de12ff`
- `CVideo.cs` `3167de9b6b50680d`
