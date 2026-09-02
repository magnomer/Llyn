# LTagArchive.cs

## `public sealed class LTagArchive`

Persists Tags.
A Tag has no identity apart from the text it reads.
So there is no Tag row to create and no id to carry.
A Tag exists exactly where a Meaning or a Collocation writes it.
Two cards writing the same words hold the same Tag by saying the same thing.
The association tables are therefore the whole of the storage.
Each row is one card, one text and the position that text takes on that card alone.
The catalog of Tags is read back out of them rather than kept beside them.

A card's Tag line is written as a whole rather than edited a reference at a time.
The old rows go and the new ones are numbered from zero in the order given.
Blank texts and repeats are dropped on the way.
Editing a card is exactly that write.
So no caller has to work out which Tags were added and which were taken away.
A card's positions are contiguous by construction rather than by repair.

Renaming and deleting a Tag reach across every card that wrote it, because that is what a Tag is.
Both fold and close.
Renaming onto a text a card already carries leaves that card one Tag, not two.
Deleting closes the gap the removed text left in every card's order.

## `public LTagArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LTag> LTagSenseRead(string senseId)`

Reads the Tags a Meaning carries, in the order that Meaning gives them.

## `public IReadOnlyList<LTag> LTagCollocationRead(string collocationId)`

Reads the Tags a Collocation carries, in the order that Collocation gives them.

## `public void LTagSenseSave(string senseId, IReadOnlyList<LTag> tags)`

Writes a Meaning's whole Tag line, replacing whatever it carried.
Texts are trimmed, blanks and repeats are dropped, and what survives is numbered from zero in the order given.

## `public void LTagCollocationSave(string collocationId, IReadOnlyList<LTag> tags)`

The same write for a Collocation.

## `public IReadOnlyList<LTag> LTagCatalogRead()`

Reads every Tag any card carries, once each, in alphabetical order.
That is the Tag list itself.
It is gathered from the cards that write it, because nothing else holds it.

## `public int LTagReferenceRead(string text)`

Counts the cards carrying `text`.
A Tag no card carries is not a Tag that was deleted.
It simply is not written anywhere.

## `public void LTagChange(string text, string renamed)`

Renames a Tag everywhere it is written.
A card already carrying `renamed` keeps one Tag rather than gaining a duplicate.
That card's order closes over the row that folded away.

## `public void LTagDelete(string text)`

Takes a Tag off every card that carries it, closing the gap it leaves in each card's order.
Nothing else is deleted: a card that carried only this Tag stays, now carrying none.

## Inline notes

### `private static void LTagOwnerNormalize(SqliteConnection connection, string table, string column, string owner)`

One card's rows are read in order, deleted and written back numbered from zero.
The rewrite rather than an in-place shift keeps the unique index on (card, position) satisfied.
A shift would have to pass through a position another row still holds.

### `private void LTagReferrerSave(string table, string column, string referrerId, IReadOnlyList<LTag> tags)`

The two association tables differ only in their name and their card column, so the write is one implementation.
Both identifiers are store-owned literals chosen by the methods above, never caller input.
So composing them into the statement text opens no injection seam.
Every value still travels as a parameter.
