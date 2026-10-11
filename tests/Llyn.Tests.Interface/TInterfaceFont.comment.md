# TInterfaceFont.cs
Hash: `d97fc8eec98e7420`

## `internal static class TInterfaceFont`

The relays for a language pack's typography, from the engine font to the one Conduct font rule.
Each relay is transparent and carries no test logic of its own.

## `internal static LFont TFontCreate(string family, double size)`

Builds an engine font for a fake settings port to answer, so a test never constructs a Core record.

## `internal static LFont TFontCreate(string family, double size, string style)`

The same engine font with a slant word, so a test sees what Core makes of the pack's word.

## `internal static LFont TFontEmptyCreate()`

Builds the engine font that stands for no family and no size, as a blank language answers.

## `internal static CFont TFontCreate(string? family, double? size, CFontSlant slant)`

Builds the expected Conduct font, so a test never constructs a Conduct record itself.

## `internal static CFontRole TFontRoleRead(LFontRole role)`

Casts an engine font role to the Conduct role of the same name.
`TFont` pins that the two enums stay in step.

## `internal static CFont TFontRead(LSettingsPort settings, string language, CFontRole role)`

Relays the one font rule over the settings port a test hands in.
