# LFont.cs

## `public sealed record LFont(`

The typography a language pack declares for showing its own words.
The headword is drawn with it wherever it is shown.
The editor and the reading view are given the same record, so they never drift apart.

**Parameters**

- `LFontFamily` — The font family the pack declares, such as `Yu Gothic UI`.
  It is `null` when the pack declares none.
  The theme's own family then stands.
- `LFontSize` — The point size the pack declares for the headword.
  It is `0` when the pack declares none.
  The theme's own size then stands.
