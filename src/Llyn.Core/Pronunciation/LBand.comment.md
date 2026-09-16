# LBand.cs

## `public sealed record LBand(string LBandName, string LBandPattern)`

One frequency band a language pack declares for a figure the shared ladder cannot grade.
A numeric figure with a word interval is graded by [LFrequency](LFrequency.comment.md) and never reaches these bands.
The pack lists bands under each frequency source in order, and the first regex matching the raw figure wins.
So a pack labels only codes and levels, such as HSK 5 or S1, and a rank of unranked.
The name must be one of the four ladder names, so a level reads the same as a graded figure.

**Parameters**

- `LBandName` — The label an entry earns when this band matches, as the pack spells it.
- `LBandPattern` — The regex the raw figure must match for this band.
