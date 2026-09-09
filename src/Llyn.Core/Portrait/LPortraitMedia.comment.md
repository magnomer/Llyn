# LPortraitMedia.cs

## `public sealed record LPortraitMedia`

One picture or video plate on a card.
Both kinds share a shape because both are drawn as a bordered preview.

**Parameters**

- `LPortraitMediaLocation` - the file path or address the plate stands for.
- `LPortraitMediaSpan` - the played span of a video, empty for a picture.
- `LPortraitMediaMoving` - whether this plate is a video rather than a picture.
