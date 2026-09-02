# LSense.cs

## `public sealed record LSense(`

One Meaning owned by an entry: a node in the entry's self-referential Meaning tree, carrying a single inline definition field.
`LSenseId` is the identity, an opaque and program-generated stable id.
It is the base every later relation, tag, situation, and example association targets.
A Meaning nests under another through `LSenseParentId`, which always names a Meaning in the same entry.
A root Meaning has no parent.
There is no separate definition entity: each Meaning holds exactly one definition (with its own optional language), empty when unset.

**Parameters**

- `LSenseId` — Opaque, program-generated stable id.
- `LSenseEntryId` — Owning entry id.
- `LSenseParentId` — Parent Meaning id in the same entry, or `null` for a root Meaning.
- `LSensePosition` — Order within its siblings under the same parent.
- `LSenseTitle` — Title typed on the Meaning card, and what is known about it.
  Nothing was recorded when none was typed.
  It is unreadable when what was typed cannot be read back.
- `LSenseGloss` — Optional short gloss, unused by the input form.
- `LSenseDefinitionLanguage` — Optional language the definition is written in.
- `LSenseDefinition` — Single inline definition text, and what is known about it.
- `LSenseLabels` — Labels as JSON array text (for example `["figurative"]`).
