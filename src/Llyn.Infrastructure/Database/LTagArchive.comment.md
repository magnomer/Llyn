# LTagArchive.cs

## `public sealed class LTagArchive`

Persists Tags.
A Tag is a row of its own, and a card links it by id.
Two cards carrying the same word link one row rather than each writing the word.
The text is kept unique, so a word names at most one row.
The association tables hold one row per card, Tag and the position that Tag takes on that card alone.
The catalog of Tags is the tag table itself, whether or not any card still links a row.
A row no card links stays until the taxonomy panel deletes it.

A card's Tag line is written as a whole rather than edited a reference at a time.
The old links go and the new ones are numbered from zero in the order given.
A Tag carrying an id links that row.
A Tag carrying only text links the row reading the same, created if absent.
Blank texts and repeats are dropped on the way.
Editing a card is exactly that write.
So no caller has to work out which Tags were added and which were taken away.
A card's positions are contiguous by construction rather than by repair.

Renaming a Tag changes one row and every card follows.
Renaming onto a text another row already reads folds the two rows into one.
A card carrying both keeps one link, not two, and its order closes over the link that folded away.
Deleting a Tag unlinks every card and closes the gap in each card's order before the row goes.

## `public LTagArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LTag> LTagMeaningRead(long meaningId)`

Reads the Tags a Meaning carries, in the order that Meaning gives them.

## `public IReadOnlyList<LTag> LTagCollocationRead(long collocationId)`

Reads the Tags a Collocation carries, in the order that Collocation gives them.

## `public IReadOnlyList<long> LTagMeaningSave(long meaningId, IReadOnlyList<LTag> tags)`

Writes a Meaning's whole Tag line, replacing whatever it carried.
Each Tag is resolved to a row by id, or by trimmed text when it carries no stored id.
Blanks and repeats are dropped, and what survives is numbered from zero in the order given.
The answer holds one row id per Tag handed in, zero for a blank.
The caller maps a draft id to its row from it.

## `public IReadOnlyList<long> LTagCollocationSave(long collocationId, IReadOnlyList<LTag> tags)`

The same write for a Collocation.

## `public LTag? LTagRead(long id)`

Reads one Tag by id, `null` when no row carries it.

## `public long LTagResolve(string text)`

The id of the row reading `text`, created when no row reads it yet.

## `public IReadOnlyList<LTag> LTagCatalogRead()`

Reads every Tag row, in alphabetical order.
That is the Tag list itself.

## `public int LTagReferenceRead(long id)`

Counts the cards linking the Tag.

## `public void LTagChange(long id, string renamed)`

Renames a Tag everywhere it is linked by rewriting its one row.
When another row already reads `renamed`, the two fold into that row.
A card already carrying both keeps one link rather than gaining a duplicate.
That card's order closes over the link that folded away.

## `public void LTagDelete(long id)`

Unlinks a Tag from every card that carries it and closes the gap it leaves in each card's order.
Then it deletes the row.
Nothing else is deleted: a card that carried only this Tag stays, now carrying none.

## Inline notes

### `private void LTagReferrerSave(string table, string column, long referrerId, IReadOnlyList<LTag> tags)`

The two association tables differ only in their name and their card column, so the write is one implementation.
Both identifiers are store-owned literals chosen by the methods above, never caller input.
So composing them into the statement text opens no injection seam.
Every value still travels as a parameter.
An id the caller hands over that names no row is treated as text.
A Tag deleted under an open card should still save as the word it shows.
