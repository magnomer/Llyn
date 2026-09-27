# CFont.cs

## `public sealed record CFont(string? CFontFamily, double? CFontSize, string? CFontStyle);`

The font a language's pack sets for one role.

**Parameters**

- `CFontFamily`: the family name, null to keep the theme's.
- `CFontSize`: the size, null to keep the theme's.
- `CFontStyle`: the style name, null to keep the theme's.
