# LSense.cs

## `public sealed record LSense(`

One Meaning owned by an entry: a node in the entry's self-referential Meaning tree, carrying a single inline definition field. `LSenseId` is the identity — an opaque, program-generated stable id — and is the base every later relation, tag, situation, and example association targets. A Meaning nests under another through `LSenseParentId`, which always names a Meaning in the same entry; a root Meaning has no parent. There is no separate definition entity: each Meaning holds exactly one definition (with its own optional language), empty when unset.

**Parameters**

- `LSenseId` — Opaque, program-generated stable id.
- `LSenseEntryId` — Owning entry id.
- `LSenseParentId` — Parent Meaning id in the same entry, or `null` for a root Meaning.
- `LSensePosition` — Order within its siblings under the same parent.
- `LSenseTitle` — Title typed on the Meaning card; `null` when none was typed.
- `LSenseGloss` — Optional short gloss, unused by the input form.
- `LSenseDefinitionLanguage` — Optional language the definition is written in.
- `LSenseDefinition` — Single inline definition text; empty when unset.
- `LSenseLabels` — Labels as JSON array text (for example `["figurative"]`).
