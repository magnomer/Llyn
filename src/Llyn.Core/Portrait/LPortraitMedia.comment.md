# LPortraitMedia.cs

## `public sealed record LPortraitMedia`

One picture or video plate on a card.
Both kinds share a shape because both are drawn as a bordered preview.

**Parameters**

- `LPortraitMediaLocation` - the file path or address the plate stands for.
- `LPortraitMediaSpan` - the played span of a video, empty for a picture.
- `LPortraitMediaMoving` - whether this plate is a video rather than a picture.

## `public static IReadOnlyList<LPortraitMedia> LPortraitMediaCreate(IReadOnlyList<LImageDraft> images, string mark)`

Turns picture drafts into plates, skipping any whose location was never written.
The display draws no empty plate, so the likeness carries none either.

## `public static IReadOnlyList<LPortraitMedia> LPortraitMediaCreate(IReadOnlyList<LVideoDraft> videos, string mark)`

Turns video drafts into moving plates, each carrying its played span.
The same skip applies: a video with no location is not a plate.
