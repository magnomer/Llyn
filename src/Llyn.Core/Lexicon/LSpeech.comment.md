# LSpeech.cs

## `public sealed record LSpeech(`

One part-of-speech assignment on an entry, ordered within it. Identity is `(entry_id, position)`. A row says its part of speech in exactly one of two ways: the stable POS id (`LSpeechValueId`) when the language's vocabulary declares one — the display name is then resolved from `LSpeechValue` and never copied here — or the text the user typed (`LSpeechCustom`) when no preset names it.

The second way exists because the field is editable: a user may name a part of speech their language pack does not declare, and refusing to store it would lose what they typed. A custom row is text and stays text — renaming a preset never touches it, and it resolves against nothing — which is exactly why it is a separate member rather than an id that resolves to nothing.

**Parameters**

- `LSpeechEntryId` — Parent entry id.
- `LSpeechPosition` — Order within the parent entry.
- `LSpeechValueId` — Stable, language-controlled POS id (for example `noun`), or `null` when the assignment is custom text no preset declares.
- `LSpeechCustom` — The part of speech exactly as the user typed it, when no preset in the entry's language carries that name; `null` when the row names a declared preset instead.
