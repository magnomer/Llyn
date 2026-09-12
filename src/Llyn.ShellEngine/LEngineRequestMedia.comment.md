# LEngineRequestMedia.cs

## `public sealed partial class LEngine`

The image and video rows of a card.
A row is added with whatever location the form already has, often none, and filled afterwards.
So a blank row is held and named, and the commit leaves it out.

## `private LEntryDraft LEngineImageAdd(LEntryDraft content, LRequestImageAddition request)`

A new image row with the location sent and a minted id, at the place asked for.

## `private LEntryDraft LEngineImageInsert(LEntryDraft content, LRequestImagePick request)`

Copies the stored image under its own id into the card, unless the card already holds it.

## `private static LEntryDraft LEngineImageChange(LEntryDraft content, LRequestImageLocation request)`

Relocates the image in every card holding it, and refuses when none does.

## `private LEntryDraft LEngineVideoAdd(LEntryDraft content, LRequestVideoAddition request)`

A new video row with the location sent, no span and a minted id, at the place asked for.

## `private LEntryDraft LEngineVideoInsert(LEntryDraft content, LRequestVideoPick request)`

Copies the stored video under its own id into the card, unless the card already holds it.

## `private static LEntryDraft LEngineVideoChange(`

Applies one change to the video in every card holding it, and refuses when none does.
The location and the span come through the same routine with a different lambda.
