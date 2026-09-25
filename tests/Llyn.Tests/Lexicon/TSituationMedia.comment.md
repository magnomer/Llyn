# TSituationMedia.cs

## `public sealed class TSituationMedia`

Covers the Images and Videos a Situation shows, the way a Meaning shows its own.
Media rows are created with the Situation, kept or dropped on update, and unlinked on delete.
The Image and Video records themselves outlive every link.

## `public void SituationCreate_WithMedia_ReadsBackInOrder()`

A Situation shows Images and Videos the way a Meaning does.
The rows come back with their ids and in the order given.
Each Image and Video counts the Situation as a referrer.

## `public void SituationUpdate_MediaDropped_DetachesAndKeepsRecord()`

An update names the rows to keep and the rows to add.
What it leaves out is detached and renumbered around, and its record stays for whatever else shows it.

## `public void SituationRead_ListWithMedia_GroupsEveryRowUnderItsOwner()`

The catalog read fills media in two grouped queries rather than two per Situation.
Three Situations with different media prove each row lands under its own owner and none leaks to a neighbour.

## `public void SituationDelete_WithMedia_DropsLinksAndKeepsRecords()`

Deleting the Situation takes its media links with it and never the Image or Video rows.
