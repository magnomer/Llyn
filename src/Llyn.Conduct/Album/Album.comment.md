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
File open asks the desk's envoy for a file, so every face browses through one path.
It names the dialog by a title key and the suffixes of the kind.
Remove takes the same flag and card as Add.
So the engine addresses the row by kind, card and id.
It no longer searches the draft for the card.

## Stamp

Commit: `9377e657b395`
- `CImage.cs` `2f2a7d6612e07f30`
- `CVideo.cs` `7b4235348da41434`
