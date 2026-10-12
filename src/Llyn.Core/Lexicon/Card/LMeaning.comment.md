# LMeaning.cs
Hash: `a467044e1d4d18bc`

## `public sealed record LMeaning(long LMeaningId, long LMeaningEntryId, long? LMeaningParentId, int LMeaningPosition, LStateValue LMeaningTitle, LStateValue LMeaningDefinition)`

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

## `public static string? LMeaningPathRead(IReadOnlyList<LCardDraft> meanings, long meaningId)`

The dotted path of a meaning in the draft tree, such as "1.2".
Each step is the rank among siblings, counted from one by list order.
It is the format a mention's `<sense>` uses in markup.
It is null for id 0 or an id absent from the tree.

## `public static long LMeaningPathFind(IReadOnlyList<LCardDraft> meanings, string path)`

The inverse of `LMeaningPathRead`.
It answers 0 when a step is not a number or the path leaves the tree.

## `public static long LMeaningRootRead(IReadOnlyList<LCardDraft> meanings, long meaningId)`

The id of the top-level meaning above a meaning, or the meaning itself at the top.
It answers 0 for an id absent from the tree.

## `public static IReadOnlyList<LMeaning> LMeaningDraftRead(long entryId, IReadOnlyList<LCardDraft> meanings)`

The flat Meaning rows of a draft tree, each owned by `entryId` and carrying its parent id.
A row's position is its index among siblings.
The rows hold the cards' own ids, so a collocation link can be checked against them.
Order across levels is not guaranteed.
