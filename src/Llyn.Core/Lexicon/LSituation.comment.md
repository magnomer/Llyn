# LSituation.cs

## `public sealed record LSituation(`

One Situation — the usage context a Meaning or Collocation belongs to, and independent data owned by nothing. No Entry, Meaning, or Collocation contains a Situation; any number of Meanings and Collocations *reference* it instead, and the order a Situation appears in lives on each reference rather than here. `LSituationId` is the identity — an opaque, program-generated stable id; the title, description, and kind are visible data and never identity, so editing any of them leaves the id and every reference to it untouched.

**Parameters**

- `LSituationId` — Opaque, program-generated stable id.
- `LSituationTitle` — The situation title; display text, never identity.
- `LSituationDescription` — Description of the situation, or `null` when absent.
- `LSituationKind` — Situation/context classification, or `null` when unclassified.
