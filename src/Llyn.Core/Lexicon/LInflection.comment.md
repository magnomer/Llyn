# LInflection.cs

## `public sealed record LInflection(`

One inflected form of an entry, ordered within it. Identity is `(entry_id, position)`: the inflection is subordinate to its `LInflectionEntryId` parent, and reordering changes `LInflectionPosition` only. It carries its own grammatical features in order (`LInflectionFeatures`) and stores only the stable part-of-speech id (`LInflectionSpeechId`), never a display name.

**Parameters**

- `LInflectionEntryId` — Parent entry id.
- `LInflectionPosition` — Order within the parent entry.
- `LInflectionText` — The inflected form.
- `LInflectionLocal` — Optional local representation.
- `LInflectionSpeechId` — Optional stable part-of-speech id; `null` when unspecified.
- `LInflectionFeatures` — Ordered grammatical features carried by the inflection.
