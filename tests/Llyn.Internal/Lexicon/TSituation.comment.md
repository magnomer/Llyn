# TSituation.cs

## `public sealed class TSituation`

Covers the engine's Situation seam, which mirrors the Tag one.
A Situation is created, read, rewritten and referenced from both card kinds.
It covers the delete the references refuse and the detach that leaves the row standing.
Situations on a card are read back through the entry's load, as the editor reads them.
The Images and Videos a Situation shows are covered by `TSituationMedia`.

## `public void SituationPick_CollocationCard_LandsOnThatCard()`

A request names its card by id alone.
So a Meaning and a Collocation of one entry must never share an id.
A pick aimed at the Collocation of a fresh entry once landed on its Meaning, since both were number 1.

## `public void SituationRead_WorkspaceShelf_ReturnsReferenceCounts()`

The panel browsing the shelf itself must see a Situation nothing references, which no card read can reach.
It must also see the count on every row without asking once per row.
A Situation nothing references is absent from the counts rather than present as zero.

## `public void UsageRead_SituationOnCard_NamesSideAndEntry()`

A count says how many places carry a Situation.
This says which Meaning or Collocation, and under which Entry, so the relationship is never flattened into the Entry alone.
A Meaning is named by its definition and a Collocation by its expression when neither carries a title.

## `public void SituationDelete_DetachingDelete_DropsAndRenumbers()`

The delete the user confirms after being told how far it reaches.
Every reference goes with the row, on both association tables.
What each card has left is renumbered.
A gap in a card's positions is the next attach failing on the unique index.

## Inline notes

### `Assert.Equal(`

The second one asked for position 0, so it takes it and the set is renumbered around it.

### `private static LCardDraft TSituationCardRead(LEngine engine, long entryId, bool collocation)`

The first Meaning or Collocation card of the entry as its load returns it.

### `private static void TSituationPickApply(`

References a stored Situation from the first Meaning the way the entry form does, by picking it in a draft.

### `private static LEntry TSituationEntryCreate(LEngine engine)`

One saved entry with a Meaning card and a Collocation card, neither carrying situations of its own.

### `private static LEntry TSituationEntryCreate(`

The same entry, with each card referencing the stored Situations given for it, in the order given.

### `private static List<LSituationDraft> TSituationDraftRead(IReadOnlyList<LSituation> situations)`

The chips a form holds for stored Situations, so the save picks them rather than minting new ones.
