# LPortraitMedia.cs

## `public sealed record LPortraitMedia`

One picture or video plate on a card.
Both kinds share a shape because both are drawn as a bordered preview.

**Parameters**

- `LPortraitMediaLocation` - the file path or address the plate stands for.
- `LPortraitMediaSpan` - the played span of a video, or the caption of a stored picture.
- `LPortraitMediaMoving` - whether this plate is a video rather than a picture.

## `public static IReadOnlyList<LPortraitMedia> LPortraitMediaCreate(IReadOnlyList<LImageDraft> images, string mark)`

Turns picture drafts into plates, skipping any whose location was never written.
The display draws no empty plate, so the likeness carries none either.

## `public static LPortraitMedia LPortraitMediaCreate(byte[] data, string caption)`

Turns picture bytes held in the workspace into a plate, such as a fetched character form.
The location is a data address, so a writer that embeds files embeds these too.
The media type is read off the leading bytes, and anything not PNG or GIF is called JPEG.

## `public static IReadOnlyList<LPortraitMedia> LPortraitMediaCreate(IReadOnlyList<LVideoDraft> videos, string mark)`

Turns video drafts into moving plates, each carrying its played span.
The same skip applies: a video with no location is not a plate.
