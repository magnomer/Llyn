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
The images and videos it shows are independent records too, referenced in the order the reference holds.
So a Situation reaches its media exactly as a Meaning does.
Every field here that can stand empty carries `LStateValue`.
A field holding nothing says whether nothing was ever recorded.
It says instead when the user marked the value as not known.

**Parameters**

- `LSituationId` — Opaque, program-generated stable id.
- `LSituationTitle` — The situation title and what is known about it, display text and never identity.
- `LSituationDescription` — Description of the situation, and what is known about it.
- `LSituationKind` — Situation/context classification, and what is known about it.
- `LSituationImage` — The Images the Situation shows, as [LImageDraft](LImageDraft.comment.md) rows in the order the user keeps them.
- `LSituationVideo` — The Videos the Situation shows, as [LVideoDraft](LVideoDraft.comment.md) rows in the order the user keeps them.

## `public bool Equals(LSituation? other)`

Two Situations are equal when every field is equal and the Image and Video lists match row by row.
A record compares a list by reference, which would make every read a change.

## `public override int GetHashCode()`

The hash that agrees with that equality.

## `public LSituation LSituationNormalize()`

The same Situation with every unreadable value dropped to unspecified.
Each Image and Video row is normalized the same way.
Called only after the user agreed to lose what the store could not read.
