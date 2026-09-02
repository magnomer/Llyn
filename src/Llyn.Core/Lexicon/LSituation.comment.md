# LSituation.cs

## `public sealed record LSituation(`

One Situation — the usage context a Meaning or Collocation belongs to, and independent data owned by nothing. No Entry, Meaning, or Collocation contains a Situation; any number of Meanings and Collocations *reference* it instead, and the order a Situation appears in lives on each reference rather than here. `LSituationId` is the identity — an opaque, program-generated stable id; the title, description, and kind are visible data and never identity, so editing any of them leaves the id and every reference to it untouched. A Situation also references *at most one* Source through `LSituationSource`, the way an Example does: a pointer, never ownership. Every field here that can stand empty carries `LStateValue`, so a field holding nothing says whether nothing was ever recorded or something was recorded that cannot be read back.

**Parameters**

- `LSituationId` — Opaque, program-generated stable id.
- `LSituationTitle` — The situation title and what is known about it; display text, never identity.
- `LSituationDescription` — Description of the situation, and what is known about it.
- `LSituationKind` — Situation/context classification, and what is known about it.
- `LSituationSource` — The single referenced Source: its id when one is cited, nothing recorded when none is, unreadable when the citation cannot be read back.
