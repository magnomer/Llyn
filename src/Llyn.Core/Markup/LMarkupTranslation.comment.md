# LMarkupTranslation.cs

## `public sealed record LMarkupTranslation(`

A translation link as a markup file carries it, naming its target entry by natural key.
On import the target is looked up among stored entries and entries the same file creates.

**Parameters**

- `LMarkupTranslationHeadword` — The headword of the target entry.
- `LMarkupTranslationLanguage` — The language of the target entry.
