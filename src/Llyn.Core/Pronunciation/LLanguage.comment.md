# LLanguage.cs

## `public sealed record LLanguage(`

A loaded language pack: the language's name and the source definitions declared for it.
Loaded from `languages//source.json`.
The engine holds no language-specific facts of its own.
Everything language-specific arrives through this record.

**Parameters**

- `LLanguageName` — The language's name, matching its folder under `languages/`.
- `LLanguageFlag` — The pack's flag as an ISO 3166-1 alpha-2 country code (for example `gb`).
  It is `null` when the pack declares none.
  The image itself is not shipped.
  The engine downloads the matching flag from the flag-icons set on demand.
  It caches the flag in the workspace.
- `LLanguageSources` — The source definitions declared for the language.
