# CScheme.cs

## `public sealed record CScheme(string CSchemeName, bool CSchemeTaken)`

One scheme a transcription row's dropdown offers.

**Parameters**

- `CSchemeName`: the scheme as the language pack declares it.
- `CSchemeTaken`: whether another row of the same draft already holds the scheme.

## `public string CSchemeKey`

The localization key the scheme is labelled under.

## `public static string CSchemeKeyRead(string name)`

The localization key a scheme is labelled under.
The one owner of the key, which the scheme, the transcription draft and the glyph heading share.
A blank name keys nothing a catalog holds, so a driver prints it as it stands.
