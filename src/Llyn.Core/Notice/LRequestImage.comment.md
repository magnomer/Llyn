# LRequestImage.cs

The field requests key `LRequestKey` by type and row id, so a deferred edit replaces only its own row.
The image requests, shaped like the situation requests.
An image row is added blank and its location typed or chosen afterwards.
So the engine keeps a blank image row in the draft and commit skips it.

## `public sealed record LRequestImageAddition(`

Adds a new image row with `LRequestValue` as its location, empty for a row still to be filled.

## `public sealed record LRequestImagePick(`

Links an existing image to the card at `LRequestPosition`, copying the stored row under its positive id.

## `public sealed record LRequestImageRemoval(long LRequestDraftId, long LRequestCardId, long LRequestImageId)`

Unlinks one image from the card.

## `public sealed record LRequestImageShift(`

Moves one image row to `LRequestPosition` inside its card.

## `public sealed record LRequestImageLocation(long LRequestDraftId, long LRequestImageId, LStateValue LRequestValue)`

Replaces the location of the image, wherever the draft holds it.
