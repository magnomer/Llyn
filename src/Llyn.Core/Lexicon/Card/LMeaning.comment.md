# LMeaning.cs
Hash: `39b3ef5e9866c04c`

## `public sealed record LMeaning(`

One Meaning owned by an entry.
It is a node in the entry's self-referential Meaning tree.
It carries a title and a single inline definition.
`LMeaningId` is the identity, an opaque and program-generated stable id.
It is the base every later tag, situation, and example association targets.
A Meaning nests under another through `LMeaningParentId`, which always names a Meaning in the same entry.
A root Meaning has no parent.
There is no separate definition entity.
Each Meaning holds exactly one definition, empty when unset.

**Parameters**

- `LMeaningId` — Opaque, program-generated stable id.
- `LMeaningEntryId` — Owning entry id.
- `LMeaningParentId` — Parent Meaning id in the same entry, or `null` for a root Meaning.
- `LMeaningPosition` — Order within its siblings under the same parent.
- `LMeaningTitle` — Title typed on the Meaning card, and what is known about it.
  Nothing was recorded when none was typed.
  It is unknown when the user marked it as not known.
- `LMeaningDefinition` — Single inline definition text, and what is known about it.

## `public string LMeaningName`

The text a Meaning is listed under, which is its title, then its definition, then empty.
A list that finds it empty prints its own unknown mark instead.
