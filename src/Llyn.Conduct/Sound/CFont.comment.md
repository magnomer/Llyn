# CFont.cs
Hash: `505e7e889f713f67`

## `public sealed record CFont(string? CFontFamily, double? CFontSize, CFontSlant CFontStyle)`

The font a language's pack sets for one role.

**Parameters**

- `CFontFamily`: the family name, never blank, null to keep the theme's.
- `CFontSize`: the size, always finite and positive, null to keep the theme's.
- `CFontStyle`: the slant, `CFontSlantTheme` to keep the theme's.

## `internal static CFont CFontRead(LSettingsPort settings, string language, CFontRole role)`

The one font rule, shared by every area that reads a pack's typography in its own language.
The engine record drops a blank family and an unusable size, so a driver sets only usable values.
A blank language passes on unread, since the engine answers it a font with nothing set.
A refused read is not caught here and reaches the caller.

## `private static CFontSlant CFontSlantRead(string? style)`

The slant an engine style name stands for.
The engine record already lowers the name, so the match needs no case fold.
Any other name keeps the theme's slant, so a driver switches on a closed set.

## `private static LFontRole CFontRoleRead(CFontRole role)`

The engine role of the same name, switched name by name.
An unknown role throws, so a role added on one side alone fails loudly.
