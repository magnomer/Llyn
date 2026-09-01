# LTranslation.cs

## `public sealed record LTranslation(`

One translation of an `LExample` into another language. A translation is owned text: it belongs to exactly one Example, is ordered within it, and is removed with it. Its identity is `LTranslationId` — an opaque, program-generated stable id — so reordering rewrites `LTranslationPosition` only and never changes which translation is which.

**Parameters**

- `LTranslationId` — Opaque, program-generated stable id.
- `LTranslationLanguage` — Language code the translation is written in.
- `LTranslationText` — The translated text.
- `LTranslationPosition` — Order among the owning Example's translations.
