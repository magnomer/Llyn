# LFont.cs
Hash: `13c17918045f5df8`

## `public sealed record LFont(string? LFontFamily, double LFontSize, string? LFontStyle = null)`

The typography a language pack declares for showing its own words.
One record carries one role, named by [LFontRole](LFontRole.comment.md).
The headword is drawn with the headword record wherever it is shown.
The editor and the reading view are given the same record, so they never drift apart.

**Parameters**

- `LFontFamily`: the font family the pack declares, such as `Yu Gothic UI`.
  It is `null` when the pack declares none.
  The theme's own family then stands.
- `LFontSize`: the point size the pack declares for the role.
  It is `0` when the pack declares none.
  The theme's own size then stands.
- `LFontStyle`: the slant the pack declares for the role, `italic` or `oblique`.
  It is `null` when the pack declares none, and the text stands upright.
  The record lowers the pack's word, so `Italic` arrives as `italic`.
  Any other word arrives `null`, so every surface maps only the two slants.
  English glosses are set in italic, as a translation under a sentence is by convention.

## `public string? LFontFace`

The declared family, or `null` when the pack declares none or a blank one.
No surface then builds a font family from nothing.

## `public double? LFontSized`

The declared size, or `null` unless it is finite and positive.
Every surface then sets only a usable size, and the theme's own size stands otherwise.
