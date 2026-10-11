# CSPressPaper.cs
Hash: `5792698814ac039b`

## `public sealed record CSPressPaper(string CSPressPaperKey, double CSPressPaperWidth, double CSPressPaperHeight, bool CSPressPaperChosen)`

One sheet in the print window's paper list, in the order `CSPressPaperSelect` indexes.

**Parameters**

- `CSPressPaperKey`: the wording key of the sheet's standard name, or `Print.PaperCustom` for an unnamed form.
- `CSPressPaperWidth`: the upright width in inches, which the custom wording shows.
- `CSPressPaperHeight`: the upright height in inches, which the custom wording shows.
- `CSPressPaperChosen`: whether this sheet is the one the setting uses.
