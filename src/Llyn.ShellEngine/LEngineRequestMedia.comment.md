# LEngineRequestMedia.cs

## `public sealed partial class LEngine`

The image and video rows of a card or a Situation.
A row is added with whatever location the form already has, often none, and filled afterwards.
So a blank row is held and named, and the commit leaves it out.
The list edits are written once over a bare list, so a card and a Situation share every rule.

## `private LSituation? LEngineSituationDispatch(LSituation situation, LRequest request)`

Routes a media request to the image list or the video list of a held Situation.
Answers null for any other request, so the draft switch goes on to its own kinds.

## `private IReadOnlyList<LImageDraft>? LEngineImageApply(IReadOnlyList<LImageDraft> images, LRequest request)`

The image list after one request, or null when the request is not an image one.
An addition is a new row with the location sent and a minted id, at the place asked for.
A pick copies the stored image under its own id, unless the list already holds it.
A location change on a row the list does not hold is refused as an item.

## `private LEntryDraft LEngineImageApply(LEntryDraft content, long cardId, LRequest request)`

The same list edit applied to the image list of one card.

## `private LImageDraft LEngineImageRead(long id)`

The stored image as a draft row, refused when no image carries the id.

## `private static LEntryDraft LEngineImageChange(LEntryDraft content, LRequestImageLocation request)`

Relocates the image in every card holding it, and refuses when none does.

## `private IReadOnlyList<LVideoDraft>? LEngineVideoApply(IReadOnlyList<LVideoDraft> videos, LRequest request)`

The video list after one request, or null when the request is not a video one.
An addition is a new row with the location sent, no span and a minted id.
A pick copies the stored video under its own id, unless the list already holds it.
The location and the span change through the same routine with a different lambda.

## `private LEntryDraft LEngineVideoApply(LEntryDraft content, long cardId, LRequest request)`

The same list edit applied to the video list of one card.

## `private LVideoDraft LEngineVideoRead(long id)`

The stored video as a draft row, refused when no video carries the id.

## `private static LEntryDraft LEngineVideoChange(`

Applies one change to the video in every card holding it, and refuses when none does.
The location and the span come through the same routine with a different lambda.
