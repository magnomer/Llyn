# LSpeech.cs

## `public sealed record LSpeech(`

One part-of-speech assignment on an entry, ordered within it.
Identity is `(entry_parent, position)`.
A row says its part of speech in exactly one of two ways.
The first is a link to a `LSpeechValue` row (`LSpeechValueId`).
The display name is then read from that row and never copied here.
The second is the text the user typed (`LSpeechCustom`) when no value names it.

The second way exists because the field is editable.
A user may name a part of speech their language pack does not declare.
Refusing to store it would lose what they typed.
A custom row is text and stays text.
Renaming a value never touches it, and it resolves against nothing.

**Parameters**

- `LSpeechEntryId` — Parent entry id.
- `LSpeechPosition` — Order within the parent entry.
- `LSpeechValueId` — Linked `speech_value` row, or `null` when the assignment is custom text.
- `LSpeechCustom` — The part of speech exactly as the user typed it.
  It is `null` when the row links a value instead.
