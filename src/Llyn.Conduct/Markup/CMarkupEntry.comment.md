# CMarkupEntry.cs
Hash: `67623e63568aa0d1`

## `public sealed record CMarkupEntry(`

One entry a markup file carries, as the import dialog lists it before anything is stored.
The dialog needs only what it shows, so its targets arrive found.

**Parameters**

- `CMarkupEntryLanguage`: the language the dialog shows.
- `CMarkupEntryName`: the headword as the dialog shows it.
- `CMarkupEntryTarget`: the ids of the stored entries it may join, found by the engine before the question.
