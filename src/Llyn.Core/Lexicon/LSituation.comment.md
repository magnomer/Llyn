# LSituation.cs

## `public sealed record LSituation(`

One Situation — the usage context a Meaning or Collocation belongs to, and independent data owned by nothing.
No Entry, Meaning, or Collocation contains a Situation.
Any number of Meanings and Collocations *reference* it instead.
The order a Situation appears in lives on each reference rather than here.
`LSituationId` is the identity, an opaque and program-generated stable id.
The title, description, and kind are visible data and never identity.
Editing any of them leaves the id and every reference to it untouched.
A Situation cites nothing: it is a description the user writes, not a passage quoted from a work.
Every field here that can stand empty carries `LStateValue`.
A field holding nothing says whether nothing was ever recorded.
It says instead when something was recorded that cannot be read back.

**Parameters**

- `LSituationId` — Opaque, program-generated stable id.
- `LSituationTitle` — The situation title and what is known about it, display text and never identity.
- `LSituationDescription` — Description of the situation, and what is known about it.
- `LSituationKind` — Situation/context classification, and what is known about it.
