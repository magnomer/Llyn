# LSyllable.cs

## `public sealed record LSyllable(`

One syllable of a pronunciation, ordered within it.
Identity is `(pronunciation_id, position)`: the syllable is subordinate to its `LSyllablePronunciationId` parent, and reordering changes `LSyllablePosition` only.
Every field but `LSyllableNucleus` is optional.
An absent field is stored as NULL, which is distinct from an empty string.

**Parameters**

- `LSyllablePronunciationId` — Parent pronunciation id.
- `LSyllablePosition` — Order within the parent pronunciation.
- `LSyllableOrthography` — Optional written form of the syllable.
- `LSyllableLocal` — Optional local representation of the syllable.
- `LSyllableOnset` — Optional onset segment.
- `LSyllableMedial` — Optional medial segment.
- `LSyllableNucleus` — The nucleus segment, and the only required field.
- `LSyllableCoda` — Optional coda segment.
- `LSyllableToneNumber` — Optional tone number.
- `LSyllableToneLocal` — Optional local tone notation.
- `LSyllableTonePoints` — Optional tone-contour points, free-form text (for example `"214"`).
