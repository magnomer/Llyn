# LSpeech.cs

## `public sealed record LSpeech(`

One part-of-speech assignment on an entry, ordered within it.
Identity is `(entry_id, position)`.
A row says its part of speech in exactly one of two ways.
The first is the stable POS id (`LSpeechValueId`) when the language's vocabulary declares one.
The display name is then resolved from `LSpeechValue` and never copied here.
The second is the text the user typed (`LSpeechCustom`) when no preset names it.

The second way exists because the field is editable.
A user may name a part of speech their language pack does not declare.
Refusing to store it would lose what they typed.
A custom row is text and stays text.
Renaming a preset never touches it, and it resolves against nothing.
That is exactly why it is a separate member rather than an id resolving to nothing.

**Parameters**

- `LSpeechEntryId` — Parent entry id.
- `LSpeechPosition` — Order within the parent entry.
- `LSpeechValueId` — Stable, language-controlled POS id (for example `noun`), or `null` when the assignment is custom text no preset declares.
- `LSpeechCustom` — The part of speech exactly as the user typed it.
  It is used when no preset in the entry's language carries that name.
  It is `null` when the row names a declared preset instead.
