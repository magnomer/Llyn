# LRelation.cs

## `public sealed record LRelation(`

One lexical relation originating from a Meaning and pointing at exactly one target — an Entry or another Meaning — through a checked reference, never a free-text id. `LRelationId` is the identity; `LRelationSenseId` names the origin Meaning the relation hangs from. The target is discriminated: exactly one of `LRelationTargetEntry` and `LRelationTargetSense` is set, and the store enforces that XOR. This is also the synonym/collocation interlink mechanism later jobs reuse, so the target model stays reusable.

**Parameters**

- `LRelationId` — Opaque, program-generated stable id.
- `LRelationSenseId` — Origin Meaning id the relation hangs from.
- `LRelationPosition` — Order among the origin Meaning's relations.
- `LRelationType` — Stable relation-type id (for example the synonym or antonym type).
- `LRelationLabel` — Optional single label text.
- `LRelationLabels` — Labels as JSON array text (same format as job04), or `null`.
- `LRelationTargetEntry` — Target Entry id when the relation points at an Entry, else `null`.
- `LRelationTargetSense` — Target Meaning id when the relation points at a Meaning, else `null`.
