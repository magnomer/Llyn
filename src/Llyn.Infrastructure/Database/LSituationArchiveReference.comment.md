# LSituationArchiveReference.cs

## `public sealed partial class LSituationArchive`

The reference side of the Situation store: how Meanings and Collocations point at a Situation.
Attaching and detaching write association rows and renumber that referrer's set through `LDatabaseOrder`.
The referrer read lists the Situations one Meaning or Collocation holds, in its order.
The link delete clears every reference to one Situation from one table.

## `public void LSituationMeaningAttach(long meaningId, long situationId, int position)`

References an existing Situation from a Meaning at `position` in that Meaning's order.

## `public void LSituationCollocationAttach(long collocationId, long situationId, int position)`

References an existing Situation from a Collocation at `position` in that Collocation's order.

## `public void LSituationMeaningDetach(long meaningId, long situationId)`

Removes a Meaning's reference to a Situation.
The Situation and its other references survive.

## `public void LSituationCollocationDetach(long collocationId, long situationId)`

Removes a Collocation's reference to a Situation.
The Situation and its other references survive.

## Inline notes

### `private void LSituationReferenceAttach(`

The two association tables differ only in their name and their referrer column.
So the reference operations share one implementation each.
Both identifiers are store-owned literals chosen by the methods above, never caller input.
So composing them into the statement text opens no injection seam.
Every value still travels as a parameter.

The row goes in beyond the end of the set and the whole set is then renumbered around it.
So the requested index is honoured.
An occupied position is no longer a unique-index failure.

### `private static void LSituationLinkDelete(SqliteConnection connection, string table, long situationId)`

Drops one association table's references to a Situation and renumbers what each referrer has left.
The referrers are read before the delete because afterwards there is nothing left to name them.
A gap in a referrer's positions is a unique-index failure waiting for its next attach.

### `private IReadOnlyList<LSituation> LSituationReferrerRead(string table, string column, long referrerId)`

Reads the Situations one referrer holds through its association table, ordered by position.
The two public readers differ only in the table and column they name.
