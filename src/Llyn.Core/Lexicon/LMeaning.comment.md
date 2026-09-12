# LMeaning.cs

## `public sealed record LMeaning(`

One Meaning owned by an entry: a node in the entry's self-referential Meaning tree, carrying a single inline definition field.
`LMeaningId` is the identity, an opaque and program-generated stable id.
It is the base every later tag, situation, and example association targets.
A Meaning nests under another through `LMeaningParentId`, which always names a Meaning in the same entry.
A root Meaning has no parent.
There is no separate definition entity: each Meaning holds exactly one definition (with its own optional language), empty when unset.

**Parameters**

- `LMeaningId` — Opaque, program-generated stable id.
- `LMeaningEntryId` — Owning entry id.
- `LMeaningParentId` — Parent Meaning id in the same entry, or `null` for a root Meaning.
- `LMeaningPosition` — Order within its siblings under the same parent.
- `LMeaningTitle` — Title typed on the Meaning card, and what is known about it.
  Nothing was recorded when none was typed.
  It is unknown when the user marked it as not known.
- `LMeaningGloss` — Optional short gloss, unused by the input form.
- `LMeaningDefinitionLanguage` — Optional language the definition is written in.
- `LMeaningDefinition` — Single inline definition text, and what is known about it.
- `LMeaningLabels` — Labels as JSON array text (for example `["figurative"]`).
