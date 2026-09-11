# LInflection.cs

## `public sealed record LInflection(`

One inflected form of an entry, ordered within it.
The inflection has its own row id so its morphology links can name it directly.
It is subordinate to its `LInflectionEntryId` parent, and reordering changes `LInflectionPosition` only.
It carries the morphology values it shows, in order (`LInflectionMorphology`).
It links its part of speech by row id (`LInflectionSpeechId`), never by name.

**Parameters**

- `LInflectionId` — Row id, `0` before the row is stored.
- `LInflectionEntryId` — Parent entry id.
- `LInflectionPosition` — Order within the parent entry.
- `LInflectionText` — The inflected form.
- `LInflectionLocal` — Optional local representation.
- `LInflectionSpeechId` — Linked `speech_value` row, or `null` when unspecified.
- `LInflectionMorphology` — Ordered `morphology_value` row ids the inflection carries.
  Each value knows its feature, so the feature is not repeated here.
