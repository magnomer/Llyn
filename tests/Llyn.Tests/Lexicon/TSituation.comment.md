# TSituation.cs

## `public sealed class TSituation`

Covers the engine's Situation seam, which mirrors the Tag one.
A Situation is created, read, rewritten and referenced from both card kinds.
It covers the difference between a detach and a remove.
It covers the delete the references refuse.

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

## `public void SituationCreate_WithMedia_ReadsBackInOrder()`

A Situation shows Images and Videos the way a Meaning does.
The rows come back with their ids and in the order given.
Each Image and Video counts the Situation as a referrer.

## `public void SituationUpdate_MediaDropped_DetachesAndKeepsRecord()`

An update names the rows to keep and the rows to add.
What it leaves out is detached and renumbered around, and its record stays for whatever else shows it.

## `public void SituationRead_ListWithMedia_GroupsEveryRowUnderItsOwner()`

The list read fills media in two grouped queries rather than two per Situation.
Three Situations with different media prove each row lands under its own owner and none leaks to a neighbour.

## `public void SituationDelete_WithMedia_DropsLinksAndKeepsRecords()`

Deleting the Situation takes its media links with it and never the Image or Video rows.

## Inline notes

### `Assert.Equal(`

The second one asked for position 0, so it takes it and the set is renumbered around it.

### `private static LEntry TSituationEntryCreate(LEngine engine)`

One saved entry with a Meaning card and a Collocation card, neither carrying situations of its own.
