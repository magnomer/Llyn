# LBand.cs

## `public sealed record LBand(string LBandName, double? LBandLimit, string? LBandPattern)`

One frequency band a language pack declares, such as common or rare.
The pack lists its bands in order, and the first band that matches a raw figure wins.
A band matches when `LBandLimit` is set and the raw figure parses as a decimal not above it.
A band also matches when `LBandPattern` is set and the raw figure matches it as a regex.
The engine holds no bands of its own.

**Parameters**

- `LBandName` — The label an entry earns when this band matches, as the pack spells it.
- `LBandLimit` — The highest figure this band still covers, or `null` when the band is not numeric.
- `LBandPattern` — The regex a raw figure must match for this band, or `null` when the band is numeric.
