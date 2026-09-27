# CMarkupEntry.cs

## `public sealed record CMarkupEntry(`

One entry a markup file carries, as the import dialog lists it before anything is stored.
The dialog needs only what it shows and what it searches the workspace by.

**Parameters**

- `CMarkupEntryHeadword`: the headword the workspace is searched by.
- `CMarkupEntryLanguage`: the language the workspace is searched by.
- `CMarkupEntryName`: the headword as the dialog shows it.
