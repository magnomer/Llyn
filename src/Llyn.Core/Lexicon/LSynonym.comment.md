# LSynonym.cs

## `public sealed record LSynonym(`

One synonym interlink hanging from a Collocation. A collocation synonym is never Entry-owned text: it points at a lexical target through the same discriminated, checked target model job05 defined for `LRelation` — exactly one of `LSynonymTargetEntry` and `LSynonymTargetSense` is set, and both the store and the table's check constraint enforce that XOR.

TODO: the precise targeting rules for a collocation synonym are not finalized — which target kinds are legal, whether a synonym may point at another Collocation, and whether the link is symmetric are still open. This type is the storage seam only; tighten it once the rules are decided.

**Parameters**

- `LSynonymId` — Opaque, program-generated stable id.
- `LSynonymCollocationId` — Origin Collocation id the synonym hangs from.
- `LSynonymPosition` — Order among the origin Collocation's synonyms.
- `LSynonymTargetEntry` — Target Entry id when the synonym points at an Entry, else `null`.
- `LSynonymTargetSense` — Target Meaning id when the synonym points at a Meaning, else `null`.
