# TInterfaceFont.cs
Hash: `a1da97247634d998`

## `internal static class TInterfaceFont`

The relays for a language pack's typography, from the engine font to the one Conduct font rule.
Each relay is transparent and carries no test logic of its own.

## `internal static LFont TFontCreate(string family, double size)`

Builds an engine font for a fake settings port to answer, so a test never constructs a Core record.

## `internal static LFont TFontCreate(string family, double size, string style)`

The same engine font with a slant word, so a test sees what Core makes of the pack's word.

## `internal static CFont TFontRead(LSettingsPort settings, string language, CFontRole role)`

Relays the one font rule over the settings port a test hands in.
