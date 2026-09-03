# LTranslationTarget.cs

## `public sealed record LTranslationTarget(`

The resolved Entry an `LTranslation` points at.
It carries the headword and language a shown link needs.
The record is read back from the target Entry and never stored beside the link.

**Parameters**

- `LTranslationTargetId` — Id of the resolved Entry.
- `LTranslationTargetHeadword` — Headword of the resolved Entry.
- `LTranslationTargetLanguage` — Language code of the resolved Entry.
