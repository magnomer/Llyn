# LSpeechValue.cs

## `public sealed record LSpeechValue(`

One entry in the language-controlled part-of-speech display vocabulary.
It maps a stable POS id to the display name shown for a given language.
Entries store only the id (`LSpeech`).
The name lives here and is resolved by `(language, value_id)`, never copied onto the entry's rows.

**Parameters**

- `LSpeechValueLanguage` — Language the display name is governed by.
- `LSpeechValueId` — Stable POS id (for example `noun`).
- `LSpeechValueName` — Display name for the language (for example `Noun`).
- `LSpeechValuePosition` — Display order within the language's vocabulary.
