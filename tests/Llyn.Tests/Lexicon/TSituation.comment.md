# TSituation.cs

## `public sealed class TSituation`

Covers the engine's Situation seam, which mirrors the Tag one.
A Situation is created, read, rewritten and referenced from both card kinds.
It covers the difference between a detach and a remove.
It covers the delete the references refuse.
The Images and Videos a Situation shows are covered by `TSituationMedia`.

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

### `private static LEntry TSituationEntryCreate(LEngine engine)`

One saved entry with a Meaning card and a Collocation card, neither carrying situations of its own.
